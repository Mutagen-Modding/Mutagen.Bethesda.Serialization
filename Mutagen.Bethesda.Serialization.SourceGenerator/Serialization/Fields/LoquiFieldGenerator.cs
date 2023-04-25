using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class IsLoquiFieldTester
{
    private readonly IsGroupTester _groupTester;
    private readonly IsLoquiObjectTester _isLoquiObjectTester;

    private static HashSet<string> _genericTestTypes = new()
    {
        "IMajorRecordInternal"
    };

    public IsLoquiFieldTester(IsGroupTester groupTester, IsLoquiObjectTester isLoquiObjectTester)
    {
        _groupTester = groupTester;
        _isLoquiObjectTester = isLoquiObjectTester;
    }

    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName)
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
}

public class LoquiFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiSerializationNaming _loquiSerializationNaming;
    private readonly ObjRequiresFolderTester _objRequiresFolder;
    private readonly IsLoquiFieldTester _isLoquiObjectTester;
    public IEnumerable<string> AssociatedTypes => Enumerable.Empty<string>();

    public LoquiFieldGenerator(
        IsLoquiFieldTester isLoquiObjectTester,
        LoquiSerializationNaming loquiSerializationNaming,
        ObjRequiresFolderTester objRequiresFolder)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
        _loquiSerializationNaming = loquiSerializationNaming;
        _objRequiresFolder = objRequiresFolder;
    }

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol) => Enumerable.Empty<string>();
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;
    
    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        return _isLoquiObjectTester.Applicable(obj, customization, typeSymbol, fieldName)
               && !_objRequiresFolder.ObjRequiresFolder(obj, typeSymbol, customization);
    }
    
    public void GenerateForSerialize(
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
        CancellationToken cancel)
    {
        if (field is ITypeParameterSymbol namedTypeSymbol
            && namedTypeSymbol.ConstraintTypes.Length == 1)
        {
            return;
        }

        if (!_loquiSerializationNaming.TryGetSerializationItems(field, out var fieldSerializationItems)) return;
        if (!compilation.Mapping.TryGetTypeSet(field, out var typeSet)) return;

        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);

        var call = fieldSerializationItems.SerializationCall(withCheck: hasInheriting);
        
        if (fieldName != null)
        {
            using (var i = sb.If(ands: true))
            {
                i.Add($"{fieldAccessor} is {{}} {fieldName}Checked");
                fieldAccessor = $"{fieldName}Checked";
                if (!field.IsNullable())
                {
                    i.Add($"{fieldSerializationItems.HasSerializationCall(hasInheriting)}({fieldAccessor}, {metaAccessor})");
                }
            }
        }
        using (sb.CurlyBrace(doIt: fieldName != null))
        {
            sb.AppendLine($"await {kernelAccessor}.WriteLoqui({writerAccessor}, {(fieldName == null ? "null" : $"\"{fieldName}\"")}, {fieldAccessor}, {metaAccessor}, static (w, i, k, m) => {call}<TKernel, TWriteObject>(w, i, k, m));");
        }
    }

    public bool HasVariableHasSerialize => true;

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        if (!_loquiSerializationNaming.TryGetSerializationItems(field, out var fieldSerializationItems)) return;
        if (!compilation.Mapping.TryGetTypeSet(field, out var typeSet)) return;
        sb.AppendLine($"if ({fieldSerializationItems.HasSerializationCall(withCheck: compilation.Mapping.HasInheritingClasses(typeSet))}({fieldAccessor}, {metaAccessor})) return true;");
    }

    public void GenerateForDeserializeSingleFieldInto(
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
        CancellationToken cancel)
    {
        if (field is ITypeParameterSymbol namedTypeSymbol
            && namedTypeSymbol.ConstraintTypes.Length == 1)
        {
            return;
        }

        if (!_loquiSerializationNaming.TryGetSerializationItems(field, out var fieldSerializationItems)) return;
        if (!compilation.Mapping.TryGetTypeSet(field, out var typeSet)) return;

        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);

        var call = fieldSerializationItems.DeserializationCall(withCheck: hasInheriting);

        if (canSet || insideCollection)
        {
            Utility.WrapStripNull(
                field, 
                fieldName,
                fieldAccessor, 
                readerAccessor, 
                kernelAccessor,
                insideCollection,
                sb,
                (sb, f, k, r, setAccessor) =>
                {
                    sb.AppendLine($"{setAccessor}await {k}.ReadLoqui({r}, {metaAccessor}, static (r, k, m) => {call}<TReadObject>(r, k, m));");
                });}
        else
        {
            sb.AppendLine($"var tmp{fieldName} = await {kernelAccessor}.ReadLoqui({readerAccessor}, {metaAccessor}, static (r, k, m) => {call}<TReadObject>(r, k, m));");
            sb.AppendLine($"{fieldAccessor}.DeepCopyIn(tmp{fieldName});");
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}