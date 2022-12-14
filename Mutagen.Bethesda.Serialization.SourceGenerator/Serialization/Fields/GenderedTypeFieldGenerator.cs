using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class GenderedTypeFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();
    
    public GenderedTypeFieldGenerator(Func<IOwned<SerializationFieldGenerator>> forFieldGenerator)
    {
        _forFieldGenerator = forFieldGenerator;
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "GenderedItem",
        "IGenderedItem",
        "IGenderedItemGetter",
    };

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Mutagen.Bethesda.Plugins.Records";
        var subType = GetSubtype((INamedTypeSymbol)typeSymbol);
        var gen = _forFieldGenerator().Value
            .GetGenerator(subType, cancel);
        if (gen != null)
        {
            foreach (var ns in gen.RequiredNamespaces(subType, cancel))
            {
                yield return ns;
            }
        }
    }
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;
    
    public bool Applicable(ITypeSymbol typeSymbol)
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
                   $"{kernelAccessor}.WriteLoqui"))
        {
            f.Add($"writer: {writerAccessor}");
            f.Add($"fieldName: \"{fieldName}\"");
            f.Add($"item: {fieldAccessor}");
            f.Add($"serializationMetaData: {metaAccessor}");
            f.Add(subSb =>
            {
                subSb.AppendLine($"writeCall: static (w, o, k, m) =>");
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

    public void GenerateForDeserialize(
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
        
        fieldAccessor = $"{fieldAccessor}{(insideCollection ? null : " = ")}";

        using (var f = sb.Call(
                   $"{fieldAccessor}{kernelAccessor}.ReadLoqui"))
        {
            f.Add($"reader: {readerAccessor}");
            f.Add($"serializationMetaData: {metaAccessor}");
            f.Add(subSb =>
            {
                subSb.AppendLine($"readCall: static (r, k, m) =>");
                using (subSb.CurlyBrace())
                {
                    using (var c = subSb.Call("return SerializationHelper.ReadGenderedItem"))
                    {
                        c.Add($"reader: r");
                        c.Add($"kernel: k");
                        c.Add($"metaData: m");
                        c.Add($"ret: new GenderedItem<{subType}>(default({subType}), default({subType}))");
                        c.Add(readerSb =>
                        {
                            readerSb.AppendLine("itemReader: static (r2, k2, m2, n) =>");
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
    }
}