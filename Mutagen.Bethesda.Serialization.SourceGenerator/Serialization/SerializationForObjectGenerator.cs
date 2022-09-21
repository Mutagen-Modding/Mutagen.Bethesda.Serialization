using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
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
    private readonly IsModObjectTester _modObjectTester;

    public SerializationForObjectGenerator(
        LoquiNameRetriever nameRetriever,
        SerializationFieldGenerator forFieldGenerator,
        WhereClauseGenerator whereClauseGenerator,
        LoquiSerializationNaming loquiSerializationNaming,
        PropertyCollectionRetriever propertyCollectionRetriever, 
        IsModObjectTester modObjectTester)
    {
        _nameRetriever = nameRetriever;
        _forFieldGenerator = forFieldGenerator;
        _whereClauseGenerator = whereClauseGenerator;
        _loquiSerializationNaming = loquiSerializationNaming;
        _propertyCollectionRetriever = propertyCollectionRetriever;
        _modObjectTester = modObjectTester;
    }
    
    public void Generate(
        CompilationUnit compilation, 
        SourceProductionContext context, 
        ITypeSymbol obj)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        
        if (!compilation.Mapping.TryGetTypeSet(obj, out var typeSet)) return;
        var baseType = compilation.Mapping.TryGetBaseClass(typeSet.Direct);
        var inheriting = compilation.Mapping.TryGetInheritingClasses(typeSet.Getter);
        
        var sb = new StructuredStringBuilder();
        
        var properties = _propertyCollectionRetriever.GetPropertyCollection(context, typeSet);
        
        GenerateUsings(context, obj, sb, properties);

        using (sb.Namespace(obj.ContainingNamespace.ToString()))
        {
        }
        
        if (!_loquiSerializationNaming.TryGetSerializationItems(obj, out var objSerializationItems)) return;
        
        var gens = GetGenerics(typeSet);

        using (var c = sb.Class(objSerializationItems.SerializationHousingClassName))
        {
            c.AccessModifier = AccessModifier.Internal;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            if (inheriting.Count > 0)
            {
                GenerateSerializeWithCheck(compilation, context, obj, sb, typeSet, gens, inheriting);
            }
            
            GenerateSerialize(compilation, context, obj, sb, typeSet, baseType, properties, gens);
            
            if (inheriting.Count > 0)
            {
                GenerateHasSerializationItemsWithCheck(compilation, context, obj, sb, typeSet, gens, inheriting);
            }
            
            GenerateHasSerializationItems(compilation, context, obj, sb, typeSet, baseType, properties, gens);
            
            GenerateDeserialize(sb, typeSet, gens);
        }
        sb.AppendLine();
        
        context.AddSource(objSerializationItems.SerializationHousingFileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static void GenerateDeserialize(StructuredStringBuilder sb,
        LoquiTypeSet typeSet,
        SerializationGenerics generics)
    {
        using (var args = sb.Function($"public static {typeSet.Setter} Deserialize{generics.ReaderGenericsString()}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            args.Wheres.AddRange(generics.ReaderWheres());
        }

        using (sb.CurlyBrace())
        {
            sb.AppendLine("throw new NotImplementedException();");
        }

        sb.AppendLine();
    }

    private void GenerateSerialize(CompilationUnit compilation,
        SourceProductionContext context,
        ITypeSymbol obj,
        StructuredStringBuilder sb,
        LoquiTypeSet typeSet,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics)
    {
        var genString = generics.WriterGenericsString(forHas: false);
        var isMod = _modObjectTester.IsModObject(obj);
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
                    $"{baseSerializationItems.SerializationCall(serialize: true)}{genString}(writer, item, kernel, metaData);");
            }

            foreach (var prop in properties.InOrder)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                _forFieldGenerator.GenerateForField(
                    compilation: compilation,
                    obj: obj, 
                    fieldType: prop.Property.Type,
                    writerAccessor: "writer", 
                    fieldName: prop.Property.Name, 
                    fieldAccessor: $"item.{prop.Property.Name}", 
                    defaultValueAccessor: prop.DefaultString,
                    prop.Generator,
                    sb: sb, 
                    cancel: context.CancellationToken);
            }
        }
        sb.AppendLine();
    }

    private void GenerateHasSerializationItems(CompilationUnit compilation,
        SourceProductionContext context,
        ITypeSymbol obj,
        StructuredStringBuilder sb,
        LoquiTypeSet typeSet,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics)
    {
        var isMod = _modObjectTester.IsModObject(obj);
        using (var args = sb.Function($"public static bool HasSerializationItems{generics.WriterGenericsString(forHas: true)}"))
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
                GenerateMetaConstruction(sb);
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
                    _forFieldGenerator.GenerateHasForField(
                        compilation: compilation,
                        obj: obj, 
                        fieldType: prop.Property.Type,
                        fieldName: prop.Property.Name, 
                        fieldAccessor: $"item.{prop.Property.Name}", 
                        defaultValueAccessor: prop.DefaultString,
                        prop.Generator,
                        sb: sb, 
                        cancel: context.CancellationToken);
                }
            
                sb.AppendLine("return false;");
            }
        }
        sb.AppendLine();
    }

    private void GenerateHasSerializationItemsWithCheck(CompilationUnit compilation,
        SourceProductionContext context,
        ITypeSymbol obj,
        StructuredStringBuilder sb,
        LoquiTypeSet typeSet,
        SerializationGenerics generics,
        IReadOnlyList<ITypeSymbol> inheriting)
    {
        var isMod = _modObjectTester.IsModObject(obj);
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
                GenerateMetaConstruction(sb);
            }
            
            sb.AppendLine("switch (item)");
            using (sb.CurlyBrace())
            {
                foreach (var inherit in inheriting)
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

                if (_loquiSerializationNaming.TryGetSerializationItems(obj, out var curSerializationItems)
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
        ITypeSymbol obj,
        StructuredStringBuilder sb, 
        LoquiTypeSet typeSet,
        SerializationGenerics generics,
        IReadOnlyList<ITypeSymbol> inheriting)
    {
        var isMod = _modObjectTester.IsModObject(obj);
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
                GenerateMetaConstruction(sb);
            }
            sb.AppendLine("switch (item)");
            using (sb.CurlyBrace())
            {
                foreach (var inherit in inheriting)
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
                            $"{inheritSerializeItems.SerializationCall(serialize: true)}(writer, {names.Direct}Getter, kernel, metaData);");
                        sb.AppendLine("break;");
                    }
                }

                if (_loquiSerializationNaming.TryGetSerializationItems(obj, out var curSerializationItems)
                    && (!typeSet.Direct?.IsAbstract ?? false))
                {
                    sb.AppendLine($"case {typeSet.Getter} {typeSet.Getter.Name}:");
                    using (sb.IncreaseDepth())
                    {
                        sb.AppendLine(
                            $"{curSerializationItems.SerializationCall(serialize: true)}(writer, {typeSet.Getter.Name}, kernel, metaData);");
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

    private static void GenerateUsings(
        SourceProductionContext context,
        ITypeSymbol obj,
        StructuredStringBuilder sb,
        PropertyCollection propertyDict)
    {
        sb.AppendLines(propertyDict.InOrder
            .SelectMany(x =>
                x.Generator?.RequiredNamespaces(x.Property.Type, context.CancellationToken) ?? Enumerable.Empty<string>())
            .And("Mutagen.Bethesda.Serialization")
            .And(obj.ContainingNamespace.ToString())
            .Distinct()
            .OrderBy(x => x)
            .Select(x => $"using {x};"));
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

    private void GenerateMetaConstruction(StructuredStringBuilder sb)
    {
        sb.AppendLine("var metaData = new SerializationMetaData(item.GameRelease);");
    }
}