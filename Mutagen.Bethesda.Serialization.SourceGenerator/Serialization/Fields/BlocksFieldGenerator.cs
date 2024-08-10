using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class BlocksFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiSerializationNaming _serializationNaming;
    private readonly LoquiNameRetriever _nameRetriever;
    
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol) => Enumerable.Empty<string>();

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public bool HasVariableHasSerialize => true;
    
    public BlocksFieldGenerator(
        LoquiSerializationNaming serializationNaming,
        LoquiNameRetriever nameRetriever)
    {
        _serializationNaming = serializationNaming;
        _nameRetriever = nameRetriever;
    }

    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationCatalog customization,
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (!customization.Overall.FilePerRecord) return false;
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeArguments.Length == 1)
        {
            var name = _nameRetriever.GetNames(typeSymbol.Name);
            if (name.Direct.EndsWith("Group")
                && IsBlockGroup(namedTypeSymbol.TypeArguments[0]))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsBlockGroup(ITypeSymbol obj)
    {
        switch (_nameRetriever.GetNames(obj).Direct)
        {
            case "CellBlock":
            case "CellSubBlock":
                return true;
            default:
                return false;
        }
    }

    private ITypeSymbol GetListTarget(ITypeSymbol typeSymbol, out string propName)
    {
        foreach (var memb in typeSymbol.GetMembers())
        {
            if (memb.DeclaredAccessibility != Accessibility.Public) continue;
            if (memb is not IPropertySymbol prop) continue;
            if (prop.Type is not INamedTypeSymbol namedTypeSymbol) continue;
            if (namedTypeSymbol.TypeArguments.Length != 1) continue;

            propName = prop.Name;
            return namedTypeSymbol.TypeArguments[0];
        }

        throw new ArgumentException();
    }

    record BlockInfoLine(
        ITypeSymbol Symbol,
        SerializationItems SerializationItems,
        string ListName,
        Names Names);

    record BlockInfo(
        BlockInfoLine Group,
        BlockInfoLine Block,
        BlockInfoLine SubBlock,
        BlockInfoLine Record);

    private BlockInfo GetBlockInfo(INamedTypeSymbol field)
    {
        var blockType = GetListTarget(field, out var groupListName);
        var subBlockType = GetListTarget(blockType, out var blockListName);
        var recordType = GetListTarget(subBlockType, out var subBlockListName);
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) throw new ArgumentException();
        if (!_serializationNaming.TryGetSerializationItems(blockType, out var blockSerializationNames)) throw new ArgumentException();
        if (!_serializationNaming.TryGetSerializationItems(subBlockType, out var subBlockSerializationNames)) throw new ArgumentException();
        if (!_serializationNaming.TryGetSerializationItems(recordType, out var recordSerializationNames)) throw new ArgumentException();

        var groupNames = _nameRetriever.GetNames(field);
        var blockNames = _nameRetriever.GetNames(blockType);
        var subBlockNames = _nameRetriever.GetNames(subBlockType);
        var recordNames = _nameRetriever.GetNames(recordType);
        
        return new BlockInfo(
            new BlockInfoLine(
                field,
                fieldSerializationNames,
                groupListName,
                groupNames),
            new BlockInfoLine(
                blockType,
                blockSerializationNames,
                blockListName,
                blockNames),
            new BlockInfoLine(
                subBlockType,
                subBlockSerializationNames,
                subBlockListName,
                subBlockNames),
            new BlockInfoLine(
                recordType,
                recordSerializationNames,
                string.Empty,
                recordNames));
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
        var blockInfo = GetBlockInfo(namedTypeSymbol);

        if (!compilation.Mapping.TryGetTypeSet(blockInfo.Record.Symbol, out var recordTypeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(recordTypeSet);

        using (var f = sb.Call($"tasks.Add(SerializationHelper.AddBlocksToWork<TKernel, TWriteObject, {blockInfo.Group.Names.Getter}<{blockInfo.Block.Names.Getter}>, {blockInfo.Block.Names.Getter}, {blockInfo.SubBlock.Names.Getter}, {blockInfo.Record.Names.Getter}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: {writerAccessor}.StreamPackage");
            f.Add($"obj: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add($"blockRetriever: static x => x.{blockInfo.Group.ListName}");
            f.Add($"subBlockRetriever: static x => x.{blockInfo.Block.ListName}");
            f.Add($"majorRetriever: static x => x.{blockInfo.SubBlock.ListName}");
            f.Add($"blockNumberRetriever: static x => x.BlockNumber");
            f.Add($"subBlockNumberRetriever: static x => x.BlockNumber");
            f.Add($"metaWriter: static (w, i, k, m) => {blockInfo.Group.SerializationItems.SerializationCall()}<TKernel, TWriteObject, {blockInfo.Block.Names.Getter}>(w, i, k, m)");
            f.Add($"metaHasSerialization: static (i, m) => {blockInfo.Group.SerializationItems.HasSerializationCall()}<{blockInfo.Block.Names.Getter}>(i, m)");
            f.Add($"blockWriter: static (w, i, k, m) => {blockInfo.Block.SerializationItems.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"blockHasSerialization: static (i, m) => {blockInfo.Block.SerializationItems.HasSerializationCall()}(i, m)");
            f.Add($"subBlockWriter: static (w, i, k, m) => {blockInfo.SubBlock.SerializationItems.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"subBlockHasSerialization: static (i, m) => {blockInfo.SubBlock.SerializationItems.HasSerializationCall()}(i, m)");
            f.Add($"majorWriter: static (w, i, k, m) => {blockInfo.Record.SerializationItems.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"withNumbering: {compilation.Customization.Overall.EnforceRecordOrder.ToString().ToLower()}");
        }
    }

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException($"No GetDefault defined for {typeof(BlocksFieldGenerator)}");
    }

    public void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public void GenerateForDeserializeSingleFieldInto(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var blockInfo = GetBlockInfo(namedTypeSymbol);
        
        if (!compilation.Mapping.TryGetTypeSet(blockInfo.Record.Symbol, out var recordTypeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(recordTypeSet);

        using (var f = sb.Call(
                   $"tasks.Add(SerializationHelper.ReadFilePerRecordIntoBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, {blockInfo.Group.Names.Setter}<{blockInfo.Block.Names.Direct}>, {blockInfo.Block.Names.Direct}, {blockInfo.SubBlock.Names.Direct}, {blockInfo.Record.Names.Direct}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: {readerAccessor}.StreamPackage");
            f.Add($"group: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"groupReader: static (r, i, k, m, n) => {blockInfo.Group.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject, {blockInfo.Block.Names.Direct}>(r, k, i, m, n)");
            f.Add(
                $"blockReader: static (r, i, k, m, n) => {blockInfo.Block.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"subBlockReader: static (r, i, k, m, n) => {blockInfo.SubBlock.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"majorReader: static async (r, k, m) => (await k.ReadLoqui(r, m, {blockInfo.Record.SerializationItems.DeserializationCall(hasInheriting)}<TReadObject>)).StripNull(\"{fieldName}\")");
            f.Add(subSb =>
            {
                subSb.AppendLine("groupSetter: static (b, sub) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine($"b.{blockInfo.Group.ListName}.SetTo(sub);");
                }
            });
            f.Add(subSb =>
            {
                subSb.AppendLine("blockSet: static (b, i, sub) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine("b.BlockNumber = i;");
                    subSb.AppendLine($"b.{blockInfo.Block.ListName}.SetTo(sub);");
                }
            });
            f.Add(subSb =>
            {
                subSb.AppendLine("subBlockSet: static (b, i, sub) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine("b.BlockNumber = i;");
                    subSb.AppendLine($"b.{blockInfo.SubBlock.ListName}.SetTo(sub);");
                }
            });
        }
    }
}