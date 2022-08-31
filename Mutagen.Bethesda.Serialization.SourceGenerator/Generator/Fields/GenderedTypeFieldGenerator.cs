using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

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
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _listStrings.Contains(typeSymbol.Name);
    }

    private ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

    public void GenerateForSerialize(
        Compilation compilation,
        ITypeSymbol obj, 
        ITypeSymbol field, 
        string? fieldName,
        string fieldAccessor,
        string writerAccessor, 
        string kernelAccessor,
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
        _forFieldGenerator().Value.GenerateForField(compilation, obj, subType, writerAccessor, fieldName == null ? null : $"{fieldName}Male", $"{fieldAccessor}.Male", sb, cancel);
        _forFieldGenerator().Value.GenerateForField(compilation, obj, subType, writerAccessor, fieldName == null ? null : $"{fieldName}Female", $"{fieldAccessor}.Female", sb, cancel);
    }

    public void GenerateForDeserialize(
        Compilation compilation,
        ITypeSymbol obj,
        IPropertySymbol propertySymbol,
        string itemAccessor, 
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        throw new NotImplementedException();
    }
}