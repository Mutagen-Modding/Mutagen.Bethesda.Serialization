using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FolderPerRecordGroupFieldGenerator : ISerializationForFieldGenerator
{
    private readonly MajorRecordListFieldGenerator _majorRecordListFieldGenerator;
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly LoquiSerializationNaming _serializationNaming;

    public FolderPerRecordGroupFieldGenerator(
        MajorRecordListFieldGenerator majorRecordListFieldGenerator,
        LoquiNameRetriever nameRetriever,
        LoquiSerializationNaming serializationNaming)
    {
        _majorRecordListFieldGenerator = majorRecordListFieldGenerator;
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

            foreach (var prop in subObj.GetMembers().OfType<IPropertySymbol>())
            {
                if (_majorRecordListFieldGenerator.Applicable(obj, customization, prop.Type, prop.Name))
                {
                    return true;
                }
            }
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

        
        using (var f = sb.Call($"SerializationHelper.WriteFolderPerRecord<TKernel, TWriteObject, {groupNames.Getter}<{subNames.Getter}>, {subNames.Getter}>"))
        {
            f.Add($"streamPackage: {writerAccessor}.StreamPackage");
            f.Add($"group: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add($"groupWriter: static (w, i, k, m) => {fieldSerializationNames.SerializationCall()}<TKernel, TWriteObject, {subNames.Getter}>(w, i, k, m)");
            f.Add($"itemWriter: static (w, i, k, m) => {subSerializationNames.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"withNumbering: {compilation.Customization.Overall.EnforceRecordOrder.ToString().ToLower()}");
            f.Add($"toDo: parallelToDo");
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
                   $"SerializationHelper.ReadFolderPerRecord<ISerializationReaderKernel<TReadObject>, TReadObject, {groupNames.Setter}<{subNames.Direct}>, {subNames.Direct}>"))
        {
            f.Add($"streamPackage: {readerAccessor}.StreamPackage");
            f.Add($"fieldName: \"{fieldName}\"");
            f.Add($"group: {fieldAccessor}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"groupReader: static (r, i, k, m, n) => {fieldSerializationNames.DeserializationSingleFieldIntoCall()}<TReadObject, {subNames.Direct}>(r, k, i, m, n)");
            f.Add(
                $"itemReader: static (r, k, m) => k.ReadLoqui(r, m, {subSerializationNames.DeserializationCall(hasInheriting)}<TReadObject>)");
            f.Add($"toDo: parallelToDo");
        }
    }
}