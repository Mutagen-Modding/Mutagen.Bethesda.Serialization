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
        IsGroupTester groupTester) 
        : base(forFieldGenerator, groupTester)
    {
        _isMajorRecordTester = isMajorRecordTester;
        _nameRetriever = nameRetriever;
        _serializationNaming = serializationNaming;
    }

    public override bool Applicable(
        LoquiTypeSet obj,
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol)
    {
        if (!customization.FolderPerRecord) return false;
        if (!base.Applicable(obj, customization, typeSymbol)) return false;
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        if (ShouldSkip(customization, obj, null)) return false;
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

        using (var f = sb.Call($"SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, {subNames.Getter}>"))
        {
            f.Add($"streamPackage: {writerAccessor}.StreamPackage");
            f.Add($"list: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add($"itemWriter: static (w, i, k, m) => {subSerializationNames.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"toDo: parallelToDo");
        }
    }

    public override void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public override void GenerateForDeserialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (ShouldSkip(compilation.Customization.Overall, obj, fieldName)) return;
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;

        var subNames = _nameRetriever.GetNames(subType);

        if (!compilation.Mapping.TryGetTypeSet(subType, out var typeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(typeSet);
        
        using (var f = sb.Call(
                   $"SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, {subType}>"))
        {
            f.Add($"streamPackage: {readerAccessor}.StreamPackage");
            f.Add($"list: {fieldAccessor}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"itemReader: static (r, k, m) => k.ReadLoqui(r, m, {subSerializationNames.DeserializationCall(hasInheriting)}<TReadObject>)");
            f.Add($"toDo: parallelToDo");
        }
    }
}