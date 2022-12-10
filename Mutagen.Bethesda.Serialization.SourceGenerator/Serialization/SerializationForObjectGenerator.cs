using System.Collections.Immutable;
using System.Text;
using Loqui;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
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

    public SerializationForObjectGenerator(
        LoquiNameRetriever nameRetriever,
        SerializationFieldGenerator forFieldGenerator,
        WhereClauseGenerator whereClauseGenerator,
        LoquiSerializationNaming loquiSerializationNaming,
        PropertyCollectionRetriever propertyCollectionRetriever, 
        CustomizationDriver customizationDriver,
        ObjectTypeTester modObjectTypeTester,
        ReleaseRetriever releaseRetriever)
    {
        _nameRetriever = nameRetriever;
        _forFieldGenerator = forFieldGenerator;
        _whereClauseGenerator = whereClauseGenerator;
        _loquiSerializationNaming = loquiSerializationNaming;
        _propertyCollectionRetriever = propertyCollectionRetriever;
        _customizationDriver = customizationDriver;
        _modObjectTypeTester = modObjectTypeTester;
        _releaseRetriever = releaseRetriever;
    }
    
    public void Generate(
        CompilationUnit compilation, 
        CustomizationCatalog? customizationDriver,
        SourceProductionContext context,
        LoquiTypeSet typeSet)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        
        var baseType = compilation.Mapping.TryGetBaseClass(typeSet.Direct);
        var inheriting = compilation.Mapping.TryGetInheritingClasses(typeSet.Getter);
        
        var sb = new StructuredStringBuilder();
        
        var properties = _propertyCollectionRetriever.GetPropertyCollection(
            context,
            typeSet);
        
        GenerateUsings(context, typeSet.Setter, sb, properties);

        using (sb.Namespace(typeSet.Setter.ContainingNamespace.ToString()))
        {
        }
        
        if (!_loquiSerializationNaming.TryGetSerializationItems(typeSet.Setter, out var objSerializationItems)) return;
        
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
                GenerateSerializeWithCheck(compilation, context, typeSet, sb, gens, inheriting);
            }
            
            GenerateSerialize(compilation, context, typeSet, sb, baseType, properties, customizationDriver, gens);
            
            if (inheriting.Count > 0)
            {
                GenerateHasSerializationItemsWithCheck(compilation, context, typeSet, sb, gens, inheriting);
            }
            
            GenerateHasSerializationItems(compilation, context, typeSet, sb, baseType, properties, customizationDriver, gens);
            
            if (inheriting.Count > 0)
            {
                GenerateDeserializeWithCheck(compilation, context, typeSet, sb, gens, inheriting);
            }

            if (isConcrete && !isGroup)
            {
                GenerateDeserialize(typeSet, sb, gens);
            }
            
            GenerateDeserializeInto(compilation, context, typeSet, sb, baseType, properties, customizationDriver, gens);
        }
        sb.AppendLine();
        
        context.AddSource(objSerializationItems.SerializationHousingFileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private void GenerateDeserialize(
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        SerializationGenerics generics)
    {
        if (typeSet.Direct == null) return;
        
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        var isMajorRecord = _modObjectTypeTester.IsMajorRecordObject(typeSet.Getter);
        var isModHeader = _modObjectTypeTester.IsModHeader(typeSet.Getter);
        
        var genString = generics.ReaderGenericsString();
        using (var args = sb.Function($"public static {typeSet.Direct} Deserialize{genString}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            if (isMod)
            {
                args.Add($"ModKey modKey");
                args.Add($"{_releaseRetriever.GetReleaseName(typeSet.Getter)}Release release");
            }
            else
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.ReaderWheres());
        }
        using (sb.CurlyBrace())
        {
            if (isMajorRecord)
            {
                sb.AppendLine($"var obj = new {typeSet.Direct}(kernel.ExtractFormKey(reader), metaData.Release.To{_releaseRetriever.GetReleaseName(typeSet.Getter)}Release());");
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
            using (var c = sb.Call($"DeserializeInto{genString}"))
            {
                c.AddPassArg("reader");
                c.AddPassArg("kernel");
                c.AddPassArg("obj");
                if (!isMod)
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
        SourceProductionContext context,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        CustomizationCatalog? customizationCatalog,
        SerializationGenerics generics)
    {
        var genString = generics.ReaderGenericsString();
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        using (var args = sb.Function($"public static void DeserializeInto{genString}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            args.Add($"{typeSet.Setter} obj");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.ReaderWheres());
        }

        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                GenerateMetaConstruction(sb, "obj");
            }

            sb.AppendLine($"while (kernel.TryGetNextField(reader, out var name))");
            using (sb.CurlyBrace())
            {
                using (var c = sb.Call("DeserializeSingleFieldInto"))
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
        sb.AppendLine();
        
        using (var args = sb.Function($"public static void DeserializeSingleFieldInto{genString}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            args.Add($"{typeSet.Setter} obj");
            args.Add("SerializationMetaData metaData");
            args.Add("string name");
            args.Wheres.AddRange(generics.ReaderWheres());
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("switch (name)");
            using (sb.CurlyBrace())
            {
                foreach (var prop in properties.InOrder)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    _customizationDriver.WrapOmission(customizationCatalog, sb, prop, () =>
                    {
                        sb.AppendLine($"case \"{prop.Property.Name}\":");
                        using (sb.IncreaseDepth())
                        {
                            _forFieldGenerator.GenerateDeserializeForField(
                                compilation: compilation,
                                obj: typeSet,
                                fieldType: prop.Property.Type,
                                readerAccessor: "reader",
                                fieldName: prop.Property.Name,
                                fieldAccessor: $"obj.{prop.Property.Name}",
                                canSet: prop.Property.IsSettable(),
                                prop.Generator,
                                sb: sb,
                                cancel: context.CancellationToken);
                            sb.AppendLine("break;");
                        }
                    });
                }
                sb.AppendLine("default:");
                using (sb.IncreaseDepth())
                {
            
                    if (baseType != null
                        && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
                    {
                        sb.AppendLine(
                            $"{baseSerializationItems.DeserializationSingleFieldIntoCall()}{genString}(reader, kernel, obj, metaData, name);");
                    }
                    sb.AppendLine("break;");
                }
            }
        }
        sb.AppendLine();
    }

    private void GenerateSerialize(
        CompilationUnit compilation,
        SourceProductionContext context,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        CustomizationCatalog? customizationCatalog,
        SerializationGenerics generics)
    {
        var genString = generics.WriterGenericsString(forHas: false);
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Setter);
        using (var args = sb.Function($"public static void Serialize{genString}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{typeSet.Getter} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.WriterWheres(forHas: false));
        }

        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                sb.AppendLine("var metaData = new SerializationMetaData(item.GameRelease);");
            }
            
            if (baseType != null
                && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
            {
                sb.AppendLine(
                    $"{baseSerializationItems.SerializationCall()}{genString}(writer, item, kernel, metaData);");
            }

            foreach (var prop in properties.InOrder)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                _customizationDriver.WrapOmission(customizationCatalog, sb, prop, () =>
                {
                    _forFieldGenerator.GenerateSerializeForField(
                        compilation: compilation,
                        obj: typeSet,
                        fieldType: prop.Property.Type,
                        writerAccessor: "writer",
                        fieldName: prop.Property.Name,
                        fieldAccessor: $"item.{prop.Property.Name}",
                        defaultValueAccessor: prop.DefaultString,
                        prop.Generator,
                        sb: sb,
                        cancel: context.CancellationToken);
                });
            }
        }
        sb.AppendLine();
    }

    private void GenerateHasSerializationItems(CompilationUnit compilation,
        SourceProductionContext context,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        CustomizationCatalog? customizationCatalog,
        SerializationGenerics generics)
    {
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        using (var args = sb.Function($"public static bool HasSerializationItems{generics.WriterGenericsString(forHas: true)}"))
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
            sb.AppendLine("if (item == null) return false;");
            if (isMod)
            {
                GenerateMetaConstruction(sb, "item");
            }
            if (baseType != null
                && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
            {
                sb.AppendLine(
                    $"if ({baseSerializationItems.HasSerializationCall()}{generics.WriterGenericsString(forHas: true)}(item, metaData)) return true;");
            }

            var hasInvariable = properties.InOrder.Any(x =>
            {
                var gen = _forFieldGenerator.GetGenerator(x.Property.Type, context.CancellationToken);
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
                    context.CancellationToken.ThrowIfCancellationRequested();
                    _customizationDriver.WrapOmission(customizationCatalog, sb, prop, () =>
                    {
                        _forFieldGenerator.GenerateHasSerializeForField(
                            compilation: compilation,
                            obj: typeSet, 
                            fieldType: prop.Property.Type,
                            fieldName: prop.Property.Name, 
                            fieldAccessor: $"item.{prop.Property.Name}", 
                            defaultValueAccessor: prop.DefaultString,
                            prop.Generator,
                            sb: sb, 
                            cancel: context.CancellationToken);
                    });
                }
            
                sb.AppendLine("return false;");
            }
        }
        sb.AppendLine();
    }

    private void GenerateHasSerializationItemsWithCheck(CompilationUnit compilation,
        SourceProductionContext context,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        SerializationGenerics generics,
        IReadOnlyCollection<LoquiTypeSet> inheriting)
    {
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Setter);
        using (var args = sb.Function($"public static bool HasSerializationItemsWithCheck{generics.WriterGenericsString(forHas: true)}"))
        {
            args.Add($"{typeSet.Getter} item");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.WriterWheres(forHas: true));
        }

        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                GenerateMetaConstruction(sb, "item");
            }
            
            sb.AppendLine("switch (item)");
            using (sb.CurlyBrace())
            {
                foreach (var inherit in inheriting
                             .Select(x => x.Getter)
                             .OrderBy(x => x.Name))
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var names = _nameRetriever.GetNames(inherit);
                    if (!_loquiSerializationNaming.TryGetSerializationItems(inherit, out var inheritSerializeItems))
                        continue;
                    if (!compilation.Mapping.TryGetTypeSet(inherit, out var inheritTypes)) continue;
                    if (inheritTypes.Direct?.IsAbstract ?? true) continue;
                    sb.AppendLine($"case {inherit.ContainingNamespace}.{names.Getter} {names.Direct}Getter:");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine(
                            $"return {inheritSerializeItems.HasSerializationCall()}({names.Direct}Getter, metaData);");
                    }
                }

                if (_loquiSerializationNaming.TryGetSerializationItems(typeSet.Getter, out var curSerializationItems)
                    && (!typeSet.Direct?.IsAbstract ?? false))
                {
                    sb.AppendLine($"case {typeSet.Getter} {typeSet.Getter.Name}:");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine(
                            $"return {curSerializationItems.HasSerializationCall()}({typeSet.Getter.Name}, metaData);");
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

    private void GenerateSerializeWithCheck(
        CompilationUnit compilation,
        SourceProductionContext context, 
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb, 
        SerializationGenerics generics,
        IReadOnlyCollection<LoquiTypeSet> inheriting)
    {
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        using (var args = sb.Function($"public static void SerializeWithCheck{generics.WriterGenericsString(forHas: false)}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{typeSet.Getter} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.WriterWheres(forHas: false));
        }

        using (sb.CurlyBrace())
        {
            if (isMod)
            {
                GenerateMetaConstruction(sb, "item");
            }
            sb.AppendLine($"kernel.WriteType(writer, LoquiRegistration.StaticRegister.GetRegister(item.GetType()).ClassType);");
            sb.AppendLine("switch (item)");
            using (sb.CurlyBrace())
            {
                GenerateSerializationCases(compilation, context, typeSet, sb, inheriting);

                if (_loquiSerializationNaming.TryGetSerializationItems(typeSet.Getter, out var curSerializationItems)
                    && (!typeSet.Direct?.IsAbstract ?? false))
                {
                    sb.AppendLine($"case {typeSet.Getter} {typeSet.Getter.Name}:");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine(
                            $"{curSerializationItems.SerializationCall()}(writer, {typeSet.Getter.Name}, kernel, metaData);");
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

    private void GenerateSerializationCases(CompilationUnit compilation, SourceProductionContext context,
        LoquiTypeSet typeSet, StructuredStringBuilder sb, IReadOnlyCollection<LoquiTypeSet> inheriting,
         HashSet<LoquiTypeSet>? passed = null)
    {
        passed ??= new();
        foreach (var inherit in inheriting
                     .OrderBy(x => x.Getter.Name))
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var names = _nameRetriever.GetNames(inherit.Getter);
            if (!compilation.Mapping.TryGetTypeSet(inherit.Getter, out var inheritTypes)) continue;
            if (inheritTypes.Direct == null) continue;
            if (inheritTypes.Direct.IsAbstract)
            {
                var inheritingFromAbstract = compilation.Mapping.TryGetInheritingClasses(inheritTypes.Getter);
                GenerateSerializationCases(compilation, context, inheritTypes, sb, inheritingFromAbstract.Where(x => !x.Equals(inherit)).ToArray(), passed);
            }
            else
            {
                GenerateSerializationCallCase(sb, inherit, names, passed);
            }
        }
    }

    private void GenerateSerializationCallCase(StructuredStringBuilder sb, LoquiTypeSet inherit, Names names,
        HashSet<LoquiTypeSet> passed)
    {
        if (!passed.Add(inherit)) return;
        var getter = inherit.Getter;
        if (!_loquiSerializationNaming.TryGetSerializationItems(getter, out var inheritSerializeItems))
            return;
        sb.AppendLine($"case {getter.ContainingNamespace}.{names.Getter} {names.Direct}Getter:");
        using (sb.IncreaseDepth())
        {
            sb.AppendLine(
                $"{inheritSerializeItems.SerializationCall()}(writer, {names.Direct}Getter, kernel, metaData);");
            sb.AppendLine("break;");
        }
    }

    private void GenerateDeserializeWithCheck(
        CompilationUnit compilation,
        SourceProductionContext context, 
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb, 
        SerializationGenerics generics,
        IReadOnlyCollection<LoquiTypeSet> inheriting)
    {
        var isMod = _modObjectTypeTester.IsModObject(typeSet.Getter);
        using (var args = sb.Function($"public static {typeSet.Direct ?? typeSet.Setter} DeserializeWithCheck{generics.ReaderGenericsString()}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            if (!isMod)
            {
                args.Add("SerializationMetaData metaData");
            }
            args.Wheres.AddRange(generics.ReaderWheres());
        }

        using (sb.CurlyBrace())
        {
            sb.AppendLine($"switch (kernel.GetNextType(reader, \"{typeSet.Getter.ContainingNamespace}\").Name)");
            using (sb.CurlyBrace())
            {
                foreach (var inherit in inheriting
                             .Select(x => x.Getter)
                             .OrderBy(x => x.Name))
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var names = _nameRetriever.GetNames(inherit);
                    if (!_loquiSerializationNaming.TryGetSerializationItems(inherit, out var inheritSerializeItems))
                        continue;
                    if (!compilation.Mapping.TryGetTypeSet(inherit, out var inheritTypes)) continue;
                    if (inheritTypes.Direct?.IsAbstract ?? true) continue;
                    sb.AppendLine($"case \"{names.Direct}\":");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine($"return {inheritSerializeItems.DeserializationCall()}(reader, kernel, metaData);");
                    }
                }

                if (_loquiSerializationNaming.TryGetSerializationItems(typeSet.Getter, out var curSerializationItems)
                    && (!typeSet.Direct?.IsAbstract ?? false))
                {
                    sb.AppendLine($"case \"{typeSet.Direct?.Name}\":");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine($"return {curSerializationItems.DeserializationCall()}(reader, kernel, metaData);");
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
        SourceProductionContext context,
        ITypeSymbol obj,
        StructuredStringBuilder sb,
        PropertyCollection propertyDict)
    {
         sb.AppendLines(propertyDict.InOrder
            .SelectMany(x =>
                x.Generator?.RequiredNamespaces(x.Property.Type, context.CancellationToken) ?? Enumerable.Empty<string>())
            .And("Mutagen.Bethesda.Plugins")
            .And("Mutagen.Bethesda.Serialization")
            .And("Loqui")
            .And(obj.ContainingNamespace.ToString())
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

    private void GenerateMetaConstruction(StructuredStringBuilder sb, string accessor)
    {
        sb.AppendLine($"var metaData = new SerializationMetaData({accessor}.GameRelease);");
    }
}