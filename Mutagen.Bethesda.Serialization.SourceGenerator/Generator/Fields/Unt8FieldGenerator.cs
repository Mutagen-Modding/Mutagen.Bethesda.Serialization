using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class UInt8FieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => new string[]
    {
        "byte",
        "Byte",
        "UInt8"
    };

    public bool Applicable(ITypeSymbol typeSymbol) => false;

    public void GenerateForSerialize(ITypeSymbol obj, IPropertySymbol propertySymbol,
        string itemAccessor, string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        sb.AppendLine($"{kernelAccessor}.WriteUInt8({writerAccessor}, {itemAccessor}.{propertySymbol.Name});");
    }

    public void GenerateForDeserialize(ITypeSymbol obj, IPropertySymbol propertySymbol,
        string itemAccessor, string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}