using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class BlocksXYFieldMemberBlocker : AListFieldGenerator
{
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly IsListTester _listTester;

    public BlocksXYFieldMemberBlocker(
        LoquiNameRetriever nameRetriever,
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator,
        IsListTester listTester,
        IsGroupTester groupTester) 
        : base(forFieldGenerator, listTester, groupTester)
    {
        _nameRetriever = nameRetriever;
        _listTester = listTester;
    }

    public override bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (!customization.FilePerRecord) return false;
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeArguments.Length == 1)
        {
            var name = _nameRetriever.GetNames(typeSymbol.Name);
            if (_listTester.ListNameStrings.Contains(name.Direct)
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

    public override void GenerateForSerialize(
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

    public override void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public override void GenerateForDeserializeSingleFieldInto(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field,
        string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        // Handled by interceptor
    }
}

public class BlocksXYFieldGenerator : AListFieldGenerator
{
    private readonly LoquiSerializationNaming _serializationNaming;
    private readonly LoquiNameRetriever _nameRetriever;

    public BlocksXYFieldGenerator(
        LoquiSerializationNaming serializationNaming,
        LoquiNameRetriever nameRetriever,
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator,
        IsListTester listTester,
        IsGroupTester groupTester) 
        : base(forFieldGenerator, listTester, groupTester)
    {
        _serializationNaming = serializationNaming;
        _nameRetriever = nameRetriever;
    }

    public override bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customizations,
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (!customizations.FilePerRecord) return false;
        if (!GroupTester.IsGroup(typeSymbol)) return false;
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var subType = namedTypeSymbol.TypeArguments[0];
        return subType.Name.Contains("Worldspace")
            && !subType.Name.Contains("Block");
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

    public override void GenerateForSerialize(
        CompilationUnit compilation,
        LoquiTypeSet typeSet,
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
        if (field is not INamedTypeSymbol namedField) return;

        var subType = namedField.TypeArguments[0];
        var groupNames = _nameRetriever.GetNames(field);
        var subNames = _nameRetriever.GetNames(subType);
        
        var listTarget = GetListProperty(subType);
        if (listTarget.Type is not INamedTypeSymbol namedListType)
        {
            throw new NullReferenceException();
        }
        
        if (!_serializationNaming.TryGetSerializationItems(subType, out var naming)) return;
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;

        var blockInfo = GetBlockInfo(namedListType);
        if (!compilation.Mapping.TryGetTypeSet(blockInfo.Record.Symbol, out var recordTypeSet))
        {
            throw new ArgumentException();
        }
        var hasInheriting = compilation.Mapping.HasInheritingClasses(recordTypeSet);

        using (var f = sb.Call($"tasks.Add(SerializationHelper.AddXYBlocksToWork<TKernel, TWriteObject, {groupNames.Getter}<{subNames.Getter}>, {subNames.Getter}, {blockInfo.Block.Names.Getter}, {blockInfo.SubBlock.Names.Getter}, {blockInfo.Record.Names.Getter}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: writer.StreamPackage");
            f.Add($"group: {fieldAccessor}");
            f.Add($"fieldName: \"{fieldName}\"");
            f.Add($"topRecordRetriever: static x => x.Records");
            f.Add($"blockRetriever: static x => x.{listTarget.Name}");
            f.Add($"subBlockRetriever: static x => x.{blockInfo.Block.ListName}");
            f.Add($"majorRetriever: static x => x.{blockInfo.SubBlock.ListName}");
            f.Add($"blockNumberRetriever: static x => new P2Int16(x.BlockNumberX, x.BlockNumberY)");
            f.Add($"subBlockNumberRetriever: static x => new P2Int16(x.BlockNumberX, x.BlockNumberY)");
            f.AddPassArg($"metaData");
            f.AddPassArg($"kernel");
            f.Add($"groupWriter: static (w, i, k, m) => {fieldSerializationNames.SerializationCall()}<TKernel, TWriteObject, {subNames.Getter}>(w, i, k, m)");
            f.Add($"groupHasSerialization: static (i, m) => {fieldSerializationNames.HasSerializationCall()}<{subNames.Getter}>(i, m)");
            f.Add($"topRecordWriter: static (w, i, k, m) => {naming.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"topRecordHasSerialization: static (i, m) => {naming.HasSerializationCall()}(i, m)");
            f.Add($"blockWriter: static (w, i, k, m) => {blockInfo.Block.SerializationItems.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"blockHasSerialization: static (i, m) => {blockInfo.Block.SerializationItems.HasSerializationCall()}(i, m)");
            f.Add($"subBlockWriter: static (w, i, k, m) => {blockInfo.SubBlock.SerializationItems.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"subBlockHasSerialization: static (i, m) => {blockInfo.SubBlock.SerializationItems.HasSerializationCall()}(i, m)");
            f.Add($"majorWriter: static (w, i, k, m) => {blockInfo.Record.SerializationItems.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
            f.Add($"withNumbering: {compilation.Customization.Overall.EnforceRecordOrder.ToString().ToLower()}");
        }
    }

    public override void GenerateForHasSerialize(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string? defaultValueAccessor, string metaAccessor, StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public override void GenerateForDeserializeSingleFieldInto(
        CompilationUnit compilation, LoquiTypeSet typeSet, ITypeSymbol field,
        string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }

    public override void GenerateForDeserializeSection(
        CompilationUnit compilation, LoquiTypeSet typeSet, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
        if (field is not INamedTypeSymbol namedField) return;

        var subType = namedField.TypeArguments[0];
        var groupNames = _nameRetriever.GetNames(field);
        var subNames = _nameRetriever.GetNames(subType);
        
        var listTarget = GetListProperty(subType);
        if (listTarget.Type is not INamedTypeSymbol namedListType)
        {
            throw new NullReferenceException();
        }
        
        if (!_serializationNaming.TryGetSerializationItems(subType, out var naming)) return;
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;

        var blockInfo = GetBlockInfo(namedListType);
        if (!compilation.Mapping.TryGetTypeSet(blockInfo.Record.Symbol, out var recordTypeSet))
        {
            throw new ArgumentException();
        }
        var hasInheriting = compilation.Mapping.HasInheritingClasses(recordTypeSet);

        
        using (var f = sb.Call(
                   $"tasks.Add(SerializationHelper.ReadIntoXYBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, {groupNames.Direct}<{subNames.Direct}>, {subNames.Direct}, {blockInfo.Block.Names.Direct}, {blockInfo.SubBlock.Names.Direct}, {blockInfo.Record.Names.Direct}>",
                   suffixLine: ")"))
        {
            f.Add($"streamPackage: reader.StreamPackage");
            f.Add($"group: {fieldAccessor}");
            f.Add($"fieldName: \"{fieldName}\"");
            f.AddPassArg($"metaData");
            f.AddPassArg($"kernel");
            f.Add(
                $"groupReader: static (r, i, k, m, n) => {fieldSerializationNames.DeserializationSingleFieldIntoCall()}<TReadObject, {subNames.Direct}>(r, k, i, m, n)");
            f.Add(
                $"objReader: static async (r, k, m) => (await k.ReadLoqui(r, m, {naming.DeserializationCall()}<TReadObject>)).StripNull(\"{fieldName}\")");
            f.Add(
                $"blockReader: static (r, i, k, m, n) => {blockInfo.Block.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"subBlockReader: static (r, i, k, m, n) => {blockInfo.SubBlock.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"majorReader: static async (r, k, m) => (await k.ReadLoqui(r, m, {blockInfo.Record.SerializationItems.DeserializationCall(hasInheriting)}<TReadObject>)).StripNull(\"{fieldName}\")");
            f.Add(subSb =>
            {
                subSb.AppendLine("groupSetter: static (b, items) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine($"b.RecordCache.SetTo(items);");
                }
            });
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
            f.Add(subSb =>
            {
                subSb.AppendLine("topRecordSetter: static (b, sub) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine($"b.{listTarget.Name}.SetTo(sub);");
                }
            });
        }
    }
}