using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static void AddXYBlocksToWork<TKernel, TWriteObject, TGroup, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TGroup group,
        string? fieldName,
        Func<TGroup, IEnumerable<TObject>> topRecordRetriever,
        Func<TObject, IEnumerable<TBlock>> blockRetriever,
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, P2Int16> blockNumberRetriever,
        Func<TSubBlock, P2Int16> subBlockNumberRetriever,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        Write<TKernel, TWriteObject, TObject> topRecordWriter,
        Write<TKernel, TWriteObject, TBlock> blockWriter,
        Write<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        Write<TKernel, TWriteObject, TMajor> majorWriter,
        bool withNumbering,
        List<Action> toDo)
        where TObject : class, IMajorRecordGetter
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));
        
        var groupDir = WriteGroupRecordData(streamPackage: streamPackage,
            @group: group,
            folderName: fieldName,
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: groupWriter,
            toDo: toDo);

        streamPackage = streamPackage with { Stream = null!, Path = groupDir };

        int? i = withNumbering ? 1 : null;
        foreach (var topRecord in topRecordRetriever(group))
        {
            WriteTopRecord(
                streamPackage, blockRetriever, subBlockRetriever, majorRetriever,
                blockNumberRetriever, subBlockNumberRetriever, metaData, kernel,
                topRecordWriter, blockWriter, subBlockWriter, majorWriter, i++,
                withNumbering, toDo, topRecord);
        }
    }

    private static void WriteTopRecord<TKernel, TWriteObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        Func<TObject, IEnumerable<TBlock>> blockRetriever,
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, P2Int16> blockNumberRetriever,
        Func<TSubBlock, P2Int16> subBlockNumberRetriever,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TObject> topRecordWriter,
        Write<TKernel, TWriteObject, TBlock> blockWriter,
        Write<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        Write<TKernel, TWriteObject, TMajor> majorWriter,
        int? number,
        bool withNumbering,
        List<Action> toDo,
        TObject topRecord) where TObject : class, IMajorRecordGetter
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        var folderName = FileNameProvider(topRecord, string.Empty, number);

        var topRecordDir = WriteGroupRecordData(
            streamPackage: streamPackage,
            group: topRecord,
            folderName: folderName,
            fileName: RecordFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: topRecordWriter,
            toDo: toDo);

        streamPackage = streamPackage with { Stream = null!, Path = topRecordDir };

        int? i = withNumbering ? 1 : null;
        foreach (var blockGetter in blockRetriever(topRecord))
        {
            WriteBlock(
                streamPackage, subBlockRetriever, majorRetriever, blockNumberRetriever,
                subBlockNumberRetriever, metaData, kernel, blockWriter, subBlockWriter,
                majorWriter, i++, withNumbering, toDo, blockGetter);
        }
    }

    private static void WriteBlock<TKernel, TWriteObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage, 
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, P2Int16> blockNumberRetriever,
        Func<TSubBlock, P2Int16> subBlockNumberRetriever, 
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TBlock> blockWriter,
        Write<TKernel, TWriteObject, TSubBlock> subBlockWriter, 
        Write<TKernel, TWriteObject, TMajor> majorWriter,
        int? number,
        bool withNumbering,
        List<Action> toDo,
        TBlock blockGetter)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        var blockNum = blockNumberRetriever(blockGetter);
        var blockDir = WriteGroupRecordData(
            streamPackage: streamPackage,
            group: blockGetter,
            folderName: DecorateWithNumber($"{blockNum.X}, {blockNum.Y}", number),
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: blockWriter,
            toDo: toDo);

        streamPackage = streamPackage with { Stream = null!, Path = blockDir };

        int? i = withNumbering ? 1 : null;
        foreach (var subBlockGetter in subBlockRetriever(blockGetter))
        {
            WriteSubBlock(
                streamPackage, majorRetriever, subBlockNumberRetriever,
                metaData, kernel, subBlockWriter, i++, 
                withNumbering, majorWriter, toDo, subBlockGetter);
        }
    }

    private static void WriteSubBlock<TKernel, TWriteObject, TSubBlock, TMajor>(
        StreamPackage streamPackage, 
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever, 
        Func<TSubBlock, P2Int16> subBlockNumberRetriever, SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        int? number,
        bool withNumbering,
        Write<TKernel, TWriteObject, TMajor> majorWriter, List<Action> toDo,
        TSubBlock subBlockGetter) 
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        var subBlockNum = subBlockNumberRetriever(subBlockGetter);
        var subBlockDir = WriteGroupRecordData(
            streamPackage: streamPackage,
            group: subBlockGetter,
            folderName: DecorateWithNumber($"{subBlockNum.X}, {subBlockNum.Y}", number),
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: subBlockWriter,
            toDo: toDo);

        int? i = withNumbering ? 1 : null;
        foreach (var recordGetter in majorRetriever(subBlockGetter))
        {
            int? recordNumber = i;
            toDo.Add(() =>
            {
                WriteMajor(
                    streamPackage with { Stream = null!, Path = subBlockDir },
                    metaData, kernel, majorWriter, recordGetter,
                    numbering: recordNumber);
            });
            i++;
        }
    }
    
    public static void ReadIntoXYBlocks<TKernel, TReadObject, TGroup, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TGroup group,
        string fieldName,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> objReader,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, P2Int16, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        Read<TKernel, TReadObject, TMajor> majorReader,
        Action<TGroup, IEnumerable<TObject>> groupSetter,
        Action<TObject, IEnumerable<TBlock>> topRecordSetter,
        List<Action> toDo)
        where TGroup : class, IClearable
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        streamPackage = streamPackage with { Path = Path.Combine(streamPackage.Path!, fieldName!) };
        
        ReadGroupHeaderPathToWork(streamPackage, group, metaData, kernel, groupReader, toDo);
        
        var groupFileName = TypicalGroupFileName(kernel.ExpectedExtension);
        var recordFileName = RecordFileName(kernel.ExpectedExtension);
        
        toDo.Add(() =>
        {
            var topRecords = streamPackage.FileSystem.Directory.GetDirectories(streamPackage.Path!)
                .AsParallel()
                .Select(topRecordDir =>
                {
                    var topRecord = ReadTopRecord(
                        streamPackage,
                        metaData,
                        kernel,
                        topRecordDir,
                        recordFileName,
                        groupFileName,
                        objReader,
                        blockReader,
                        blockSet,
                        subBlockReader,
                        subBlockSet,
                        majorReader,
                        topRecordSetter);
                    return (Path.GetFileName(topRecordDir), topRecord);
                })
                .ToArray()
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.Item2)
                .NotNull();
            
            groupSetter(group, topRecords);
        });
    }

    private static TObject? ReadTopRecord<TKernel, TReadObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        SerializationMetaData metaData,
        TKernel kernel,
        string topRecordDir,
        string recordFileName,
        string groupFileName,
        Read<TKernel, TReadObject, TObject> objReader,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, P2Int16, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        Read<TKernel, TReadObject, TMajor> majorReader,
        Action<TObject, IEnumerable<TBlock>> topRecordSetter)
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        var topRecordPath = Path.Combine(topRecordDir, recordFileName);
        TObject topRecord;
        if (streamPackage.FileSystem.File.Exists(topRecordPath))
        {
            using (var recordStream = streamPackage.FileSystem.File.OpenRead(topRecordPath))
            {
                var reader = kernel.GetNewObject(streamPackage with { Stream = recordStream });

                topRecord = objReader(reader, kernel, metaData);
            }
        }
        else
        {
            if (!FormKey.TryFactory(topRecordDir, out var fk))
            {
                return default(TObject);
            }

            topRecord = MajorRecordInstantiator<TObject>.Activator(fk, metaData.Release);
        }

        streamPackage = streamPackage with { Path = Path.Combine(streamPackage.Path!, topRecordDir) };

        var blocks = streamPackage.FileSystem.Directory.GetDirectories(streamPackage.Path!)
            .AsParallel()
            .Select(blockDir =>
            {
                var block = ReadBlock(
                    streamPackage with { Path = Path.Combine(streamPackage.Path!, blockDir)},
                    metaData,
                    kernel,
                    blockDir,
                    groupFileName,
                    blockReader,
                    blockSet,
                    subBlockReader,
                    subBlockSet,
                    majorReader);
                return (Path.GetFileName(blockDir), block);
            })
            .ToArray()
            .OrderBy(x => TryGetNumber(x.Item1))
            .Select(x => x.block)
            .NotNull();

        topRecordSetter(topRecord, blocks);

        return topRecord;
    }

    private static TBlock? ReadBlock<TKernel, TReadObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        SerializationMetaData metaData,
        TKernel kernel,
        string blockDir,
        string groupFileName,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, P2Int16, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        Read<TKernel, TReadObject, TMajor> majorReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        if (!P2Int16.TryParse(TrimOrdering(Path.GetFileName(blockDir.AsSpan())), out var blockNum))
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
                var subBlock = ReadSubBlock(
                    streamPackage,
                    metaData,
                    kernel,
                    subBlockDir,
                    groupFileName,
                    subBlockReader,
                    subBlockSet,
                    majorReader);
                return (Path.GetFileName(subBlockDir), subBlock);
            })
            .ToArray()
            .OrderBy(x => TryGetNumber(x.Item1))
            .Select(x => x.subBlock)
            .NotNull();

        blockSet(block, blockNum, subBlocks);

        return block;
    }

    private static TSubBlock? ReadSubBlock<TKernel, TReadObject, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        SerializationMetaData metaData,
        TKernel kernel,
        string subBlockDir,
        string groupFileName,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        Read<TKernel, TReadObject, TMajor> majorReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TSubBlock : class, new()
    {
        if (!P2Int16.TryParse(TrimOrdering(Path.GetFileName(subBlockDir.AsSpan())), out var subBlockNum))
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

        return subBlock;
    }
}