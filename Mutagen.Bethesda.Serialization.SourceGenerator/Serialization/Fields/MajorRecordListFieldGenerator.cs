using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class MajorRecordListFieldGenerator : AListFieldGenerator
{
    private readonly IsMajorRecordTester _isMajorRecordTester;
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly LoquiSerializationNaming _serializationNaming;
    
    public MajorRecordListFieldGenerator(
        IsMajorRecordTester isMajorRecordTester,
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator, 
        LoquiNameRetriever nameRetriever,
        LoquiSerializationNaming serializationNaming,
        IsListTester listTester,
        IsGroupTester groupTester) 
        : base(forFieldGenerator, listTester, groupTester)
    {
        _isMajorRecordTester = isMajorRecordTester;
        _nameRetriever = nameRetriever;
        _serializationNaming = serializationNaming;
    }

    public override bool Applicable(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol, 
        string? fieldName,
        bool isInsideCollection)
    {
        if (!compilation.Customization.Overall.FilePerRecord) return false;
        if (compilation.Customization.TargetRecordSpecs?.EmbedRecordForProperty(fieldName) ?? false) return false;
        if (!base.Applicable(obj, compilation, typeSymbol, fieldName, isInsideCollection)) return false;
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        if (ShouldSkip(compilation.Customization.Overall, obj, null)) return false;
        return _isMajorRecordTester.IsMajorRecord(namedTypeSymbol.TypeArguments[0]);
    }

    public override void GenerateForSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string writerAccessor, string kernelAccessor,
        string metaAccessor, bool isInsideCollection, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (ShouldSkip(compilation.Customization.Overall, obj, fieldName)) return;
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
        }
    }

    public override void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public override void GenerateForDeserializeSingleFieldInto(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }

    public override void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (ShouldSkip(compilation.Customization.Overall, obj, fieldName)) return;
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
        }
    }
}