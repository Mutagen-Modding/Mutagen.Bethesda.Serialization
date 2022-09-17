using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

record PropertyMetadata(IPropertySymbol Property, ISerializationForFieldGenerator? Generator)
{
    public IFieldSymbol? Default { get; set; }
}

public class SerializationForObjectGenerator
{
    private readonly PropertyFilter _propertyFilter;
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly SerializationFieldGenerator _forFieldGenerator;
    private readonly WhereClauseGenerator _whereClauseGenerator;
    private readonly LoquiSerializationNaming _loquiSerializationNaming;

    public SerializationForObjectGenerator(
        PropertyFilter propertyFilter,
        LoquiNameRetriever nameRetriever,
        SerializationFieldGenerator forFieldGenerator,
        WhereClauseGenerator whereClauseGenerator,
        LoquiSerializationNaming loquiSerializationNaming)
    {
        _propertyFilter = propertyFilter;
        _nameRetriever = nameRetriever;
        _forFieldGenerator = forFieldGenerator;
        _whereClauseGenerator = whereClauseGenerator;
        _loquiSerializationNaming = loquiSerializationNaming;
    }
    
    public void Generate(
        CompilationUnit compilation, 
        SourceProductionContext context, 
        ITypeSymbol obj)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        
        if (!compilation.Mapping.TryGetTypeSet(obj, out var typeSet)) return;
        var baseType = compilation.Mapping.TryGetBaseClass(obj);
        var inheriting = compilation.Mapping.TryGetInheritingClasses(typeSet.Getter);
        
        var sb = new StructuredStringBuilder();
        
        var propertyDict = GetPropertyDictionary(context, obj);
        
        GenerateUsings(context, obj, sb, propertyDict);

        using (sb.Namespace(obj.ContainingNamespace.ToString()))
        {
        }
        
        if (!_loquiSerializationNaming.TryGetSerializationItems(obj, out var objSerializationItems)) return;
        
        GetGenerics(typeSet, out var readerWheres, out var writerWheres, out var writeObjectGenericsString, out var readObjectGenericsString);

        using (var c = sb.Class(objSerializationItems.SerializationHousingClassName))
        {
            c.AccessModifier = AccessModifier.Internal;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            if (inheriting.Count > 0)
            {
                GenerateSerializeWithCheck(compilation, context, obj, sb, writeObjectGenericsString, typeSet, writerWheres, inheriting);
            }
            
            GenerateSerialize(compilation, context, obj, sb, writeObjectGenericsString, typeSet, writerWheres, baseType, propertyDict);
            
            GenerateDeserialize(sb, typeSet, readObjectGenericsString, readerWheres);
        }
        sb.AppendLine();
        
