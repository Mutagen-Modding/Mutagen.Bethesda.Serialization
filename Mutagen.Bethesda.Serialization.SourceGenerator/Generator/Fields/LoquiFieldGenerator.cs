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
        ITypeSymbol field, 
        string? fieldName,
        string fieldAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb)
    {
        sb.AppendLine($"{field.Name}_Serialization.Serialize({fieldAccessor}, {writerAccessor}, {kernelAccessor});");
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