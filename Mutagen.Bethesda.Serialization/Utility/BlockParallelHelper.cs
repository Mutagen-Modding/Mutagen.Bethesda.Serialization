using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static void AddBlocksToWork<TKernel, TWriteObject, TGroup, TBlock, TSubBlock, TMajor>(
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
        Write<TKernel, TWriteObject, TGroup> metaWriter,
        Write<TKernel, TWriteObject, TBlock> blockWriter,
        Write<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        Write<TKernel, TWriteObject, TMajor> majorWriter,
        bool withNumbering,
        List<Action> toDo)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));

        var groupDir = WriteGroupRecordData(streamPackage: streamPackage,
            @group: obj,
            folderName: fieldName,
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: metaWriter,
            toDo: toDo);

        int? blockNumber = withNumbering ? 1 : null;
        foreach (var blockGetter in blockRetriever(obj))
        {
            var blockDir = WriteGroupRecordData(
                streamPackage: streamPackage with { Stream = null!, Path = groupDir },
                @group: blockGetter,
                folderName: DecorateWithNumber(blockNumberRetriever(blockGetter).ToString(), blockNumber++),
                fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                metaData: metaData,
                kernel: kernel,
                groupWriter: blockWriter,
                toDo: toDo);

            int? subBlockNumber = withNumbering ? 1 : null;
            foreach (var subBlockGetter in subBlockRetriever(blockGetter))
            {
                var subBlockDir = WriteGroupRecordData(
                    streamPackage: streamPackage with { Stream = null!, Path = blockDir },
                    @group: subBlockGetter,
                    folderName: DecorateWithNumber(subBlockNumberRetriever(subBlockGetter).ToString(), subBlockNumber++),
                    fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                    metaData: metaData,
                    kernel: kernel,
                    groupWriter: subBlockWriter,
                    toDo: toDo);

                int? recordI = withNumbering ? 1 : null;
                foreach (var recordGetter in majorRetriever(subBlockGetter))
                {
                    int? recordNumber = recordI;
                    toDo.Add(() =>
                    {
                        WriteMajor(
                            streamPackage with { Stream = null!, Path = subBlockDir },
                            metaData, kernel, majorWriter, recordGetter,
                            numbering: recordNumber);
                    });
                    recordI++;
                }
            }
        }
    }

    private static ReadOnlySpan<char> TrimOrdering(ReadOnlySpan<char> str)
    {
        const string delimeter = "] ";
        var index = str.IndexOf(delimeter);
        if (index == -1) return str;
        return str.Slice(index + delimeter.Length);
    }

    public static void ReadFilePerRecordIntoBlocks<TKernel, TReadObject, TGroup, TBlock, TSubBlock, TMajor>(
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
        Read<TKernel, TReadObject, TMajor> majorReader,
        Action<TGroup, IEnumerable<TBlock>> groupSetter,
        List<Action> toDo)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TGroup : class, IClearable
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        streamPackage = streamPackage with { Path = Path.Combine(streamPackage.Path!, fieldName) };
        
        ReadGroupHeaderPathToWork(streamPackage, group, metaData, kernel, groupReader, toDo);

        var groupFileName = TypicalGroupFileName(kernel.ExpectedExtension);

        toDo.Add(() =>
        {
            var blocks = streamPackage.FileSystem.Directory.GetDirectories(streamPackage.Path!)
                .AsParallel()
                .Select(blockDir =>
                {
                    if (!int.TryParse(TrimOrdering(Path.GetFileName(blockDir.AsSpan())), out var blockNum))
                    {
                        return default;
                    }

                    var blockDataPath = Path.Combine(blockDir, groupFileName);
                    TBlock block = new TBlock();
                    if (streamPackage.FileSystem.File.Exists(blockDataPath))
                    {
                        using (var blockStream = streamPackage.FileSystem.File.OpenRead(blockDataPath))
                        {
                            var reader = kernel.GetNewObject(streamPackage with { Stream = blockStream });

                            while (kernel.TryGetNextField(reader, out var name))
                            {
                                blockReader(reader, block, kernel, metaData, name);
                            }
                        }
                    }

                    var subBlocks = streamPackage.FileSystem.Directory.GetDirectories(blockDir)
                        .AsParallel()
                        .Select(subBlockDir =>
                        {
                            if (!int.TryParse(TrimOrdering(Path.GetFileName(subBlockDir.AsSpan())), out var subBlockNum))
                            {
                                return default;
                            }

                            var subBlockDataPath = Path.Combine(subBlockDir, groupFileName);
                            var subBlockDataFileName = Path.GetFileName(subBlockDataPath);

                            TSubBlock subBlock = new();
                            if (streamPackage.FileSystem.File.Exists(subBlockDataPath))
                            {
                                using (var subBlockStream = streamPackage.FileSystem.File.OpenRead(subBlockDataPath))
                                {
                                    var reader = kernel.GetNewObject(streamPackage with { Stream = subBlockStream });

                                    while (kernel.TryGetNextField(reader, out var name))
                                    {
                                        subBlockReader(reader, subBlock, kernel, metaData, name);
                                    }
                                }
                            }

                            var records = streamPackage.FileSystem.Directory.GetFiles(subBlockDir)
                                .Where(x => !subBlockDataFileName.AsSpan().Equals(Path.GetFileName(x.AsSpan()), StringComparison.OrdinalIgnoreCase))
                                .Where(x => !x.Equals(subBlockDataPath, StringComparison.OrdinalIgnoreCase))
                                .AsParallel()
                                .Select(recordFile =>
                                {
                                    using (var recordStream = streamPackage.FileSystem.File.OpenRead(recordFile))
                                    {
                                        var reader = kernel.GetNewObject(streamPackage with { Stream = recordStream });

                                        return (Path.GetFileName(recordFile), majorReader(reader, kernel, metaData));
                                    }
                                })
                                .ToArray()
                                .OrderBy(x => TryGetNumber(x.Item1))
                                .Select(x => x.Item2)
                                .NotNull();

                            subBlockSet(subBlock, subBlockNum, records);

                            return (Path.GetFileName(subBlockDir), subBlock);
                        })
                        .ToArray()
                        .OrderBy(x => TryGetNumber(x.Item1))
                        .Select(x => x.subBlock)
                        .NotNull();

                    blockSet(block, blockNum, subBlocks);

                    return (Path.GetFileName(blockDir), block);
                })
                .ToArray()
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.block)
                .NotNull();
            groupSetter(group, blocks);
        });
    }
}