        context.AddSource(objSerializationItems.SerializationHousingFileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static void GenerateDeserialize(StructuredStringBuilder sb, LoquiTypeSet typeSet,
        string readObjectGenericsString, List<string> readerWheres)
    {
        using (var args = sb.Function($"public static {typeSet.Setter} Deserialize{readObjectGenericsString}"))
        {
            args.Add($"TReadObject reader");
            args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            args.Wheres.AddRange(readerWheres);
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
        string writeObjectGenericsString,
        LoquiTypeSet typeSet,
        List<string> writerWheres,
        ITypeSymbol? baseType,
        Dictionary<string, PropertyMetadata> propertyDict)
    {
        using (var args = sb.Function($"public static void Serialize{writeObjectGenericsString}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{typeSet.Getter} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            args.Wheres.AddRange(writerWheres);
        }

        using (sb.CurlyBrace())
        {
            if (baseType != null
                && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
            {
                sb.AppendLine(
                    $"{baseSerializationItems.SerializationCall(serialize: true)}<TWriteObject>(item, writer, kernel);");
            }

            foreach (var prop in obj.GetMembers().WhereCastable<ISymbol, IPropertySymbol>())
            {
                if (!propertyDict.TryGetValue(prop.Name, out var propEntry)) continue;
                GenerateForProperty(compilation, obj, propEntry, sb, context.CancellationToken);
            }
        }
        sb.AppendLine();
    }

    private void GenerateSerializeWithCheck(
        CompilationUnit compilation,
        SourceProductionContext context, 
        ITypeSymbol obj,
        StructuredStringBuilder sb, 
        string writeObjectGenericsString,
        LoquiTypeSet typeSet,
        List<string> writerWheres,
        IReadOnlyList<ITypeSymbol> inheriting)
    {
        using (var args = sb.Function($"public static void SerializeWithCheck{writeObjectGenericsString}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{typeSet.Getter} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            args.Wheres.AddRange(writerWheres);
        }

        using (sb.CurlyBrace())
        {
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
                            $"{inheritSerializeItems.SerializationCall(serialize: true)}(writer, {names.Direct}Getter, kernel);");
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
                            $"{curSerializationItems.SerializationCall(serialize: true)}(writer, {typeSet.Getter.Name}, kernel);");
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

    private Dictionary<string, PropertyMetadata> GetPropertyDictionary(SourceProductionContext context, ITypeSymbol obj)
    {
        var propertyDict = obj.GetMembers().WhereCastable<ISymbol, IPropertySymbol>()
            .Where(x => !_propertyFilter.Skip(x))
            .Select(x =>
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                return x;
            })
            .ToDictionary(x => x.Name, x =>
            {
                var gen = _forFieldGenerator.GetGenerator(x.Type, context.CancellationToken);
                return new PropertyMetadata(x, gen);
            });

        foreach (var field in obj.GetMembers().WhereCastable<ISymbol, IFieldSymbol>())
        {
            if (!field.IsStatic || !field.IsReadOnly) continue;
            if (!field.Name.EndsWith("Default")) continue;
            if (propertyDict.TryGetValue(field.Name.TrimEnd("Default"), out var prop))
            {
                prop.Default = field;
            }
        }
        return propertyDict;
    }

    private static void GenerateUsings(SourceProductionContext context, ITypeSymbol obj, StructuredStringBuilder sb,
        Dictionary<string, PropertyMetadata> propertyDict)
    {
        sb.AppendLines(propertyDict.Values
            .SelectMany(x =>
                x.Generator?.RequiredNamespaces(x.Property.Type, context.CancellationToken) ?? Enumerable.Empty<string>())
            .And("Mutagen.Bethesda.Serialization")
            .And(obj.ContainingNamespace.ToString())
            .Distinct()
            .OrderBy(x => x)
            .Select(x => $"using {x};"));
        sb.AppendLine();
    }

    private void GetGenerics(LoquiTypeSet typeSet,
        out List<string> readerWheres,
        out List<string> writerWheres,
        out string writeObjectGenericsString,
        out string readObjectGenericsString)
    {
        List<string> writeObjectGenerics = new() { "TKernel", "TWriteObject" };
        List<string> readObjectGenerics = new() { "TReadObject" };
        readerWheres = new();
        writerWheres = new();
        if (typeSet.Getter is INamedTypeSymbol writerTypeSymbol
            && writerTypeSymbol.TypeArguments.Length > 0)
        {
            writeObjectGenerics.AddRange(writerTypeSymbol.TypeArguments.Select(x => x.ToString()));
            writerWheres.AddRange(_whereClauseGenerator.GetWheres(writerTypeSymbol));
        }

        if (typeSet.Setter is INamedTypeSymbol readerTypeSymbol
            && readerTypeSymbol.TypeArguments.Length > 0)
        {
            readObjectGenerics.AddRange(readerTypeSymbol.TypeArguments.Select(x => x.ToString()));
            readerWheres.AddRange(_whereClauseGenerator.GetWheres(readerTypeSymbol));
        }

        writerWheres.Add("where TKernel : ISerializationWriterKernel<TWriteObject>, new()");
        writeObjectGenericsString = $"<{string.Join(", ", writeObjectGenerics)}>";
        readObjectGenericsString = $"<{string.Join(", ", readObjectGenerics)}>";
    }

    private void GenerateForProperty(
        CompilationUnit compilation,
        ITypeSymbol obj, 
        PropertyMetadata prop,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var def = prop.Default == null ? null : $"{prop.Default.ContainingSymbol.ContainingNamespace}.{prop.Default.ContainingSymbol.Name}.{prop.Default.Name}";
        _forFieldGenerator.GenerateForField(
            compilation: compilation,
            obj: obj, 
            fieldType: prop.Property.Type,
            writerAccessor: "writer", 
            fieldName: prop.Property.Name, 
            fieldAccessor: $"item.{prop.Property.Name}", 
            defaultValueAccessor: def,
            prop.Generator,
            sb: sb, 
            cancel: cancel);
    }
}