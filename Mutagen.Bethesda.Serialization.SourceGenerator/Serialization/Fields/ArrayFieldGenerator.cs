using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class ArrayFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<ListFieldGenerator>> _listGenerator;
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    private static readonly HashSet<string> _listStrings = new()
    {
        "MemorySlice",
        "ReadOnlyMemorySlice",
    };

    public ArrayFieldGenerator(
        Func<IOwned<ListFieldGenerator>> listGenerator, 
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator)
    {
        _listGenerator = listGenerator;
        _forFieldGenerator = forFieldGenerator;
    }

    public ITypeSymbol GetSubtype(ITypeSymbol t)
    {
        if (t is IArrayTypeSymbol arr)
        {
            return arr.ElementType;
        }
        else if (t is INamedTypeSymbol named)
        {
            return named.TypeArguments[0];
        }

        throw new NotImplementedException();
    }
    
    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        var subType = GetSubtype(typeSymbol);
        var gen = _forFieldGenerator().Value
            .GetGenerator(subType, cancel);
        return gen?.RequiredNamespaces(subType, cancel) ?? Enumerable.Empty<string>();
    }
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            return arr.ElementType.Name != "Byte";
        }

        typeSymbol = typeSymbol.PeelNullable();
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _listStrings.Contains(typeSymbol.Name);
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public string Name(ITypeSymbol typeSymbol) => typeSymbol is IArrayTypeSymbol ? "Array" : "Slice";

    public bool HasVariableHasSerialize => true;

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor, 
        string metaAccessor,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}{field.NullChar()}.Length > 0) return true;");
    }

    public void GenerateForSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field, 
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        _listGenerator().Value.GenerateForSerialize(
            compilation: compilation,
            obj: obj, 
            field: field,
            fieldName: fieldName, 
            fieldAccessor: fieldAccessor,
            defaultValueAccessor: defaultValueAccessor, 
            writerAccessor: writerAccessor,
            kernelAccessor: kernelAccessor, 
            metaAccessor: metaAccessor,
            sb: sb,
            cancel: cancel);
    }

    public void GenerateForDeserialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
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
        var isNullable = field.IsNullable();
        field = field.PeelNullable();

        var subType = GetSubtype(field);

        using (sb.CurlyBrace())
        {
            using (var c = sb.Call($"{(isNullable ? $"{fieldAccessor} = " : null)}SerializationHelper.Read{(isNullable ? null : "Into")}{Name(field)}"))
            {
                c.AddPassArg("reader");
                if (!isNullable)
                {
                    c.Add($"arr: {fieldAccessor}");
                }
                c.AddPassArg("kernel");
                c.AddPassArg("metaData");
                c.Add((subSb) =>
                {
                    subSb.AppendLine("itemReader: (r, k, m) =>");
                    using (subSb.CurlyBrace())
                    {
                        _forFieldGenerator().Value.GenerateDeserializeForField(
                            compilation: compilation,
                            obj: obj,
                            fieldType: subType,
                            readerAccessor: "r", 
                            fieldName: null, 
                            fieldAccessor: "return ",
                            sb: subSb,
                            cancel: cancel);
                    }
                });
            }
        }
    }
}