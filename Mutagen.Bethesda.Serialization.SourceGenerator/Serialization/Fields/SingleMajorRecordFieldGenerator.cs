using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class SingleMajorRecordFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiFieldGenerator _loquiFieldGenerator;
    private readonly IsMajorRecordTester _majorRecordTester;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();
    public IEnumerable<string> RequiredNamespaces(LoquiTypeSet obj, CompilationUnit compilation, ITypeSymbol typeSymbol)
        => Enumerable.Empty<string>();

    public SingleMajorRecordFieldGenerator(
        LoquiFieldGenerator loquiFieldGenerator,
        IsMajorRecordTester majorRecordTester)
    {
        _loquiFieldGenerator = loquiFieldGenerator;
        _majorRecordTester = majorRecordTester;
    }
    
    public bool Applicable(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol,
        string? fieldName,
        bool isInsideCollection)
    {
        if (isInsideCollection) return false;
        if (fieldName == null) return false;
        if (!compilation.Customization.Overall.FilePerRecord) return false;
        if (compilation.Customization.TargetRecordSpecs?.EmbedRecordForProperty(fieldName) ?? false) return false;
        var ret = _majorRecordTester.IsMajorRecord(typeSymbol);
        return ret;
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public void GenerateForSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string writerAccessor, string kernelAccessor,
        string metaAccessor, bool insideCollection, StructuredStringBuilder sb, CancellationToken cancel)
    {
        _loquiFieldGenerator.GenerateForSerialize(compilation, obj, field, fieldName, fieldAccessor, defaultValueAccessor, writerAccessor, kernelAccessor, metaAccessor, insideCollection, sb, cancel);
    }

    public bool HasVariableHasSerialize => true;
    public string? GetDefault(ITypeSymbol field)
    {
        return _loquiFieldGenerator.GetDefault(field);
    }

    public void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        _loquiFieldGenerator.GenerateForHasSerialize(compilation, obj, field, fieldName, fieldAccessor, defaultValueAccessor, metaAccessor, sb, cancel);
    }

    // ToDo
    // Update to add to separate file?
    
    public void GenerateForDeserializeSingleFieldInto(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field,
        string? fieldName, string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor,
        bool insideCollection, bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        _loquiFieldGenerator.GenerateForDeserializeSingleFieldInto(compilation, obj, field, fieldName, fieldAccessor, readerAccessor, kernelAccessor, metaAccessor, insideCollection, canSet, sb, cancel);
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        _loquiFieldGenerator.GenerateForDeserializeSection(compilation, obj, field, fieldName, fieldAccessor, readerAccessor, kernelAccessor, metaAccessor, insideCollection, canSet, sb, cancel);
    }
}