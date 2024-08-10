using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FolderForSingleLoquiFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiFieldGenerator _loquiFieldGenerator;
    private readonly IsLoquiFieldTester _loquiFieldTester;
    private readonly ObjRequiresFolderTester _objRequiresFolderTester;
    private readonly LoquiSerializationNaming _serializationNaming;
    private readonly LoquiNameRetriever _nameRetriever;
    
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public bool HasVariableHasSerialize => true;

    public FolderForSingleLoquiFieldGenerator(
        IsLoquiFieldTester loquiFieldTester,
        ObjRequiresFolderTester objRequiresFolderTester, 
        LoquiFieldGenerator loquiFieldGenerator,
        LoquiSerializationNaming serializationNaming, 
        LoquiNameRetriever nameRetriever)
    {
        _loquiFieldTester = loquiFieldTester;
        _objRequiresFolderTester = objRequiresFolderTester;
        _loquiFieldGenerator = loquiFieldGenerator;
        _serializationNaming = serializationNaming;
        _nameRetriever = nameRetriever;
    }
    
    public IEnumerable<string> RequiredNamespaces(LoquiTypeSet obj, CompilationUnit compilation, ITypeSymbol typeSymbol)
        => Enumerable.Empty<string>();

    public bool Applicable(LoquiTypeSet obj, CustomizationCatalog customization, ITypeSymbol typeSymbol, string? fieldName)
    {
        if (!customization.Overall.FilePerRecord) return false;
        if (!_loquiFieldTester.Applicable(obj, customization, typeSymbol, fieldName)) return false;
        return _objRequiresFolderTester.ObjRequiresFolder(obj, typeSymbol, fieldName, customization);
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public void GenerateForSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string writerAccessor, string kernelAccessor,
        string metaAccessor, bool insideCollection, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;

        using (var f = sb.Call(
                   "await SerializationHelper.WriteRecordAsFolder"))
        {
            f.Add($"streamPackage: {writerAccessor}.StreamPackage");
            f.Add($"obj: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add($"itemWriter: static (w, i, k, m) => {fieldSerializationNames.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"hasSerializationItems: static (i, m) => {fieldSerializationNames.HasSerializationCall()}(i, m)");
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
        _loquiFieldGenerator.GenerateForHasSerialize(
            compilation, obj, field, fieldName,
            fieldAccessor, defaultValueAccessor, metaAccessor, sb, cancel);
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
        if (compilation.Customization.EmbedRecordForProperty(fieldName)) return;
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;
        var names = _nameRetriever.GetNames(field);
        
        var hasInheriting = compilation.Mapping.HasInheritingClasses(obj);
        
        using (var f = sb.Call(
                   $"{fieldAccessor} = await SerializationHelper.ReadRecordAsFolder<ISerializationReaderKernel<TReadObject>, TReadObject, {names.Direct}>"))
        {
            f.Add($"streamPackage: {readerAccessor}.StreamPackage");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, {fieldSerializationNames.DeserializationCall(hasInheriting)}<TReadObject>)).StripNull(\"{fieldName}\")");
        }
    }
}