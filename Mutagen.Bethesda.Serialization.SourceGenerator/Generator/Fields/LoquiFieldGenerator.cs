using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class LoquiFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiMapping _loquiMapping;
    private readonly IsLoquiObjectTester _isLoquiObjectTester;
    public IEnumerable<string> AssociatedTypes => Enumerable.Empty<string>();

    public LoquiFieldGenerator(IsLoquiObjectTester isLoquiObjectTester, LoquiMapping loquiMapping)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
        _loquiMapping = loquiMapping;
    }

    private static HashSet<string> _genericTestTypes = new()
    {
        "IMajorRecordInternal"
    };

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (_isLoquiObjectTester.IsLoqui(typeSymbol)) return true;
        if (typeSymbol is ITypeParameterSymbol typeParameterSymbol)
        {
            if (typeParameterSymbol.ConstraintTypes.Any(x => _genericTestTypes.Contains(x.Name)))
            {
                return true;
            }
        }

        return false;
    }
    
    public void GenerateForSerialize(
        ITypeSymbol obj,
        ITypeSymbol field, 
        string? fieldName,
        string fieldAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb)
    {
        if (_loquiMapping.HasInheritingClasses(field))
        {
            sb.AppendLine($"{field.Name}_Serialization.SerializeWithCheck({fieldAccessor}, {writerAccessor}, {kernelAccessor});");
        }
        else
        {
            sb.AppendLine($"{field.Name}_Serialization.Serialize({fieldAccessor}, {writerAccessor}, {kernelAccessor});");
        }
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