using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class ListFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public ListFieldGenerator(Func<IOwned<SerializationFieldGenerator>> forFieldGenerator)
    {
        _forFieldGenerator = forFieldGenerator;
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "List",
        "IReadOnlyList",
        "IList",
        "ExtendedList",
    };

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            if (arr.ElementType.Name == "Byte") return false;
            return true;
        }
        else
        {
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
            var typeMembers = namedTypeSymbol.TypeArguments;
            if (typeMembers.Length != 1) return false;
            return _listStrings.Contains(typeSymbol.Name);
        }
    }

    private ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

    public void GenerateForSerialize(
        ITypeSymbol obj, 
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string writerAccessor,
        string kernelAccessor, 
        StructuredStringBuilder sb)
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
        sb.AppendLine($"foreach (var listItem in {fieldAccessor})");
        using (sb.CurlyBrace())
        {
            _forFieldGenerator().Value.GenerateForField(obj, subType, null, "listItem", sb);
        }
    }

    public void GenerateForDeserialize(ITypeSymbol obj, IPropertySymbol propertySymbol, string itemAccessor, string writerAccessor,
        string kernelAccessor, StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}