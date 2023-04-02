using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class BlocksXYFieldGenerator : ISerializationForFieldGenerator
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

    public BlocksXYFieldGenerator(
        LoquiSerializationNaming serializationNaming,
        LoquiNameRetriever nameRetriever)
    {
        _serializationNaming = serializationNaming;
        _nameRetriever = nameRetriever;
    }

    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol)
    {
        if (!customization.FolderPerRecord) return false;
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeArguments.Length == 1)
        {
            var name = _nameRetriever.GetNames(typeSymbol.Name);
            if (ListFieldGenerator.ListNameStrings.Contains(name.Direct)
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
            case "WorldspaceBlock":
                return true;
            default:
                return false;
        }
    }

    public IPropertySymbol GetListProperty(ITypeSymbol typeSymbol)
    {
        foreach (var memb in typeSymbol.GetMembers())
        {
            if (memb.DeclaredAccessibility != Accessibility.Public) continue;
            if (memb is not IPropertySymbol prop) continue;
            if (prop.Type is not INamedTypeSymbol namedTypeSymbol) continue;
            if (namedTypeSymbol.TypeArguments.Length != 1) continue;
            if (!namedTypeSymbol.TypeArguments[0].Name.Contains("Block")) continue;
            return prop;
        }

        throw new ArgumentException();
    }

    public ITypeSymbol GetListTarget(ITypeSymbol typeSymbol, out string propName)
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

    public record BlockInfoLine(
        ITypeSymbol Symbol,
        SerializationItems SerializationItems,
        string ListName,
        Names Names);

    public record BlockInfo(
        BlockInfoLine Group,
        BlockInfoLine Block,
        BlockInfoLine SubBlock,
        BlockInfoLine Record);

    public BlockInfo GetBlockInfo(INamedTypeSymbol field)
    {
        var blockType = field.TypeArguments[0];
        var subBlockType = GetListTarget(blockType, out var blockListName);
        var recordType = GetListTarget(subBlockType, out var subBlockListName);
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) throw new ArgumentException();
        if (!_serializationNaming.TryGetSerializationItems(blockType, out var blockSerializationNames)) throw new ArgumentException();
        if (!_serializationNaming.TryGetSerializationItems(subBlockType, out var subBlockSerializationNames)) throw new ArgumentException();
        if (!_serializationNaming.TryGetSerializationItems(recordType, out var recordSerializationNames)) throw new ArgumentException();

        var blockNames = _nameRetriever.GetNames(blockType);
        var subBlockNames = _nameRetriever.GetNames(subBlockType);
        var recordNames = _nameRetriever.GetNames(recordType);
        
        return new BlockInfo(
            new BlockInfoLine(
                field,
                fieldSerializationNames,
                string.Empty,
                new Names(
                    "ExtendedList",
                    "IReadOnlyList",
                    "IExtendedList")),
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
        // Handled by interceptor
    }

    public void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public void GenerateForDeserialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field,
        string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        // Handled by interceptor
    }

    public void GenerateDeserialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field,
        string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var blockInfo = GetBlockInfo(namedTypeSymbol);

        if (!compilation.Mapping.TryGetTypeSet(blockInfo.Record.Symbol, out var recordTypeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(recordTypeSet);

        using (var f = sb.Call(
                   $"SerializationHelper.ReadFolderPerRecordIntoXYBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, {blockInfo.Group.Names.Setter}<{blockInfo.Block.Names.Direct}>, {blockInfo.Block.Names.Direct}, {blockInfo.SubBlock.Names.Direct}, {blockInfo.Record.Names.Direct}>"))
        {
            f.Add($"streamPackage: {readerAccessor}.StreamPackage");
            f.Add($"obj: {fieldAccessor}");
            f.Add($"metaData: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add(
                $"blockReader: static (r, i, k, m, n) => {blockInfo.Block.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"subBlockReader: static (r, i, k, m, n) => {blockInfo.SubBlock.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"majorReader: static (r, k, m) => k.ReadLoqui(r, m, {blockInfo.Record.SerializationItems.DeserializationCall(hasInheriting)}<TReadObject>)");
            f.Add(subSb =>
            {
                subSb.AppendLine("blockSet: static (b, i, sub) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine("b.BlockNumberX = i.X;");
                    subSb.AppendLine("b.BlockNumberY = i.Y;");
                    subSb.AppendLine($"b.{blockInfo.Block.ListName}.SetTo(sub);");
                }
            });
            f.Add(subSb =>
            {
                subSb.AppendLine("subBlockSet: static (b, i, sub) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine("b.BlockNumberX = i.X;");
                    subSb.AppendLine("b.BlockNumberY = i.Y;");
                    subSb.AppendLine($"b.{blockInfo.SubBlock.ListName}.SetTo(sub);");
                }
            });
            f.Add($"toDo: parallelToDo");
        }
    }
}