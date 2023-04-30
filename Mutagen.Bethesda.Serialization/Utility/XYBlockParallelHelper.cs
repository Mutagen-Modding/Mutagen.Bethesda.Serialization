using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task AddXYBlocksToWork<TKernel, TWriteObject, TGroup, TObject, TBlock, TSubBlock, TMajor>(
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
        WriteAsync<TKernel, TWriteObject, TGroup> groupWriter,
        WriteAsync<TKernel, TWriteObject, TObject> topRecordWriter,
        WriteAsync<TKernel, TWriteObject, TBlock> blockWriter,
        WriteAsync<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        WriteAsync<TKernel, TWriteObject, TMajor> majorWriter,
        bool withNumbering)
        where TObject : class, IMajorRecordGetter
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));
        
        var groupDir = await WriteGroupRecordData(streamPackage: streamPackage,
            @group: group,
            folderName: fieldName,
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: groupWriter);

        streamPackage = streamPackage with { Stream = null!, Path = groupDir };

        await metaData.WorkDropoff.EnqueueAndWait(
            topRecordRetriever(group)
                .WithIndex(),
            async topRecord =>
            {
                await WriteTopRecord(
                    streamPackage, blockRetriever, subBlockRetriever, majorRetriever,
                    blockNumberRetriever, subBlockNumberRetriever, metaData, kernel,
                    topRecordWriter, blockWriter, subBlockWriter, majorWriter, withNumbering ? topRecord.Index : null,
                    withNumbering, topRecord.Item);
            });
    }

    private static async Task WriteTopRecord<TKernel, TWriteObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        Func<TObject, IEnumerable<TBlock>> blockRetriever,
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, P2Int16> blockNumberRetriever,
        Func<TSubBlock, P2Int16> subBlockNumberRetriever,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TObject> topRecordWriter,
        WriteAsync<TKernel, TWriteObject, TBlock> blockWriter,
        WriteAsync<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        WriteAsync<TKernel, TWriteObject, TMajor> majorWriter,
        int? number,
        bool withNumbering,
        TObject topRecord) where TObject : class, IMajorRecordGetter
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        var folderName = RecordFileNameProvider(topRecord, string.Empty, number);

        var topRecordDir = await WriteGroupRecordData(
            streamPackage: streamPackage,
            group: topRecord,
            folderName: folderName,
            fileName: RecordDataFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: topRecordWriter);

        streamPackage = streamPackage with { Stream = null!, Path = topRecordDir };

        await metaData.WorkDropoff.EnqueueAndWait(
            blockRetriever(topRecord)
                .WithIndex(),
            async blockGetter =>
            {
                await WriteBlock(
                    streamPackage, subBlockRetriever, majorRetriever, blockNumberRetriever,
                    subBlockNumberRetriever, metaData, kernel, blockWriter, subBlockWriter,
                    majorWriter, withNumbering ? blockGetter.Index : null, withNumbering, blockGetter.Item);
            });
    }

    private static async Task WriteBlock<TKernel, TWriteObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage, 
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, P2Int16> blockNumberRetriever,
        Func<TSubBlock, P2Int16> subBlockNumberRetriever, 
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TBlock> blockWriter,
        WriteAsync<TKernel, TWriteObject, TSubBlock> subBlockWriter, 
        WriteAsync<TKernel, TWriteObject, TMajor> majorWriter,
        int? number,
        bool withNumbering,
        TBlock blockGetter)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        var blockNum = blockNumberRetriever(blockGetter);
        var blockDir = await WriteGroupRecordData(
            streamPackage: streamPackage,
            group: blockGetter,
            folderName: DecorateWithNumber($"{blockNum.X}, {blockNum.Y}", number),
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: blockWriter);

        streamPackage = streamPackage with { Stream = null!, Path = blockDir };

        await metaData.WorkDropoff.EnqueueAndWait(
            subBlockRetriever(blockGetter)
                .WithIndex(),
            async subBlockGetter =>
            {
                await WriteSubBlock(
                    streamPackage, majorRetriever, subBlockNumberRetriever,
                    metaData, kernel, subBlockWriter, withNumbering ? subBlockGetter.Index : null, 
                    withNumbering, majorWriter, subBlockGetter.Item);
            });
    }

    private static async Task WriteSubBlock<TKernel, TWriteObject, TSubBlock, TMajor>(
        StreamPackage streamPackage, 
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever, 
        Func<TSubBlock, P2Int16> subBlockNumberRetriever, SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        int? number,
        bool withNumbering,
        WriteAsync<TKernel, TWriteObject, TMajor> majorWriter,
        TSubBlock subBlockGetter) 
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        var subBlockNum = subBlockNumberRetriever(subBlockGetter);
        var subBlockDir = await WriteGroupRecordData(
            streamPackage: streamPackage,
            group: subBlockGetter,
            folderName: DecorateWithNumber($"{subBlockNum.X}, {subBlockNum.Y}", number),
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: subBlockWriter);

        await metaData.WorkDropoff.EnqueueAndWait(
            majorRetriever(subBlockGetter)
                .WithIndex(),
            async recordGetter =>
            {
                var recordFolder = Path.Combine(subBlockDir, RecordNameProvider(
                    recordGetter.Item,
                    withNumbering ? recordGetter.Index : null));

                metaData.FileSystem.Directory.CreateDirectory(recordFolder);
                                
                var fileName = RecordDataFileName(kernel.ExpectedExtension);
                var recordPath = Path.Combine(recordFolder, fileName);
                await using var stream = metaData.StreamCreator.GetStreamFor(metaData.FileSystem, recordPath, write: true);
                var recordStreamPackage = streamPackage with { Stream = stream, Path = recordFolder };
                var recordWriter = kernel.GetNewObject(recordStreamPackage);
                await majorWriter(recordWriter, recordGetter.Item, kernel, metaData);
                kernel.Finalize(recordStreamPackage, recordWriter);
            });
    }
    
    public static async Task ReadIntoXYBlocks<TKernel, TReadObject, TGroup, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TGroup group,
        string fieldName,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        ReadAsync<TKernel, TReadObject, TObject> objReader,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, P2Int16, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        ReadAsync<TKernel, TReadObject, TMajor> majorReader,
        Action<TGroup, IEnumerable<TObject>> groupSetter,
        Action<TObject, IEnumerable<TBlock>> topRecordSetter)
        where TGroup : class, IClearable
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        streamPackage = streamPackage with { Path = Path.Combine(streamPackage.Path!, fieldName!) };
        
        await ReadGroupHeaderPathToWork(streamPackage, group, metaData, kernel, groupReader);
        
        var groupFileName = TypicalGroupFileName(kernel.ExpectedExtension);
        var recordFileName = RecordDataFileName(kernel.ExpectedExtension);

        var topRecords = await metaData.WorkDropoff.EnqueueAndWait(
            metaData.FileSystem.Directory.GetDirectories(streamPackage.Path!),
            async topRecordDir =>
            {
                var topRecord = await ReadTopRecord(
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
            });
            
        groupSetter(
            group, 
            topRecords
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.Item2)
                .NotNull());
    }

    private static async Task<TObject?> ReadTopRecord<TKernel, TReadObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        SerializationMetaData metaData,
        TKernel kernel,
        string topRecordDir,
        string recordFileName,
        string groupFileName,
        ReadAsync<TKernel, TReadObject, TObject> objReader,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, P2Int16, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        ReadAsync<TKernel, TReadObject, TMajor> majorReader,
        Action<TObject, IEnumerable<TBlock>> topRecordSetter)
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        var topRecordPath = Path.Combine(topRecordDir, recordFileName);
        TObject topRecord;
        if (metaData.FileSystem.File.Exists(topRecordPath))
        {
            using (var recordStream = metaData.FileSystem.File.OpenRead(topRecordPath))
            {
                var reader = kernel.GetNewObject(streamPackage with
                {
                    Stream = recordStream,
                    Path = topRecordDir,
                });

                topRecord = await objReader(reader, kernel, metaData);
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

        var blocks = await metaData.WorkDropoff.EnqueueAndWait(
            metaData.FileSystem.Directory.GetDirectories(streamPackage.Path!),
            async blockDir =>
            {
                var block = await ReadBlock(
                    streamPackage with { Path = Path.Combine(streamPackage.Path!, blockDir) },
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
            });

        topRecordSetter(
            topRecord, 
            blocks
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.block)
                .NotNull());

        return topRecord;
    }

    private static async Task<TBlock?> ReadBlock<TKernel, TReadObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        SerializationMetaData metaData,
        TKernel kernel,
        string blockDir,
        string groupFileName,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, P2Int16, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        ReadAsync<TKernel, TReadObject, TMajor> majorReader)
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
        if (metaData.FileSystem.File.Exists(blockDataPath))
        {
            using (var blockStream = metaData.FileSystem.File.OpenRead(blockDataPath))
            {
                var reader = kernel.GetNewObject(streamPackage with { Stream = blockStream });

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
                var subBlock = await ReadSubBlock(
                    streamPackage,
                    metaData,
                    kernel,
                    subBlockDir,
                    groupFileName,
                    subBlockReader,
                    subBlockSet,
                    majorReader);
                return (Path.GetFileName(subBlockDir), subBlock);
            });

        blockSet(
            block,
            blockNum, 
            subBlocks
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.subBlock)
                .NotNull());

        return block;
    }

    private static async Task<TSubBlock?> ReadSubBlock<TKernel, TReadObject, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        SerializationMetaData metaData,
        TKernel kernel,
        string subBlockDir,
        string groupFileName,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        ReadAsync<TKernel, TReadObject, TMajor> majorReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TSubBlock : class, new()
    {
        if (!P2Int16.TryParse(TrimOrdering(Path.GetFileName(subBlockDir.AsSpan())), out var subBlockNum))
        {
            return default;
        }

        var subBlockDataPath = Path.Combine(subBlockDir, groupFileName);

        TSubBlock subBlock = new();
        if (metaData.FileSystem.File.Exists(subBlockDataPath))
        {
            using (var subBlockStream = metaData.FileSystem.File.OpenRead(subBlockDataPath))
            {
                var reader = kernel.GetNewObject(streamPackage with { Stream = subBlockStream });

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
                    var reader = kernel.GetNewObject(streamPackage with
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

        return subBlock;
    }
}