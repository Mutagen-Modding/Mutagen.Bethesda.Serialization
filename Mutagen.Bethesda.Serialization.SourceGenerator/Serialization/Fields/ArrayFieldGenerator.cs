using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
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

        throw new ArgumentException($"Could not get SubType on {t} in {nameof(ArrayFieldGenerator)}");
    }
    
    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        var subType = GetSubtype(typeSymbol);
        var gen = _forFieldGenerator().Value
            .GetGenerator(obj, compilation, subType, fieldName: null);
        return gen?.RequiredNamespaces(obj, compilation, subType) ?? Enumerable.Empty<string>();
    }
    
    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            return arr.ElementType.Name != "Byte";
        }

        typeSymbol = typeSymbol.PeelNullable();
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        if (!_listStrings.Contains(typeSymbol.Name)) return false;
        return typeMembers[0].Name != "Byte";
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public string Name(ITypeSymbol typeSymbol) => typeSymbol is IArrayTypeSymbol ? "Array" : "Slice";

    public bool HasVariableHasSerialize => true;

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException($"No GetDefault defined for {typeof(ArrayFieldGenerator)}");
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
        sb.AppendLine($"if ({fieldAccessor}{field.NullChar()}.Length > 0) return true;");
    }

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
            isInsideCollection: isInsideCollection,
            sb: sb,
            cancel: cancel);
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
        field = field.PeelNullable();

        var subType = GetSubtype(field);

        var readOut = canSet;

        using (sb.CurlyBrace())
        {
            using (var c = sb.Call($"{(readOut ? $"{fieldAccessor} = " : null)}await SerializationHelper.Read{(readOut ? null : "Into")}{Name(field)}<ISerializationReaderKernel<TReadObject>, TReadObject, {subType}>"))
            {
                c.AddPassArg("reader");
                if (!readOut)
                {
                    c.Add($"arr: {fieldAccessor}");
                }
                c.AddPassArg("kernel");
                c.AddPassArg("metaData");
                c.Add((subSb) =>
                {
                    subSb.AppendLine("itemReader: static async (r, k, m) =>");
                    using (subSb.CurlyBrace())
                    {
                        _forFieldGenerator().Value.GenerateDeserializeForField(
                            compilation: compilation,
                            obj: obj,
                            fieldType: subType,
                            readerAccessor: "r", 
                            kernelAccessor: "k",
                            metaDataAccessor: "m",
                            fieldName: fieldName, 
                            fieldAccessor: "return ",
                            sb: subSb,
                            cancel: cancel);
                    }
                });
            }
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}