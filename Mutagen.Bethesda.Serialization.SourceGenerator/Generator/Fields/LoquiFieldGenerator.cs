using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class LoquiFieldGenerator : ISerializationForFieldGenerator
{
    private readonly IsGroupTester _groupTester;
    private readonly LoquiSerializationNaming _loquiSerializationNaming;
    private readonly IsLoquiObjectTester _isLoquiObjectTester;
    public IEnumerable<string> AssociatedTypes => Enumerable.Empty<string>();

    public LoquiFieldGenerator(
        IsLoquiObjectTester isLoquiObjectTester,
        LoquiSerializationNaming loquiSerializationNaming,
        IsGroupTester groupTester)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
        _loquiSerializationNaming = loquiSerializationNaming;
        _groupTester = groupTester;
    }

    private static HashSet<string> _genericTestTypes = new()
    {
        "IMajorRecordInternal"
    };

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (_groupTester.IsGroup(typeSymbol)) return false;
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
        if (!compilation.Mapping.TryGetTypeSet(field, out var typeSet)) return;

        var call = fieldSerializationItems.SerializationCall(serialize: true, withCheck: compilation.Mapping.HasInheritingClasses(typeSet.Getter));
        if (fieldName == null)
        {
            sb.AppendLine($"{call}({writerAccessor}, {fieldAccessor}, {kernelAccessor});");
        }
        else
        {
            sb.AppendLine($"{kernelAccessor}.WriteLoqui({writerAccessor}, \"{fieldName}\", {fieldAccessor}, static (w, i, k) => {call}(w, i, k));");
        }
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