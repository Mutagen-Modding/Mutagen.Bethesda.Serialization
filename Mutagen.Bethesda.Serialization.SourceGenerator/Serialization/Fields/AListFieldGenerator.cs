using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public abstract class AListFieldGenerator : ISerializationForFieldGenerator
{
    protected readonly IsGroupTester GroupTester;
    protected readonly Func<IOwned<SerializationFieldGenerator>> ForFieldGenerator;
    private readonly IsListTester _isListTester;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public AListFieldGenerator(
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator,
        IsListTester isListTester,
        IsGroupTester groupTester)
    {
        ForFieldGenerator = forFieldGenerator;
        _isListTester = isListTester;
        GroupTester = groupTester;
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        var subType = GetSubtype((INamedTypeSymbol)typeSymbol);
        var gen = ForFieldGenerator().Value
            .GetGenerator(obj, compilation, subType, fieldName: null, isInsideCollection: true);
        return gen?.RequiredNamespaces(obj, compilation, subType) ?? Enumerable.Empty<string>();
    }
    
    protected bool ShouldSkip(
        CustomizationSpecifications customization,
        LoquiTypeSet obj, 
        string? fieldName)
    {
        if (GroupTester.IsGroup(obj.Getter)) return true;
        if (customization.FilePerRecord)
        {
            if (obj.Direct == null) return false;
            if (obj.Direct.Name.Equals("CellBlock")) return true;
            if (obj.Direct.Name.Equals("CellSubBlock")) return true;
            if (obj.Direct.Name.Equals("WorldspaceBlock")) return true;
            if (obj.Direct.Name.Equals("WorldspaceSubBlock")) return true;
            if (obj.Direct.Name == "Worldspace" && fieldName == "SubCells") return true;
        }
        return false;
    }
    
    public virtual bool Applicable(
        LoquiTypeSet obj, 
        CompilationUnit compilation,
        ITypeSymbol typeSymbol, 
        string? fieldName,
        bool isInsideCollection)
    {
        return _isListTester.Applicable(typeSymbol);
    }

    protected ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

    protected string GetCountAccessor(ITypeSymbol t)
    {
        switch (t.Name)
        {
            case "List":
            case "IReadOnlyList":
            case "IList":
            case "ExtendedList":
                return ".Count";
            default:
                return ".Length";
        }
    }

    public abstract void GenerateForSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor,
        string metaAccessor,
        bool isInsideCollection,
        StructuredStringBuilder sb,
        CancellationToken cancel);

    public bool HasVariableHasSerialize => true;

    public string? GetDefault(ITypeSymbol field)
    {
        if (field is not INamedTypeSymbol namedField)
        {
            throw new NotImplementedException($"No GetDefault defined for {typeof(AListFieldGenerator)} for {field}");
        }

        if (field.IsNullable())
        {
            return "default";
        }
        else
        {
            return $"new ExtendedList<{GetSubtype(namedField)}>()";
        }
    }

    public abstract void GenerateForHasSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel);

    public abstract void GenerateForDeserializeSingleFieldInto(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string readerAccessor,
        string kernelAccessor,
        string metaAccessor,
        bool insideCollection,
        bool canSet,
        StructuredStringBuilder sb,
        CancellationToken cancel);

    public virtual void GenerateForDeserializeSection(
        CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}