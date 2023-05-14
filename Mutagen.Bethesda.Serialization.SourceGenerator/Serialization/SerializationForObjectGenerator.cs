using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class SerializationForObjectGenerator
{
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly SerializationFieldGenerator _forFieldGenerator;
    private readonly WhereClauseGenerator _whereClauseGenerator;
    private readonly LoquiSerializationNaming _loquiSerializationNaming;
    private readonly PropertyCollectionRetriever _propertyCollectionRetriever;
    private readonly CustomizationDriver _customizationDriver;
    private readonly ObjectTypeTester _modObjectTypeTester;
    private readonly ReleaseRetriever _releaseRetriever;
    private readonly ISerializationInterceptor[] _serializationInterceptors;

    public SerializationForObjectGenerator(
        LoquiNameRetriever nameRetriever,
        SerializationFieldGenerator forFieldGenerator,
        WhereClauseGenerator whereClauseGenerator,
        LoquiSerializationNaming loquiSerializationNaming,
        PropertyCollectionRetriever propertyCollectionRetriever, 
        CustomizationDriver customizationDriver,
        ObjectTypeTester modObjectTypeTester,
        ReleaseRetriever releaseRetriever,
        ISerializationInterceptor[] serializationInterceptors)
    {
        _nameRetriever = nameRetriever;
        _forFieldGenerator = forFieldGenerator;
        _whereClauseGenerator = whereClauseGenerator;
        _loquiSerializationNaming = loquiSerializationNaming;
        _propertyCollectionRetriever = propertyCollectionRetriever;
        _customizationDriver = customizationDriver;
        _modObjectTypeTester = modObjectTypeTester;
        _releaseRetriever = releaseRetriever;
        _serializationInterceptors = serializationInterceptors;
    }
    
    public void Generate(
        CompilationUnit compilation, 
        LoquiTypeSet typeSet)
    {
        compilation.Context.CancellationToken.ThrowIfCancellationRequested();
        
        var baseType = compilation.Mapping.TryGetBaseClass(typeSet);
        var inheriting = compilation.Mapping.TryGetDeepInheritingClasses(typeSet);
        
        var sb = new StructuredStringBuilder();

        var properties = _propertyCollectionRetriever.GetPropertyCollection(
            compilation,
            typeSet);
        
        GenerateUsings(compilation, typeSet, sb, properties);

        GenerateWarningSuppressions(sb);

        using (sb.Namespace(typeSet.GetAny().ContainingNamespace.ToString()))
        {
        }
        
        if (!_loquiSerializationNaming.TryGetSerializationItems(typeSet.GetAny(), out var objSerializationItems)) return;
        
        var gens = GetGenerics(typeSet);

        var isConcrete = _modObjectTypeTester.IsConcrete(typeSet.Direct);

        var isGroup = _modObjectTypeTester.IsGroup(typeSet.Setter);
        
        using (var c = sb.Class(objSerializationItems.SerializationHousingClassName))
        {
            c.AccessModifier = AccessModifier.Internal;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            if (inheriting.Count > 0)
            {
                GenerateSerializeWithCheck(compilation, typeSet, sb, gens, inheriting);
            }
            
            GenerateSerialize(compilation, typeSet, sb, baseType, properties, gens);
            
            GenerateSerializeFields(compilation, typeSet, sb, baseType, properties, gens);
            
            if (inheriting.Count > 0)
            {
                GenerateHasSerializationItemsWithCheck(compilation, typeSet, sb, gens, inheriting);
            }
            
            GenerateHasSerializationItems(compilation, typeSet, sb, baseType, properties, gens);
            
            if (inheriting.Count > 0)
            {
                GenerateDeserializeWithCheck(compilation, typeSet, sb, gens, inheriting);
            }

            if (isConcrete && !isGroup)
            {
                GenerateDeserialize(compilation, typeSet, sb, properties, gens);
            }
            
            GenerateDeserializeInto(compilation, typeSet, sb, baseType, properties, gens);
        }
        sb.AppendLine();
        
        compilation.Context.AddSource(objSerializationItems.SerializationHousingFileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private void GenerateWarningSuppressions(StructuredStringBuilder sb)
    {
        sb.AppendLine("#pragma warning disable CA1998 // No awaits used");
        sb.AppendLine("#pragma warning disable CS0618 // Obsolete");
        sb.AppendLine();
    }

    private void GenerateDeserialize(
        CompilationUnit compilation,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        PropertyCollection properties,
        SerializationGenerics generics)
    {
        if (typeSet.Direct == null) return;
        
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        var isMajorRecord = _modObjectTypeTester.IsMajorRecordObject(typeSet.Getter);
        var isModHeader = _modObjectTypeTester.IsModHeader(typeSet.Getter);
        
        var genString = generics.ReaderGenericsString();
        using (var args = sb.Function($"public static async Task<{typeSet.Direct}> Deserialize{genString}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            if (isMod)
            {
                args.Add($"ModKey modKey");
                args.Add($"{_releaseRetriever.GetReleaseName(typeSet.Getter!)}Release release");
                args.Add($"IWorkDropoff? workDropoff");
                args.Add($"IFileSystem? fileSystem");
                args.Add($"ICreateStream? streamCreator");
            }
            else
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.Add("where TReadObject : IContainStreamPackage");
            args.Wheres.AddRange(generics.ReaderWheres());
        }
        using (sb.CurlyBrace())
        {
            if (isMajorRecord)
            {
                sb.AppendLine($"var obj = new {typeSet.Direct}(kernel.ExtractFormKey(reader), metaData.Release.To{_releaseRetriever.GetReleaseName(typeSet.Getter!)}Release());");
            }
            else if (isMod)
            {
                sb.AppendLine($"var obj = new {typeSet.Direct}(modKey, release);");
            }
            else
            {
                sb.AppendLine($"var obj = new {typeSet.Direct}();");
            }

            if (isModHeader)
            {
                sb.AppendLine($"obj.FormVersion = metaData.Release.GetDefaultFormVersion()!.Value;");
            }
            using (var c = sb.Call($"await DeserializeInto{genString}"))
            {
                c.AddPassArg("reader");
                c.AddPassArg("kernel");
                c.AddPassArg("obj");
                if (isMod)
                {
                    c.AddPassArg("workDropoff");
                    c.AddPassArg("fileSystem");
                    c.AddPassArg("streamCreator");
                }
                else
                {
                    c.AddPassArg("metaData");
                }
            }
            sb.AppendLine("return obj;");
        }
        sb.AppendLine();
    }

    private void GenerateDeserializeInto(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics)
    {
        var genString = generics.ReaderGenericsString();
        var isMod = _modObjectTypeTester.IsModObject(obj.Getter);
        
        var hadIntoFields = GenerateDeserializeSingleFieldInto(compilation, obj, sb, baseType, properties, generics, genString);

        using (var args = sb.Function($"public static async Task DeserializeInto{genString}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            args.Add($"{obj.DeserializeSymbol} obj");
            if (isMod)
            {
                args.Add("IWorkDropoff? workDropoff");
                args.Add("IFileSystem? fileSystem");
                args.Add($"ICreateStream? streamCreator");
            }
            else
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.Add("where TReadObject : IContainStreamPackage");
            args.Wheres.AddRange(generics.ReaderWheres());
        }
        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                GenerateMetaConstruction(sb, "obj", "workDropoff", "fileSystem", "streamCreator");
            }

            _customizationDriver.SerializationPreWork(obj, compilation, sb, properties);

            GenerateDeserializeCall(compilation, obj, sb, baseType, properties, generics, 
                generateLoop: hadIntoFields);
            
            _customizationDriver.SerializationPostWork(obj, compilation, sb, properties);
        }
        sb.AppendLine();
    }

    private bool GenerateDeserializeSingleFieldInto(
        CompilationUnit compilation, 
        LoquiTypeSet obj,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties, 
        SerializationGenerics generics,
        string genString)
    {        
        var hasInheritingClasses = compilation.Mapping.HasInheritingClasses(obj);
        var hasBaseClass = compilation.Mapping.TryGetBaseClass(obj) != null;

        var alwaysGenerate = hasInheritingClasses || hasBaseClass;

        var orig = sb;
        sb = new StructuredStringBuilder();
        using (var args = sb.Function($"public static async Task DeserializeSingleFieldInto{genString}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            args.Add($"{obj.DeserializeSymbol} obj");
            args.Add("SerializationMetaData metaData");
            args.Add("string name");

            args.Wheres.Add("where TReadObject : IContainStreamPackage");
            args.Wheres.AddRange(generics.ReaderWheres());
        }

        using (sb.CurlyBrace())
        {
            sb.AppendLine("switch (name)");
            using (sb.CurlyBrace())
            {
                bool generatedForProperty = false;
                foreach (var prop in properties.InOrder)
                {
                    compilation.Context.CancellationToken.ThrowIfCancellationRequested();
                    _customizationDriver.WrapOmission(compilation, sb, prop, () =>
                    {
                        var subSb = new StructuredStringBuilder();
                        _forFieldGenerator.GenerateDeserializeForField(
                            compilation: compilation,
                            obj: obj,
                            fieldType: prop.Property.Type,
                            readerAccessor: "reader",
                            fieldName: prop.Property.Name,
                            fieldAccessor: $"obj.{prop.Property.Name}",
                            canSet: prop.Property.IsSettable(),
                            prop.Generator,
                            sb: subSb,
                            cancel: compilation.Context.CancellationToken);
                        if (subSb.Count == 0) return;
                        generatedForProperty = true;

                        sb.AppendLine($"case \"{prop.Property.Name}\":");
                        using (sb.IncreaseDepth())
                        {
                            sb.AppendLines(subSb);
                            sb.AppendLine("break;");
                        }
                    });
                }

                if (!generatedForProperty && !alwaysGenerate)
                {
                    return false;
                }

                sb.AppendLine("default:");
                using (sb.IncreaseDepth())
                {
                    if (baseType != null
                        && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
                    {
                        sb.AppendLine(
                            $"await {baseSerializationItems.DeserializationSingleFieldIntoCall()}{genString}(reader, kernel, obj, metaData, name);");
                    }

                    sb.AppendLine("break;");
                }
            }
        }

        sb.AppendLine();
        orig.AppendLines(sb);
        return true;
    }

    private void GenerateDeserializeCall(
        CompilationUnit compilation, 
        LoquiTypeSet obj,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics,
        bool generateLoop)
    {
        foreach (var interceptor in _serializationInterceptors)
        {
            if (interceptor.Applicable(compilation, obj))
            {
                interceptor.GenerateDeserialize(
                    compilation,
                    obj,
                    sb,
                    baseType,
                    properties,
                    generics);
                return;
            }
        }

        if (generateLoop)
        {
            sb.AppendLine($"while (kernel.TryGetNextField(reader, out var name))");
            using (sb.CurlyBrace())
            {
                using (var c = sb.Call("await DeserializeSingleFieldInto"))
                {
                    c.AddPassArg("reader");
                    c.AddPassArg("kernel");
                    c.AddPassArg("obj");
                    c.AddPassArg("metaData");
                    c.AddPassArg("name");
                }
            }            
            sb.AppendLine();
        }
        
        foreach (var prop in properties.InOrder)
        {
            compilation.Context.CancellationToken.ThrowIfCancellationRequested();
            _customizationDriver.WrapOmission(compilation, sb, prop, () =>
            {
                _forFieldGenerator.GenerateForDeserializeSection(
                    compilation: compilation,
                    obj: obj,
                    fieldType: prop.Property.Type,
                    readerAccessor: "reader",
                    fieldName: prop.Property.Name,
                    fieldAccessor: $"obj.{prop.Property.Name}",
                    canSet: prop.Property.IsSettable(),
                    prop.Generator,
                    sb: sb,
                    cancel: compilation.Context.CancellationToken);
            });
        }
    }

    private void GenerateSerialize(
        CompilationUnit compilation,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics)
    {
        foreach (var interceptor in _serializationInterceptors)
        {
            if (interceptor.Applicable(compilation, typeSet))
            {
                interceptor.GenerateSerialize(
                    compilation,
                    typeSet,
                    sb,
                    baseType,
                    properties,
                    generics);
                return;
            }
        }
        
        var genString = generics.WriterGenericsString(forHas: false);
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Setter);
        using (var args = sb.Function($"public static async Task Serialize{genString}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{typeSet.SerializeSymbol} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            if (isMod)
            {
                args.Add("IWorkDropoff? workDropoff");
                args.Add("IFileSystem? fileSystem");
                args.Add($"ICreateStream? streamCreator");
            }
            else
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.WriterWheres(forHas: false));
            args.Wheres.Add("where TWriteObject : IContainStreamPackage");
        }
        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                sb.AppendLine("var metaData = new SerializationMetaData(item.GameRelease, workDropoff, fileSystem, streamCreator);");
            }

            using (var args = sb.Call($"await SerializeFields{genString}"))
            {
                args.AddPassArg($"writer");
                args.AddPassArg($"item");
                args.AddPassArg($"kernel");
                args.AddPassArg($"metaData");
            }
        }
        sb.AppendLine();
    }

    private void GenerateSerializeFields(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics)
    {
        var genString = generics.WriterGenericsString(forHas: false);
        using (var args = sb.Function($"public static async Task SerializeFields{genString}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{obj.SerializeSymbol} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            args.Add("SerializationMetaData metaData");
            args.Wheres.AddRange(generics.WriterWheres(forHas: false));
            args.Wheres.Add("where TWriteObject : IContainStreamPackage");
        }
        
        var isMod = _modObjectTypeTester.IsModObject(obj.Setter);

        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                sb.AppendLine($"kernel.WriteEnum<GameRelease>(writer, \"GameRelease\", item.GameRelease, default, checkDefaults: false);");
            }
            
            if (baseType != null
                && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
            {
                sb.AppendLine(
                    $"await {baseSerializationItems.SerializationCall()}{genString}(writer, item, kernel, metaData);");
            }

            _customizationDriver.SerializationPreWork(obj, compilation, sb, properties);
            
            foreach (var prop in properties.InOrder)
            {
                compilation.Context.CancellationToken.ThrowIfCancellationRequested();
                _customizationDriver.WrapOmission(compilation, sb, prop, () =>
                {
                    _forFieldGenerator.GenerateSerializeForField(
                        compilation: compilation,
                        obj: obj,
                        fieldType: prop.Property.Type,
                        writerAccessor: "writer",
                        fieldName: prop.Property.Name,
                        fieldAccessor: $"item.{prop.Property.Name}",
                        defaultValueAccessor: prop.DefaultString,
                        prop.Generator,
                        sb: sb,
                        cancel: compilation.Context.CancellationToken);
                });
            }

            _customizationDriver.SerializationPostWork(obj, compilation, sb, properties);
        }
        sb.AppendLine();
    }

    private void GenerateHasSerializationItems(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics)
    {
        var isMod = _modObjectTypeTester.IsModObject(obj.Getter);
        using (var args = sb.Function($"public static bool HasSerializationItems{generics.WriterGenericsString(forHas: true)}"))
        {
            args.Add($"{obj.SerializeSymbol}? item");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.WriterWheres(forHas: true));
        }

        using (sb.CurlyBrace())
        {
            sb.AppendLine("if (item == null) return false;");
            if (isMod)
            {
                GenerateMetaConstruction(sb, "item", "null!", "null!", "null!");
            }
            if (baseType != null
                && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
            {
                sb.AppendLine(
                    $"if ({baseSerializationItems.HasSerializationCall()}{generics.WriterGenericsString(forHas: true)}(item, metaData)) return true;");
            }

            var hasInvariable = properties.InOrder.Any(x =>
            {
                var gen = _forFieldGenerator.GetGenerator(obj, compilation, x.Property.Type, x.Property.Name);
                return !gen?.HasVariableHasSerialize ?? true;
            });

            if (hasInvariable)
            {
                sb.AppendLine("return true;");
            }
            else
            {
                foreach (var prop in properties.InOrder)
                {
                    compilation.Context.CancellationToken.ThrowIfCancellationRequested();
                    _customizationDriver.WrapOmission(compilation, sb, prop, () =>
                    {
                        _forFieldGenerator.GenerateHasSerializeForField(
                            compilation: compilation,
                            obj: obj, 
                            fieldType: prop.Property.Type,
                            fieldName: prop.Property.Name, 
                            fieldAccessor: $"item.{prop.Property.Name}", 
                            defaultValueAccessor: prop.DefaultString,
                            prop.Generator,
                            sb: sb, 
                            cancel: compilation.Context.CancellationToken);
                    });
                }
            
                sb.AppendLine("return false;");
            }
        }
        sb.AppendLine();
    }

    private void GenerateHasSerializationItemsWithCheck(
        CompilationUnit compilation,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        SerializationGenerics generics,
        IReadOnlyCollection<LoquiTypeSet> inheriting)
    {
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Setter);
        using (var args = sb.Function($"public static bool HasSerializationItemsWithCheck{generics.WriterGenericsString(forHas: true)}"))
        {
            args.Add($"{typeSet.Getter}? item");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.WriterWheres(forHas: true));
        }

        using (sb.CurlyBrace())
        {
            // Need to encode inheriting type, so no short circuiting
            sb.AppendLine("return true;");
            
            // if (isMod)
            // {
            //     GenerateMetaConstruction(sb, "item");
            // }
            //
            // sb.AppendLine("switch (item)");
            // using (sb.CurlyBrace())
            // {
            //     foreach (var inherit in inheriting
            //                  .Select(x => x.Getter)
            //                  .OrderBy(x => x.Name))
            //     {
            //         context.CancellationToken.ThrowIfCancellationRequested();
            //         var names = _nameRetriever.GetNames(inherit);
            //         if (!_loquiSerializationNaming.TryGetSerializationItems(inherit, out var inheritSerializeItems))
            //             continue;
            //         if (!compilation.Mapping.TryGetTypeSet(inherit, out var inheritTypes)) continue;
            //         if (inheritTypes.Direct?.IsAbstract ?? true) continue;
            //         sb.AppendLine($"case {inherit.ContainingNamespace}.{names.Getter} {names.Direct}Getter:");
            //         using (sb.IncreaseDepth())
            //         {
            //             sb.AppendLine(
            //                 $"return {inheritSerializeItems.HasSerializationCall()}({names.Direct}Getter, metaData);");
            //         }
            //     }
            //
            //     if (_loquiSerializationNaming.TryGetSerializationItems(typeSet.Getter, out var curSerializationItems)
            //         && (!typeSet.Direct?.IsAbstract ?? false))
            //     {
            //         sb.AppendLine($"case {typeSet.Getter} {typeSet.Getter.Name}:");
            //         using (sb.IncreaseDepth())
            //         {
            //             sb.AppendLine(
            //                 $"return {curSerializationItems.HasSerializationCall()}({typeSet.Getter.Name}, metaData);");
            //         }
            //     }
            //
            //     sb.AppendLine("default:");
            //     using (sb.IncreaseDepth())
            //     {
            //         sb.AppendLine($"throw new NotImplementedException();");
            //     }
            // }
        }
        sb.AppendLine();
    }

    private void GenerateSerializeWithCheck(
        CompilationUnit compilation,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb, 
        SerializationGenerics generics,
        IReadOnlyCollection<LoquiTypeSet> inheriting)
    {
        if (typeSet.Getter == null) return;
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        using (var args = sb.Function($"public static async Task SerializeWithCheck{generics.WriterGenericsString(forHas: false)}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{typeSet.Getter} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.WriterWheres(forHas: false));
            args.Wheres.Add("where TWriteObject : IContainStreamPackage");
        }
        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                GenerateMetaConstruction(sb, "item", "null!", "null!", "null!");
            }
            sb.AppendLine($"kernel.WriteType(writer, LoquiRegistration.StaticRegister.GetRegister(item.GetType()).ClassType);");
            sb.AppendLine("switch (item)");
            using (sb.CurlyBrace())
            {
                foreach (var inherit in inheriting
                             .Select(x => x.Getter)
                             .NotNull()
                             .OrderBy(x => x.Name))
                {
                    compilation.Context.CancellationToken.ThrowIfCancellationRequested();
                    var names = _nameRetriever.GetNames(inherit);
                    if (!_loquiSerializationNaming.TryGetSerializationItems(inherit, out var inheritSerializeItems))
                        continue;
                    if (!compilation.Mapping.TryGetTypeSet(inherit, out var inheritTypes)) continue;
                    if (inheritTypes.Direct?.IsAbstract ?? true) continue;
                    sb.AppendLine($"case {inherit.ContainingNamespace}.{names.Getter} {names.Direct}Getter:");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine(
                            $"await {inheritSerializeItems.SerializationCall()}(writer, {names.Direct}Getter, kernel, metaData);");
                        sb.AppendLine("break;");
                    }
                }
                
                if (_loquiSerializationNaming.TryGetSerializationItems(typeSet.Getter, out var curSerializationItems)
                    && (!typeSet.Direct?.IsAbstract ?? false))
                {
                    sb.AppendLine($"case {typeSet.Getter} {typeSet.Getter.Name}:");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine(
                            $"await {curSerializationItems.SerializationCall()}(writer, {typeSet.Getter.Name}, kernel, metaData);");
                        sb.AppendLine("break;");
                    }
                }

                sb.AppendLine("default:");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine($"throw new NotImplementedException();");
                }
            }
        }

        sb.AppendLine();
    }

    private void GenerateDeserializeWithCheck(
        CompilationUnit compilation,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb, 
        SerializationGenerics generics,
        IReadOnlyCollection<LoquiTypeSet> inheriting)
    {
        if (typeSet.Getter == null) return;
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        using (var args = sb.Function($"public static async Task<{typeSet.Direct ?? typeSet.Setter}> DeserializeWithCheck{generics.ReaderGenericsString()}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.Add("where TReadObject : IContainStreamPackage");
            args.Wheres.AddRange(generics.ReaderWheres());
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"var type = kernel.GetNextType(reader, \"{typeSet.GetAny().ContainingNamespace}\");");
            sb.AppendLine($"switch (type.Name)");
            using (sb.CurlyBrace())
            {
                foreach (var inherit in inheriting
                             .Select(x => x.Getter)
                             .NotNull()
                             .OrderBy(x => x.Name))
                {
                    compilation.Context.CancellationToken.ThrowIfCancellationRequested();
                    var names = _nameRetriever.GetNames(inherit);
                    if (!_loquiSerializationNaming.TryGetSerializationItems(inherit, out var inheritSerializeItems))
                        continue;
                    if (!compilation.Mapping.TryGetTypeSet(inherit, out var inheritTypes)) continue;
                    if (inheritTypes.Direct?.IsAbstract ?? true) continue;
                    sb.AppendLine($"case \"{names.Direct}\":");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine($"return await {inheritSerializeItems.DeserializationCall()}(reader, kernel, metaData);");
                    }
                }

                if (_loquiSerializationNaming.TryGetSerializationItems(typeSet.Getter, out var curSerializationItems)
                    && (!typeSet.Direct?.IsAbstract ?? false))
                {
                    sb.AppendLine($"case \"{typeSet.Direct?.Name}\":");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine($"return await {curSerializationItems.DeserializationCall()}(reader, kernel, metaData);");
                    }
                }

                sb.AppendLine("default:");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine($"throw new NotImplementedException();");
                }
            }
        }

        sb.AppendLine();
    }

    private static void GenerateUsings(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        StructuredStringBuilder sb,
        PropertyCollection propertyDict)
    {
         sb.AppendLines(propertyDict.InOrder
            .SelectMany(x =>
                x.Generator?.RequiredNamespaces(obj, compilation, x.Property.Type) ?? Enumerable.Empty<string>())
            .And("Mutagen.Bethesda.Plugins")
            .And("Mutagen.Bethesda.Serialization")
            .And("Mutagen.Bethesda.Serialization.Streams")
            .And("Mutagen.Bethesda.Serialization.Utility")
            .And("System.Threading.Tasks")
            .And("System.IO.Abstractions")
            .And("Noggog.WorkEngine")
            .And("Loqui")
            .And("Noggog")
            .And(obj.GetAny().ContainingNamespace.ToString())
            .Distinct()
            .OrderBy(x => x)
            .Select(x => $"using {x};"));
        sb.AppendLine();
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
    }

    private SerializationGenerics GetGenerics(LoquiTypeSet typeSet)
    {
        SerializationGenerics? ret = null;
        
        if (typeSet.Getter is INamedTypeSymbol writerTypeSymbol
            && writerTypeSymbol.TypeArguments.Length > 0)
        {
            ret ??= new();
            ret.ExtraWriterGenerics = new();
            ret.ExtraWriterGenerics.AddRange(writerTypeSymbol.TypeArguments.Select(x => x.ToString()));
            ret.ExtraWriterWheres = new();
            ret.ExtraWriterWheres.AddRange(_whereClauseGenerator.GetWheres(writerTypeSymbol));
        }

        if (typeSet.Setter is INamedTypeSymbol readerTypeSymbol
            && readerTypeSymbol.TypeArguments.Length > 0)
        {
            ret ??= new();
            ret.ExtraReaderGenerics = new();
            ret.ExtraReaderGenerics.AddRange(readerTypeSymbol.TypeArguments.Select(x => x.ToString()));
            ret.ExtraReaderWheres = new();
            ret.ExtraReaderWheres.AddRange(_whereClauseGenerator.GetWheres(readerTypeSymbol));
        }

        return ret ?? SerializationGenerics.Instance;
    }

    private void GenerateMetaConstruction(StructuredStringBuilder sb, string accessor, string workDropoffAccessor, string fileSystemAccessor, string streamCreatorAccessor)
    {
        sb.AppendLine($"var metaData = new SerializationMetaData({accessor}.GameRelease, {workDropoffAccessor}, {fileSystemAccessor}, {streamCreatorAccessor});");
    }
}