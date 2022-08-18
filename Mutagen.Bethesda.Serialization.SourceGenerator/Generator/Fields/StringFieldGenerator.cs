using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class StringFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => new string[]
    {
        "string",
        "String"
    };

    public bool Applicable(ITypeSymbol typeSymbol) => false;

    public void GenerateForSerialize(ITypeSymbol obj, IPropertySymbol propertySymbol,
        string itemAccessor, string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        sb.AppendLine($"{kernelAccessor}.WriteString({writerAccessor}, {itemAccessor}.{propertySymbol.Name});");
    }

    public void GenerateForDeserialize(ITypeSymbol obj, IPropertySymbol propertySymbol,
        string itemAccessor, string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}