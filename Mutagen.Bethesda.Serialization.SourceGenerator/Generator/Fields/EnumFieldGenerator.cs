using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class EnumFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind == TypeKind.Enum;
    }

    public void GenerateForSerialize(ITypeSymbol obj, IPropertySymbol propertySymbol,
        string itemAccessor, string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        sb.AppendLine($"{kernelAccessor}.WriteEnum({writerAccessor}, {itemAccessor}.{propertySymbol.Name});");
    }

    public void GenerateForDeserialize(ITypeSymbol obj, IPropertySymbol propertySymbol,
        string itemAccessor, string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}