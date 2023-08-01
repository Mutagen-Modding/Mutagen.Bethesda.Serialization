using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class GenderedTypeFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();
    
    public GenderedTypeFieldGenerator(
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator)
    {
        _forFieldGenerator = forFieldGenerator;
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "GenderedItem",
        "IGenderedItem",
        "IGenderedItemGetter",
    };

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "Mutagen.Bethesda.Plugins.Records";
        var subType = GetSubtype((INamedTypeSymbol)typeSymbol);
        var gen = _forFieldGenerator().Value
            .GetGenerator(obj, compilation, subType, fieldName: null);
        if (gen != null)
        {
            foreach (var ns in gen.RequiredNamespaces(obj, compilation, subType))
            {
                yield return ns;
            }
        }
    }
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;
    
    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _listStrings.Contains(typeSymbol.Name);
    }

    private ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

    public void GenerateForSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj, 
        ITypeSymbol field, 
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor, 
        string kernelAccessor,
        string metaAccessor,
        bool isInsideCollection,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        ITypeSymbol subType;
        if (field is IArrayTypeSymbol arr)
        {
            subType = arr.ElementType;
        }
        else if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = GetSubtype(namedTypeSymbol);
        }
        else
        {
            return;
        }
        
        using (var f = sb.Call(
                   $"await {kernelAccessor}.WriteLoqui"))
        {
            f.Add($"writer: {writerAccessor}");
            f.Add($"fieldName: \"{fieldName}\"");
            f.Add($"item: {fieldAccessor}");
            f.Add($"serializationMetaData: {metaAccessor}");
            f.Add(subSb =>
            {
                subSb.AppendLine($"writeCall: static async (w, o, k, m) =>");
                using (subSb.CurlyBrace())
                {
                    _forFieldGenerator().Value.GenerateSerializeForField(
                        compilation: compilation,
                        obj: obj,
                        fieldType: subType,
                        writerAccessor: "w",
                        kernelAccessor: "k",
                        metaDataAccessor: "m",
                        fieldName: "Male",
                        fieldAccessor: $"o.Male",
                        defaultValueAccessor: null,
                        sb: subSb,
                        cancel: cancel);
                    _forFieldGenerator().Value.GenerateSerializeForField(
                        compilation: compilation,
                        obj: obj,
                        fieldType: subType,
                        writerAccessor: "w",
                        kernelAccessor: "k",
                        metaDataAccessor: "m",
                        fieldName: "Female",
                        fieldAccessor: $"o.Female",
                        defaultValueAccessor: null,
                        sb: subSb,
                        cancel: cancel);
                }
            });
        }
    }

    public bool HasVariableHasSerialize => true;

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException($"No GetDefault defined for {typeof(GenderedTypeFieldGenerator)}");
    }

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor, 
        string metaAccessor,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        ITypeSymbol subType;
        if (field is IArrayTypeSymbol arr)
        {
            subType = arr.ElementType;
        }
        else if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = GetSubtype(namedTypeSymbol);
        }
        else
        {
            return;
        }

        var nullable = field.IsNullable();

        if (nullable)
        {
            sb.AppendLine($"if ({fieldAccessor} is {{ }} {fieldName}Item)");
            fieldAccessor = $"{fieldName}Item";
        }

        using (sb.CurlyBrace(doIt: nullable))
        {
            _forFieldGenerator().Value.GenerateHasSerializeForField(
                compilation: compilation,
                obj: obj, 
                fieldType: subType,
                fieldName: "Male",
                fieldAccessor: $"{fieldAccessor}.Male",
                defaultValueAccessor: null,
                sb: sb,
                cancel: cancel);
            _forFieldGenerator().Value.GenerateHasSerializeForField(
                compilation: compilation,
                obj: obj, 
                fieldType: subType,
                fieldName: "Female", 
                fieldAccessor: $"{fieldAccessor}.Female",
                defaultValueAccessor: null,
                sb: sb,
                cancel: cancel);
        }
    }

    public void GenerateForDeserializeSingleFieldInto(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string readerAccessor,
        string kernelAccessor,
        string metaAccessor,
        bool insideCollection,
        bool canSet,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        ITypeSymbol subType;
        if (field is IArrayTypeSymbol arr)
        {
            subType = arr.ElementType;
        }
        else if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = GetSubtype(namedTypeSymbol);
        }
        else
        {
            return;
        }
        
        var subGen = _forFieldGenerator().Value.GetGenerator(obj, compilation, subType, null);
        var def = subGen?.GetDefault(subType) ?? $"default({subType})";
        
        Utility.WrapStripNull(
            field,
            fieldName,
            fieldAccessor,
            readerAccessor,
            kernelAccessor,
            insideCollection,
            sb,
            (s, t, k, r, setAccessor) =>
            {
                using (var f = s.Call(
                           $"{setAccessor}await {k}.ReadLoqui"))
                {
                    f.Add($"reader: {r}");
                    f.Add($"serializationMetaData: {metaAccessor}");
                    f.Add(subSb =>
                    {
                        subSb.AppendLine($"readCall: static (r, k, m) =>");
                        using (subSb.CurlyBrace())
                        {
                            using (var c = subSb.Call($"return SerializationHelper.ReadGenderedItem<ISerializationReaderKernel<TReadObject>, TReadObject, {subType}>"))
                            {
                                c.Add($"reader: r");
                                c.Add($"kernel: k");
                                c.Add($"metaData: m");
                                c.Add($"ret: new GenderedItem<{subType}>({def}, {def})");
                                c.Add(readerSb =>
                                {
                                    readerSb.AppendLine("itemReader: static async (r2, k2, m2, n) =>");
                                    using (readerSb.CurlyBrace())
                                    {
                                        _forFieldGenerator().Value.GenerateDeserializeForField(
                                            compilation: compilation,
                                            obj: obj, fieldType: subType,
                                            readerAccessor: "r2",
                                            kernelAccessor: "k2",
                                            metaDataAccessor: "m2",
                                            fieldName: "n",
                                            fieldAccessor: $"return ",
                                            sb: readerSb,
                                            cancel: cancel);
                                    }
                                });
                            }
                        }
                    });
                }
            });
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}