using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class GroupFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiSerializationNaming _serializationNaming;
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly FolderPerRecordGroupFieldGenerator _folderPerRecordGroupFieldGenerator;
    private readonly BlocksFieldGenerator _blocksFieldGenerator;
    private readonly BlocksXYFieldGenerator _blocksXyFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public GroupFieldGenerator(
        LoquiSerializationNaming serializationNaming,
        LoquiNameRetriever nameRetriever,
        FolderPerRecordGroupFieldGenerator folderPerRecordGroupFieldGenerator,
        BlocksFieldGenerator blocksFieldGenerator,
        BlocksXYFieldGenerator blocksXyFieldGenerator)
    {
        _serializationNaming = serializationNaming;
        _nameRetriever = nameRetriever;
        _folderPerRecordGroupFieldGenerator = folderPerRecordGroupFieldGenerator;
        _blocksFieldGenerator = blocksFieldGenerator;
        _blocksXyFieldGenerator = blocksXyFieldGenerator;
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
        if (_blocksFieldGenerator.Applicable(obj, customization, typeSymbol, fieldName)
            || _blocksXyFieldGenerator.Applicable(obj, customization, typeSymbol, fieldName)
            || _folderPerRecordGroupFieldGenerator.Applicable(obj, customization, typeSymbol, fieldName))
        {
            return false;
        }

        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeParameters.Length == 1)
        {
            var name = _nameRetriever.GetNames(typeSymbol.Name);
            if (name.Direct == "ListGroup")
            {
                return !customization.FilePerRecord;
            }
            
            if (name.Direct.EndsWith("Group"))
            {
                return true;
            }
        }

        return false;
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
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;
        
        var groupNames = _nameRetriever.GetNames(field);
        var subNames = _nameRetriever.GetNames(subType);
        
        if (!compilation.Mapping.TryGetTypeSet(subType, out var typeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);

        if (compilation.Customization.Overall.FilePerRecord)
        {
            using (var f = sb.Call($"tasks.Add(SerializationHelper.WriteFilePerRecord<TKernel, TWriteObject, {groupNames.Getter}<{subNames.Getter}>, {subNames.Getter}>",
                       suffixLine: ")"))
            {
                f.Add($"streamPackage: {writerAccessor}.StreamPackage");
                f.Add($"group: {fieldAccessor}");
                f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
                f.Add($"metaData: {metaAccessor}");
                f.Add($"kernel: {kernelAccessor}");
                f.Add($"groupWriter: static (w, i, k, m) => {fieldSerializationNames.SerializationCall()}<TKernel, TWriteObject, {subNames.Getter}>(w, i, k, m)");
                f.Add($"groupHasSerializationItems: static (i, m) => {fieldSerializationNames.HasSerializationCall()}<{subNames.Getter}>(i, m)");
                f.Add($"itemWriter: static (w, i, k, m) => {subSerializationNames.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
                f.Add($"withNumbering: {compilation.Customization.Overall.EnforceRecordOrder.ToString().ToLower()}");
            }
        }
        else
        {
            using (var f = sb.Call($"await SerializationHelper.WriteGroup<TKernel, TWriteObject, {groupNames.Getter}<{subNames.Getter}>, {subNames.Getter}>"))
            {
                f.Add($"writer: {writerAccessor}");
                f.Add($"group: {fieldAccessor}");
                f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
                f.Add($"metaData: {metaAccessor}");
                f.Add($"kernel: {kernelAccessor}");
                f.Add($"groupWriter: static (w, i, k, m) => {fieldSerializationNames.SerializationCall()}<TKernel, TWriteObject, {subNames.Getter}>(w, i, k, m)");
                f.Add($"itemWriter: static (w, i, k, m) => {subSerializationNames.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
            }
        }
    }

    public bool HasVariableHasSerialize => true;

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException();
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
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
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
        if (compilation.Customization.Overall.FilePerRecord) return;
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;
        var groupNames = _nameRetriever.GetNames(field);
        var subNames = _nameRetriever.GetNames(subType);

        if (!compilation.Mapping.TryGetTypeSet(subType, out var typeSet)) return;

        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);

        var isListGroup = groupNames.Getter.Contains("List");

        using (var f = sb.Call(
                   $"await SerializationHelper.ReadInto{(isListGroup ? "List" : null)}Group<ISerializationReaderKernel<TReadObject>, TReadObject, {groupNames.Setter}<{subNames.Direct}>, {subNames.Direct}>"))
        {
            f.Add($"reader: {readerAccessor}");
            f.Add($"group: {fieldAccessor}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"groupReader: static (r, i, k, m, n) => {fieldSerializationNames.DeserializationSingleFieldIntoCall()}<TReadObject, {subNames.Direct}>(r, k, i, m, n)");
            f.Add(
                $"itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, {subSerializationNames.DeserializationCall(hasInheriting)}<TReadObject>)).StripNull(\"Group\")");
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation,
        LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
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
                   $"tasks.Add(SerializationHelper.ReadFilePerRecord<ISerializationReaderKernel<TReadObject>, TReadObject, {groupNames.Setter}<{subNames.Direct}>, {subNames.Direct}>",
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
                $"itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, {subSerializationNames.DeserializationCall(hasInheriting)}<TReadObject>)).StripNull(\"{fieldName}\")");
        }
    }
}