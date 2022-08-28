using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class ArrayFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<ListFieldGenerator>> _listGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public ArrayFieldGenerator(Func<IOwned<ListFieldGenerator>> listGenerator)
    {
        _listGenerator = listGenerator;
    }
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            return arr.ElementType.Name != "Byte";
        }

        return false;
    }

    public void GenerateForSerialize(ITypeSymbol obj, ITypeSymbol field, string? fieldName, string fieldAccessor,
        string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        _listGenerator().Value.GenerateForSerialize(obj, field, fieldName, fieldAccessor, writerAccessor, kernelAccessor, sb);
    }

    public void GenerateForDeserialize(ITypeSymbol obj, IPropertySymbol propertySymbol, string itemAccessor, string writerAccessor,
        string kernelAccessor, StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}