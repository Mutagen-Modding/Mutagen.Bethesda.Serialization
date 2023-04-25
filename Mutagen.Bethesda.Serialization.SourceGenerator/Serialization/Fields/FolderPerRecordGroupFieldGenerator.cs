using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FolderPerRecordGroupFieldGenerator : ISerializationForFieldGenerator
{
    private readonly ObjRequiresFolderTester _objRequiresFolder;
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly LoquiSerializationNaming _serializationNaming;

    public FolderPerRecordGroupFieldGenerator(
        ObjRequiresFolderTester objRequiresFolder,
        LoquiNameRetriever nameRetriever,
        LoquiSerializationNaming serializationNaming)
    {
        _objRequiresFolder = objRequiresFolder;
        _nameRetriever = nameRetriever;
        _serializationNaming = serializationNaming;
    }

    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public bool HasVariableHasSerialize => true;
    
    public IEnumerable<string> RequiredNamespaces(LoquiTypeSet obj, CompilationUnit compilation, ITypeSymbol typeSymbol)
        => Enumerable.Empty<string>();

    public bool Applicable(LoquiTypeSet obj, 
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol, string? fieldName)
    {
        if (!customization.FilePerRecord) return false;
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeParameters.Length == 1)
        {
            var name = _nameRetriever.GetNames(typeSymbol.Name);

            if (!name.Direct.EndsWith("Group")) return false;

            var subObj = namedTypeSymbol.TypeArguments[0];

            if (_objRequiresFolder.ObjRequiresFolder(obj, subObj, customization)) return true;
        }

        return false;
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public void GenerateForSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string writerAccessor, string kernelAccessor,
        string metaAccessor, bool insideCollection, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;
        
        var groupNames = _nameRetriever.GetNames(field);
        var subNames = _nameRetriever.GetNames(subType);
        
        if (!compilation.Mapping.TryGetTypeSet(subType, out var typeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);

        
        using (var f = sb.Call($"tasks.Add(SerializationHelper.WriteFolderPerRecord<TKernel, TWriteObject, {groupNames.Getter}<{subNames.Getter}>, {subNames.Getter}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: {writerAccessor}.StreamPackage");
            f.Add($"group: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add($"groupWriter: static (w, i, k, m) => {fieldSerializationNames.SerializationCall()}<TKernel, TWriteObject, {subNames.Getter}>(w, i, k, m)");
            f.Add($"itemWriter: static (w, i, k, m) => {subSerializationNames.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"withNumbering: {compilation.Customization.Overall.EnforceRecordOrder.ToString().ToLower()}");
        }
    }

    public void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
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
        if (!compilation.Customization.Overall.FilePerRecord) return;
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;
        var groupNames = _nameRetriever.GetNames(field);
        var subNames = _nameRetriever.GetNames(subType);

        if (!compilation.Mapping.TryGetTypeSet(subType, out var typeSet)) return;

        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);
        
        using (var f = sb.Call(
                   $"tasks.Add(SerializationHelper.ReadFolderPerRecord<ISerializationReaderKernel<TReadObject>, TReadObject, {groupNames.Setter}<{subNames.Direct}>, {subNames.Direct}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: {readerAccessor}.StreamPackage");
            f.Add($"fieldName: \"{fieldName}\"");
            f.Add($"group: {fieldAccessor}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"groupReader: static (r, i, k, m, n) => {fieldSerializationNames.DeserializationSingleFieldIntoCall()}<TReadObject, {subNames.Direct}>(r, k, i, m, n)");
            f.Add(
                $"itemReader: static async (r, k, m) => SerializationHelper.StripNull(await k.ReadLoqui(r, m, {subSerializationNames.DeserializationCall(hasInheriting)}<TReadObject>), \"{fieldName}\")");
        }
    }
}