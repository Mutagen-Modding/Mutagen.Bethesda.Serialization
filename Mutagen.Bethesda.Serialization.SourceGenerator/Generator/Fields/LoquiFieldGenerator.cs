using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class LoquiFieldGenerator : ISerializationForFieldGenerator
{
    private readonly IsLoquiObjectTester _isLoquiObjectTester;
    public IEnumerable<string> AssociatedTypes => Enumerable.Empty<string>();

    public LoquiFieldGenerator(IsLoquiObjectTester isLoquiObjectTester)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
    }

    public bool Applicable(ITypeSymbol typeSymbol) => _isLoquiObjectTester.IsLoqui(typeSymbol);
    
    public void GenerateForSerialize(
        ITypeSymbol obj,
        IPropertySymbol propertySymbol,
        string itemAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb)
    {
        sb.AppendLine($"{propertySymbol.Type.Name}_Serialization.Serialize({itemAccessor}.{propertySymbol.Name}, {writerAccessor}, {kernelAccessor});");
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