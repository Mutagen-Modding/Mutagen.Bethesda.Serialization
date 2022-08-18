using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class LoquiFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Enumerable.Empty<string>();
    public void GenerateForSerialize(
        ITypeSymbol obj,
        IPropertySymbol propertySymbol,
        string itemAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb)
    {
        sb.AppendLine($"{obj.Name}_Serialization.Serialize({itemAccessor}.{propertySymbol.Name}, {writerAccessor}, {kernelAccessor});");
    }

    public void GenerateForDeserialize(
        ITypeSymbol obj,
        IPropertySymbol propertySymbol,
        string itemAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}