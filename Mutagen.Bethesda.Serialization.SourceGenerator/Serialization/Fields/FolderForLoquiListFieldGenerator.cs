using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FolderForLoquiListFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiFieldGenerator _loquiFieldGenerator;
    private readonly IsLoquiFieldTester _loquiFieldTester;
    private readonly ObjRequiresFolderTester _objRequiresFolderTester;
    private readonly LoquiSerializationNaming _serializationNaming;
    private readonly IsListTester _isListTester;
    private readonly LoquiNameRetriever _nameRetriever;
    
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public bool HasVariableHasSerialize => true;

    public FolderForLoquiListFieldGenerator(
        IsLoquiFieldTester loquiFieldTester,
        ObjRequiresFolderTester objRequiresFolderTester, 
        LoquiFieldGenerator loquiFieldGenerator,
        LoquiSerializationNaming serializationNaming, 
        IsListTester isListTester,
        LoquiNameRetriever nameRetriever)
    {
        _loquiFieldTester = loquiFieldTester;
        _objRequiresFolderTester = objRequiresFolderTester;
        _loquiFieldGenerator = loquiFieldGenerator;
        _serializationNaming = serializationNaming;
        _isListTester = isListTester;
        _nameRetriever = nameRetriever;
    }
    
    public IEnumerable<string> RequiredNamespaces(LoquiTypeSet obj, CompilationUnit compilation, ITypeSymbol typeSymbol)
        => Enumerable.Empty<string>();

    public bool Applicable(LoquiTypeSet obj, CustomizationSpecifications customization, ITypeSymbol typeSymbol, string? fieldName)
    {
        if (!customization.FilePerRecord) return false;
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        if (!_isListTester.Applicable(obj, customization, typeSymbol, fieldName)) return false;
        return _objRequiresFolderTester.ObjRequiresFolder(obj, namedTypeSymbol.TypeArguments[0], customization);
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public void GenerateForSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string writerAccessor, string kernelAccessor,
        string metaAccessor, bool insideCollection, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        var subNames = _nameRetriever.GetNames(subType);
        
        if (!compilation.Mapping.TryGetTypeSet(subType, out var typeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);
        
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;

        using (var f = sb.Call($"tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, {subNames.Getter}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: {writerAccessor}.StreamPackage");
            f.Add($"list: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add($"itemWriter: static (w, i, k, m) => {subSerializationNames.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"withNumbering: {compilation.Customization.Overall.EnforceRecordOrder.ToString().ToLower()}");
            f.Add("eachRecordInFolder: true");
        }
    }

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException($"No GetDefault defined for {typeof(FolderForSingleLoquiFieldGenerator)}");
    }

    public void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field,
        string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public void GenerateForDeserializeSingleFieldInto(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field,
        string? fieldName, string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor,
        bool insideCollection, bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;

        if (!compilation.Mapping.TryGetTypeSet(subType, out var typeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);
        
        using (var f = sb.Call(
                   $"tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, {subType}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: {readerAccessor}.StreamPackage");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"list: {fieldAccessor}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, {subSerializationNames.DeserializationCall(hasInheriting)}<TReadObject>)).StripNull(\"{fieldName}\")");
            f.Add("eachRecordInFolder: true");
        }
    }
}