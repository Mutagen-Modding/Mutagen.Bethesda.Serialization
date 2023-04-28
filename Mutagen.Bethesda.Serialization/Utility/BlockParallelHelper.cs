using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task AddBlocksToWork<TKernel, TWriteObject, TGroup, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TGroup obj,
        string? fieldName,
        Func<TGroup, IEnumerable<TBlock>> blockRetriever,
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, int> blockNumberRetriever,
        Func<TSubBlock, int> subBlockNumberRetriever,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TGroup> metaWriter,
        WriteAsync<TKernel, TWriteObject, TBlock> blockWriter,
        WriteAsync<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        WriteAsync<TKernel, TWriteObject, TMajor> majorWriter,
        bool withNumbering)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));

        var groupDir = await WriteGroupRecordData(streamPackage: streamPackage,
            @group: obj,
            folderName: fieldName,
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: metaWriter);

        await metaData.WorkDropoff.EnqueueAndWait(
            blockRetriever(obj)
                .WithIndex(),
            async blockGetter =>
            {
                var blockDir = await WriteGroupRecordData(
                    streamPackage: streamPackage with { Stream = null!, Path = groupDir },
                    @group: blockGetter.Item,
                    folderName: DecorateWithNumber(
                        blockNumberRetriever(blockGetter.Item).ToString(), 
                        withNumbering ? blockGetter.Index : null),
                    fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                    metaData: metaData,
                    kernel: kernel,
                    groupWriter: blockWriter);

                await metaData.WorkDropoff.EnqueueAndWait(
                    subBlockRetriever(blockGetter.Item)
                        .WithIndex(),
                    async subBlockGetter =>
                    {
                        var subBlockDir = await WriteGroupRecordData(
                            streamPackage: streamPackage with { Stream = null!, Path = blockDir },
                            @group: subBlockGetter.Item,
                            folderName: DecorateWithNumber(
                                subBlockNumberRetriever(subBlockGetter.Item).ToString(),
                                withNumbering ? subBlockGetter.Index : null),
                            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                            metaData: metaData,
                            kernel: kernel,
                            groupWriter: subBlockWriter);

                        await metaData.WorkDropoff.EnqueueAndWait(
                            majorRetriever(subBlockGetter.Item).WithIndex(),
                            async recordGetter =>
                            {
                                var recordFolder = Path.Combine(subBlockDir, RecordNameProvider(
                                    recordGetter.Item,
                                    withNumbering ? recordGetter.Index : null));

                                metaData.FileSystem.Directory.CreateDirectory(recordFolder);
                                
                                var fileName = RecordDataFileName(kernel.ExpectedExtension);
                                var recordPath = Path.Combine(recordFolder, fileName);
                                await using var stream = metaData.FileSystem.File.Create(recordPath);
                                var recordStreamPackage = streamPackage with { Stream = stream, Path = recordFolder };
                                var recordWriter = kernel.GetNewObject(recordStreamPackage);
                                await majorWriter(recordWriter, recordGetter.Item, kernel, metaData);
                                kernel.Finalize(recordStreamPackage, recordWriter);
                            });
                    });
            });
    }

    private static ReadOnlySpan<char> TrimOrdering(ReadOnlySpan<char> str)
    {
        const string delimeter = "] ";
        var index = str.IndexOf(delimeter);
        if (index == -1) return str;
        return str.Slice(index + delimeter.Length);
    }

    public static async Task ReadFilePerRecordIntoBlocks<TKernel, TReadObject, TGroup, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TGroup group,
        string fieldName,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, int, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, int, IEnumerable<TMajor>> subBlockSet,
        ReadAsync<TKernel, TReadObject, TMajor> majorReader,
        Action<TGroup, IEnumerable<TBlock>> groupSetter)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TGroup : class, IClearable
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        streamPackage = streamPackage with { Path = Path.Combine(streamPackage.Path!, fieldName) };
        
        await ReadGroupHeaderPathToWork(streamPackage, group, metaData, kernel, groupReader);

        var groupFileName = TypicalGroupFileName(kernel.ExpectedExtension);

        var blocks = await metaData.WorkDropoff.EnqueueAndWait(
            metaData.FileSystem.Directory.GetDirectories(streamPackage.Path!),
            async blockDir =>
            {
                if (!int.TryParse(TrimOrdering(Path.GetFileName(blockDir.AsSpan())), out var blockNum))
                {
                    return default;
                }

                var blockStreamPackage = streamPackage with { Path = blockDir };

                var blockDataPath = Path.Combine(blockDir, groupFileName);
                TBlock block = new TBlock();
                if (metaData.FileSystem.File.Exists(blockDataPath))
                {
                    using (var blockStream = metaData.FileSystem.File.OpenRead(blockDataPath))
                    {
                        var reader = kernel.GetNewObject(blockStreamPackage with { Stream = blockStream });

                        while (kernel.TryGetNextField(reader, out var name))
                        {
                            await blockReader(reader, block, kernel, metaData, name);
                        }
                    }
                }

                var subBlocks = await metaData.WorkDropoff.EnqueueAndWait(
                    metaData.FileSystem.Directory.GetDirectories(blockDir),
                    async subBlockDir =>
                    {
                        if (!int.TryParse(TrimOrdering(Path.GetFileName(subBlockDir.AsSpan())), out var subBlockNum))
                        {
                            return default;
                        }

                        var subBlockDataPath = Path.Combine(subBlockDir, groupFileName);
                        var subBlockDataFileName = Path.GetFileName(subBlockDataPath);
                        var subBlockStreamPackage = blockStreamPackage with { Path = subBlockDir };

                        TSubBlock subBlock = new();
                        if (metaData.FileSystem.File.Exists(subBlockDataPath))
                        {
                            using (var subBlockStream = metaData.FileSystem.File.OpenRead(subBlockDataPath))
                            {
                                var reader = kernel.GetNewObject(subBlockStreamPackage with { Stream = subBlockStream });

                                while (kernel.TryGetNextField(reader, out var name))
                                {
                                    await subBlockReader(reader, subBlock, kernel, metaData, name);
                                }
                            }
                        }

                        var records = await metaData.WorkDropoff.EnqueueAndWait(
                            metaData.FileSystem.Directory.GetDirectories(subBlockDir),
                            async recordDir =>
                            {
                                var recordFile = Path.Combine(recordDir, RecordDataFileName(kernel.ExpectedExtension));
                                
                                using (var recordStream = metaData.FileSystem.File.OpenRead(recordFile))
                                {
                                    var reader = kernel.GetNewObject(subBlockStreamPackage with
                                    {
                                        Stream = recordStream,
                                        Path = recordDir
                                    });

                                    return (Path.GetFileName(recordDir), await majorReader(reader, kernel, metaData));
                                }
                            });

                        subBlockSet(
                            subBlock,
                            subBlockNum, 
                            records
                                .OrderBy(x => TryGetNumber(x.Item1))
                                .Select(x => x.Item2)
                                .NotNull());

                        return (Path.GetFileName(subBlockDir), subBlock);
                    });

                blockSet(
                    block,
                    blockNum,
                    subBlocks
                        .OrderBy(x => TryGetNumber(x.Item1))
                        .Select(x => x.subBlock)
                        .NotNull());

                return (Path.GetFileName(blockDir), block);
            });

        groupSetter(
            group, 
            blocks
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.block)
                .NotNull());
    }
}