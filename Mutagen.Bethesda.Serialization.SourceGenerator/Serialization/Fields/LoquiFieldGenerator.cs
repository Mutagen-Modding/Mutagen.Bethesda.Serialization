using Microsoft.CodeAnalysis;
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
        CompilationUnit compilation, 
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
    private readonly IsMajorRecordTester _majorRecordTester;
    private readonly IsLoquiFieldTester _isLoquiObjectTester;
    public IEnumerable<string> AssociatedTypes => Enumerable.Empty<string>();

    public LoquiFieldGenerator(
        IsMajorRecordTester majorRecordTester,
        IsLoquiFieldTester isLoquiObjectTester,
        LoquiSerializationNaming loquiSerializationNaming,
        ObjRequiresFolderTester objRequiresFolder)
    {
        _majorRecordTester = majorRecordTester;
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
        CompilationUnit compilation, 
        ITypeSymbol typeSymbol, 
        string? fieldName,
        bool isInsideCollection)
    {
        if (fieldName != null
            && !isInsideCollection
            && compilation.Customization.Overall.FilePerRecord 
            && (!compilation.Customization.TargetRecordSpecs?.EmbedRecordForProperty(fieldName) ?? true)
            && _majorRecordTester.IsMajorRecord(typeSymbol))
        {
            return false;
        }
        return _isLoquiObjectTester.Applicable(obj, compilation, typeSymbol, fieldName)
               && !_objRequiresFolder.ObjRequiresFolder(obj, typeSymbol, fieldName, compilation);
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
        if (field is ITypeParameterSymbol typeParameterSymbol
            && typeParameterSymbol.ConstraintTypes.Length == 1)
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
            string generics = "TKernel, TWriteObject";
            if (field is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Length > 0)
            {
                generics += ", " + string.Join(", ", namedTypeSymbol.TypeArguments);
            }
            sb.AppendLine($"await {kernelAccessor}.WriteLoqui({writerAccessor}, {(fieldName == null ? "null" : $"\"{fieldName}\"")}, {fieldAccessor}, {metaAccessor}, static (w, i, k, m) => {call}<{generics}>(w, i, k, m));");
        }
    }

    public bool HasVariableHasSerialize => true;

    public string? GetDefault(ITypeSymbol field)
    {
        if (field.IsNullable())
        {
            return $"default({field})";
        }
        else
        {
            return $"new {field}()";
        }
    }

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
        if (field is ITypeParameterSymbol typeParameterSymbol
            && typeParameterSymbol.ConstraintTypes.Length == 1)
        {
            return;
        }

        if (!_loquiSerializationNaming.TryGetSerializationItems(field, out var fieldSerializationItems)) return;
        if (!compilation.Mapping.TryGetTypeSet(field, out var typeSet)) return;

        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);

        var call = fieldSerializationItems.DeserializationCall(withCheck: hasInheriting);

        var genString = "TReadObject";
        if (field is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Length > 0)
        {
            genString += ", " + string.Join(", ", namedTypeSymbol.TypeArguments);
        }

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
                    sb.AppendLine($"{setAccessor}await {k}.ReadLoqui({r}, {metaAccessor}, static (r, k, m) => {call}<{genString}>(r, k, m));");
                });}
        else
        {
            sb.AppendLine($"var tmp{fieldName} = await {kernelAccessor}.ReadLoqui({readerAccessor}, {metaAccessor}, static (r, k, m) => {call}<{genString}>(r, k, m));");
            sb.AppendLine($"if (tmp{fieldName} == null) return;");
            sb.AppendLine($"{fieldAccessor}.DeepCopyIn(tmp{fieldName});");
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}