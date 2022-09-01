using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class LoquiFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiMapper _loquiMapper;
    private readonly LoquiSerializationNaming _loquiSerializationNaming;
    private readonly IsLoquiObjectTester _isLoquiObjectTester;
    public IEnumerable<string> AssociatedTypes => Enumerable.Empty<string>();

    public LoquiFieldGenerator(
        IsLoquiObjectTester isLoquiObjectTester,
        LoquiMapper loquiMapper,
        LoquiSerializationNaming loquiSerializationNaming)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
        _loquiMapper = loquiMapper;
        _loquiSerializationNaming = loquiSerializationNaming;
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
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field, 
        string? fieldName,
        string fieldAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        if (field is ITypeParameterSymbol namedTypeSymbol
            && namedTypeSymbol.ConstraintTypes.Length == 1)
        {
            return;
        }

        if (!_loquiSerializationNaming.TryGetSerializationItems(field, out var fieldSerializationItems)) return;

        sb.AppendLine($"{fieldSerializationItems.SerializationCall(serialize: true, withCheck: compilation.Mapping.HasInheritingClasses(field))}({fieldAccessor}, {writerAccessor}, {kernelAccessor});");
    }

    public void GenerateForDeserialize(
        CompilationUnit compilation,
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