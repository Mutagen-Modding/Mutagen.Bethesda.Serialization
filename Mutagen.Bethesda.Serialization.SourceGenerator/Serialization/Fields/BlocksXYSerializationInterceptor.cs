using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class BlocksXYSerializationInterceptor : ISerializationInterceptor
{
    private readonly BlocksXYFieldGenerator _blocksXyFieldGenerator;
    private readonly LoquiSerializationNaming _serializationNaming;

    public BlocksXYSerializationInterceptor(
        BlocksXYFieldGenerator blocksXyFieldGenerator,
        LoquiSerializationNaming serializationNaming)
    {
        _blocksXyFieldGenerator = blocksXyFieldGenerator;
        _serializationNaming = serializationNaming;
    }
    
    public bool Applicable(CompilationUnit compilation, LoquiTypeSet typeSet)
    {
        if (!compilation.Customization.Overall.FolderPerRecord) return false;
        if (typeSet.Direct == null) return false;
        foreach (var prop in typeSet.Direct.GetMembers()
                     .OfType<IPropertySymbol>())
        {
            if (_blocksXyFieldGenerator.Applicable(typeSet, compilation.Customization.Overall, prop.Type))
            {
                return true;
            }
        }

        return false;
    }

    public void GenerateSerialize(CompilationUnit compilation,
        LoquiTypeSet typeSet, 
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties, 
        SerializationGenerics generics)
    {
        if (!_serializationNaming.TryGetSerializationItems(typeSet.Getter, out var naming)) return;
        var genString = generics.WriterGenericsString(forHas: false);
        
        var listTarget = _blocksXyFieldGenerator.GetListProperty(typeSet.Getter);
        if (listTarget.Type is not INamedTypeSymbol namedListType)
        {
            throw new NullReferenceException();
        }
        
        using (var args = sb.Function($"public static void Serialize{genString}"))
        {
            args.Add($"TWriteObject writer");
            args.Add($"{typeSet.Getter} item");
            args.Add($"MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel");
            args.Add("SerializationMetaData metaData");
            args.Wheres.AddRange(generics.WriterWheres(forHas: false));
            args.Wheres.Add("where TWriteObject : IContainStreamPackage");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("List<Action> parallelToDo = new List<Action>();");

            var blockInfo = _blocksXyFieldGenerator.GetBlockInfo(namedListType);
            if (!compilation.Mapping.TryGetTypeSet(blockInfo.Record.Symbol, out var recordTypeSet))
            {
                throw new ArgumentException();
            }
            var hasInheriting = compilation.Mapping.HasInheritingClasses(recordTypeSet);

            using (var f = sb.Call($"SerializationHelper.AddXYBlocksToWork<TKernel, TWriteObject, {typeSet.Getter}, {blockInfo.Block.Names.Getter}, {blockInfo.SubBlock.Names.Getter}, {blockInfo.Record.Names.Getter}>"))
            {
                f.Add($"streamPackage: writer.StreamPackage");
                f.Add($"obj: item");
                f.Add($"fieldName: \"{listTarget.Name}\"");
                f.AddPassArg($"metaData");
                f.AddPassArg($"kernel");
                f.Add($"blockRetriever: x => x.{listTarget.Name}");
                f.Add($"subBlockRetriever: x => x.{blockInfo.Block.ListName}");
                f.Add($"majorRetriever: x => x.{blockInfo.SubBlock.ListName}");
                f.Add($"blockNumberRetriever: x => new P2Int16(x.BlockNumberX, x.BlockNumberY)");
                f.Add($"subBlockNumberRetriever: x => new P2Int16(x.BlockNumberX, x.BlockNumberY)");
                f.Add($"metaWriter: static (w, i, k, m) => {naming.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
                f.Add($"blockWriter: static (w, i, k, m) => {blockInfo.Block.SerializationItems.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
                f.Add($"subBlockWriter: static (w, i, k, m) => {blockInfo.SubBlock.SerializationItems.SerializationCall()}<TKernel, TWriteObject>(w, i, k, m)");
                f.Add($"majorWriter: static (w, i, k, m) => {blockInfo.Record.SerializationItems.SerializationCall(withCheck: hasInheriting)}<TKernel, TWriteObject>(w, i, k, m)");
                f.Add($"toDo: parallelToDo");
            }
            
            sb.AppendLine("Parallel.Invoke(parallelToDo.ToArray());");
        }
        sb.AppendLine();
    }

    public void GenerateDeserialize(CompilationUnit compilation, LoquiTypeSet typeSet, StructuredStringBuilder sb,
        ITypeSymbol? baseType, PropertyCollection properties, SerializationGenerics generics)
    {
        if (!_serializationNaming.TryGetSerializationItems(typeSet.Getter, out var naming)) return;
        var listTarget = _blocksXyFieldGenerator.GetListProperty(typeSet.Getter);
        if (listTarget.Type is not INamedTypeSymbol namedListType)
        {
            throw new NullReferenceException();
        }
        
        sb.AppendLine("List<Action> parallelToDo = new List<Action>();");
        
        var blockInfo = _blocksXyFieldGenerator.GetBlockInfo(namedListType);

        if (!compilation.Mapping.TryGetTypeSet(blockInfo.Record.Symbol, out var recordTypeSet)) return;
        var hasInheriting = compilation.Mapping.HasInheritingClasses(recordTypeSet);

        using (var f = sb.Call(
                   $"SerializationHelper.ReadFolderPerRecordIntoXYBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, {typeSet.Setter}, {blockInfo.Block.Names.Direct}, {blockInfo.SubBlock.Names.Direct}, {blockInfo.Record.Names.Direct}>"))
        {
            f.Add($"streamPackage: reader.StreamPackage");
            f.AddPassArg($"obj");
            f.AddPassArg($"metaData");
            f.AddPassArg($"kernel");
            f.Add(
                $"objReader: static (r, i, k, m, n) => SerializationHelper.DeserializeAllFieldsInto<TReadObject, {typeSet.Setter}>(r, k, i, m, {naming.DeserializationSingleFieldIntoCall()})");
            f.Add(
                $"blockReader: static (r, i, k, m, n) => {blockInfo.Block.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"subBlockReader: static (r, i, k, m, n) => {blockInfo.SubBlock.SerializationItems.DeserializationSingleFieldIntoCall()}<TReadObject>(r, k, i, m, n)");
            f.Add(
                $"majorReader: static (r, k, m) => k.ReadLoqui(r, m, {blockInfo.Record.SerializationItems.DeserializationCall(hasInheriting)}<TReadObject>)");
            f.Add(subSb =>
            {
                subSb.AppendLine("groupSetter: static (b, items) =>");
                using (subSb.CurlyBrace())
                {
                    subSb.AppendLine($"b.{listTarget.Name}.SetTo(items);");
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
            f.Add($"toDo: parallelToDo");
        }
        
        sb.AppendLine("Parallel.Invoke(parallelToDo.ToArray());");
    }
}