using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization;

public static class SerializationHelper
{
    public delegate void DeserializeIntoCall<TReadObject, TObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        TObject obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage;

    public static void DeserializeAllFieldsInto<TReadObject, TObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        TObject obj,
        SerializationMetaData metaData,
        DeserializeIntoCall<TReadObject, TObject> deserializeInto)
        where TReadObject : IContainStreamPackage
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            deserializeInto(reader, kernel, obj, metaData, name);
        }
    }
    
    public static void WriteGroup<TKernel, TWriteObject, TGroup, TObject>(
        TWriteObject writer,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        Write<TKernel, TWriteObject, TObject> itemWriter)
        where TGroup : class, IReadOnlyCollection<TObject>
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        if (group.Count == 0) return;
        kernel.WriteLoqui(writer, fieldName, group, metaData, (w, g, k, m) =>
        {
            groupWriter(w, g, k, m);
            k.StartListSection(w, "Records");
            foreach (var recordGetter in g)
            {
                k.WriteLoqui(w, null, recordGetter, m, itemWriter);
            }
            k.EndListSection(w);
        });
    }
    
    public static void ReadIntoGroup<TKernel, TReadObject, TGroup, TObject>(
        TReadObject reader,
        TGroup group,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TGroup : class, IGroup<TObject>
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        group.Clear();
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "Records":
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = itemReader(reader, kernel, metaData);
                        group.Add(item);
                    }
                    kernel.EndListSection(reader);
                    break;
                default:
                    groupReader(reader, group, kernel, metaData, name);
                    break;
            }
        }
    }

    private static string TypicalGroupFileName(string expectedExtension)
    {
        return $"GroupRecordData{expectedExtension}";
    }

    private static string RecordFileName(string expectedExtension)
    {
        return $"RecordData{expectedExtension}";
    }
    
    private static string WriteGroupRecordData<TKernel, TWriteObject, TGroup>(
        StreamPackage streamPackage,
        TGroup group, 
        string folderName,
        string fileName,
        SerializationMetaData metaData, 
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        List<Action> toDo) 
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        if (streamPackage.Path == null)
        {
            throw new ArgumentException("Stream had no anchor path");
        }

        var groupDir = Path.Combine(streamPackage.Path, folderName);
        streamPackage.FileSystem.Directory.DeleteEntireFolder(groupDir);
        streamPackage.FileSystem.Directory.CreateDirectory(groupDir);

        toDo.Add(() =>
        {
            var dataPath = Path.Combine(groupDir, fileName);
            using var stream = streamPackage.FileSystem.File.Create(dataPath);
            var groupRecStreamPackage = new StreamPackage(stream, groupDir, streamPackage.FileSystem);
            var dataWriter = kernel.GetNewObject(groupRecStreamPackage);
            groupWriter(dataWriter, group, kernel, metaData);
            kernel.Finalize(groupRecStreamPackage, dataWriter);
        });
        return groupDir;
    }
    
    public static void ReadFolderPerRecord<TKernel, TReadObject, TGroup, TObject>(
        StreamPackage streamPackage,
        TGroup group,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> itemReader,
        List<Action> toDo)
        where TGroup : class, IGroup<TObject>
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        var groupHeaderPath = ReadGroupHeaderPathToWork(
            streamPackage, group, metaData, kernel, groupReader, toDo);

        toDo.Add(() =>
        {
            var records = streamPackage.FileSystem.Directory.GetFiles(streamPackage.Path!)
                .Where(x => !groupHeaderPath.Equals(x, StringComparison.OrdinalIgnoreCase))
                .AsParallel()
                .Select(x =>
                {
                    using var stream = streamPackage.FileSystem.File.OpenRead(x);

                    var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

                    return (x, itemReader(reader, kernel, metaData));
                })
                .OrderBy(x => x)
                .Select(x => x.Item2)
                .ToArray();
            group.RecordCache.SetTo(
                x => x.FormKey,
                records);
        });
    }

    private static string ReadGroupHeaderPathToWork<TKernel, TReadObject, TGroup>(
        StreamPackage streamPackage,
        TGroup group,
        SerializationMetaData metaData, 
        TKernel kernel, 
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        List<Action> toDo)
        where TGroup : class, IClearable
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        group.Clear();
        return ReadPathToWork(streamPackage, group, TypicalGroupFileName(kernel.ExpectedExtension), metaData, kernel, groupReader, toDo);
    }

    private static string ReadPathToWork<TKernel, TReadObject, TGroup>(
        StreamPackage streamPackage,
        TGroup group,
        string fileName,
        SerializationMetaData metaData, 
        TKernel kernel, 
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        List<Action> toDo)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        var groupHeaderPath = Path.Combine(streamPackage.Path!, fileName);
        toDo.Add(() =>
        {
            if (streamPackage.FileSystem.File.Exists(groupHeaderPath))
            {
                using var groupStream = streamPackage.FileSystem.File.OpenRead(groupHeaderPath);

                var reader = kernel.GetNewObject(streamPackage with { Stream = groupStream });
                while (kernel.TryGetNextField(reader, out var name))
                {
                    groupReader(reader, group, kernel, metaData, name);
                }
            }
        });
        return groupHeaderPath;
    }

    public static void WriteFolderPerRecord<TKernel, TWriteObject, TGroup, TObject>(
        StreamPackage streamPackage,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        Write<TKernel, TWriteObject, TObject> itemWriter,
        List<Action> toDo)
        where TGroup : class, IReadOnlyCollection<TObject>
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TObject : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));
        
        var groupDir = WriteGroupRecordData(
            streamPackage: streamPackage,
            @group: group,
            folderName: fieldName,
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: groupWriter,
            toDo: toDo);

        foreach (var recordGetter in group)
        {
            toDo.Add(() =>
            {
                WriteMajor(streamPackage with { Stream = null!, Path = groupDir }, metaData, kernel, itemWriter, recordGetter);
            });
        }
    }

    private static void WriteMajor<TKernel, TWriteObject, TObject>(
        StreamPackage streamPackage,
        SerializationMetaData metaData, 
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TObject> itemWriter,
        TObject recordGetter) where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TObject : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        var fileName = FileNameProvider(recordGetter, kernel.ExpectedExtension);
        var recordPath = Path.Combine(streamPackage.Path!, fileName);
        using var stream = streamPackage.FileSystem.File.Create(recordPath);
        var recordStreamPackage = streamPackage with { Stream = stream };
        var recordWriter = kernel.GetNewObject(recordStreamPackage);
        itemWriter(recordWriter, recordGetter, kernel, metaData);
        kernel.Finalize(recordStreamPackage, recordWriter);
    }

    public static void AddBlocksToWork<TKernel, TWriteObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TObject obj,
        string? fieldName,
        Func<TObject, IEnumerable<TBlock>> blockRetriever,
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, int> blockNumberRetriever,
        Func<TSubBlock, int> subBlockNumberRetriever,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TObject> metaWriter,
        Write<TKernel, TWriteObject, TBlock> blockWriter,
        Write<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        Write<TKernel, TWriteObject, TMajor> majorWriter,
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

        foreach (var blockGetter in blockRetriever(obj))
        {
            var blockDir = WriteGroupRecordData(
                streamPackage: streamPackage with { Stream = null!, Path = groupDir },
                @group: blockGetter, 
                folderName: blockNumberRetriever(blockGetter).ToString(), 
                fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                metaData: metaData,
                kernel: kernel,
                groupWriter: blockWriter, 
                toDo: toDo);

            foreach (var subBlockGetter in subBlockRetriever(blockGetter))
            {
                var subBlockDir = WriteGroupRecordData(
                    streamPackage: streamPackage with { Stream = null!, Path = blockDir },
                    @group: subBlockGetter, 
                    folderName: subBlockNumberRetriever(subBlockGetter).ToString(), 
                    fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                    metaData: metaData,
                    kernel: kernel,
                    groupWriter: subBlockWriter, 
                    toDo: toDo);

                foreach (var recordGetter in majorRetriever(subBlockGetter))
                {
                    toDo.Add(() =>
                    {
                        WriteMajor(
                            streamPackage with { Stream = null!, Path = subBlockDir },
                            metaData, kernel, majorWriter, recordGetter);
                    });
                }
            }
        }
    }

    public static void ReadFolderPerRecordIntoBlocks<TKernel, TReadObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TObject group,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TObject> groupReader,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, int, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, int, IEnumerable<TMajor>> subBlockSet,
        Read<TKernel, TReadObject, TMajor> majorReader,
        Action<TObject, IEnumerable<TBlock>> groupSetter,
        List<Action> toDo)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TObject : class, IClearable
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        ReadGroupHeaderPathToWork(streamPackage, group, metaData, kernel, groupReader, toDo);

        var dataFileName = TypicalGroupFileName(kernel.ExpectedExtension);
        
        toDo.Add(() =>
        {
            var blocks = streamPackage.FileSystem.Directory.GetDirectories(streamPackage.Path!)
                .AsParallel()
                .Select(blockDir =>
                {
                    if (!int.TryParse(Path.GetFileName(blockDir), out var blockNum))
                    {
                        return default;
                    }

                    var blockDataPath = Path.Combine(blockDir, dataFileName);
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
                            if (!int.TryParse(Path.GetFileName(subBlockDir), out var subBlockNum))
                            {
                                return default;
                            }

                            var subBlockDataPath = Path.Combine(subBlockDir, dataFileName);

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
                                .Where(x => !x.Equals(subBlockDataPath, StringComparison.OrdinalIgnoreCase))
                                .AsParallel()
                                .Select(recordFile =>
                                {
                                    using (var recordStream = streamPackage.FileSystem.File.OpenRead(recordFile))
                                    {
                                        var reader = kernel.GetNewObject(streamPackage with { Stream = recordStream });

                                        return (recordFile, majorReader(reader, kernel, metaData));
                                    }
                                })
                                .OrderBy(x => x.recordFile)
                                .Select(x => x.Item2)
                                .NotNull()
                                .ToArray();
                            
                            subBlockSet(subBlock, subBlockNum, records);
                            
                            return (subBlockDir, subBlock);
                        })
                        .OrderBy(x => x.subBlockDir)
                        .Select(x => x.subBlock)
                        .NotNull()
                        .ToArray();
                    
                    blockSet(block, blockNum, subBlocks);

                    return (blockDir, block);
                })
                .OrderBy(x => x.blockDir)
                .Select(x => x.block)
                .NotNull()
                .ToArray();
            groupSetter(group, blocks);
        });
    }

    public static void AddXYBlocksToWork<TKernel, TWriteObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TObject obj,
        string? fieldName,
        Func<TObject, IEnumerable<TBlock>> blockRetriever,
        Func<TBlock, IEnumerable<TSubBlock>> subBlockRetriever,
        Func<TSubBlock, IEnumerable<TMajor>> majorRetriever,
        Func<TBlock, P2Int16> blockNumberRetriever,
        Func<TSubBlock, P2Int16> subBlockNumberRetriever,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TObject> metaWriter,
        Write<TKernel, TWriteObject, TBlock> blockWriter,
        Write<TKernel, TWriteObject, TSubBlock> subBlockWriter,
        Write<TKernel, TWriteObject, TMajor> majorWriter,
        List<Action> toDo)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
        where TMajor : class, IMajorRecordGetter
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));
        
        var groupDir = WriteGroupRecordData(
            streamPackage: streamPackage,
            @group: obj,
            folderName: fieldName,
            fileName: RecordFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: metaWriter,
            toDo: toDo);
        
        foreach (var blockGetter in blockRetriever(obj))
        {
            var blockNum = blockNumberRetriever(blockGetter);
            var blockDir = WriteGroupRecordData(
                streamPackage: streamPackage with { Stream = null!, Path = groupDir },
                @group: blockGetter, 
                folderName: $"{blockNum.X}, {blockNum.Y}", 
                fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                metaData: metaData,
                kernel: kernel,
                groupWriter: blockWriter, 
                toDo: toDo);

            foreach (var subBlockGetter in subBlockRetriever(blockGetter))
            {
                var subBlockNum = subBlockNumberRetriever(subBlockGetter);
                var subBlockDir = WriteGroupRecordData(
                    streamPackage: streamPackage with { Stream = null!, Path = blockDir },
                    @group: subBlockGetter, 
                    folderName: $"{subBlockNum.X}, {subBlockNum.Y}", 
                    fileName: TypicalGroupFileName(kernel.ExpectedExtension),
                    metaData: metaData,
                    kernel: kernel,
                    groupWriter: subBlockWriter, 
                    toDo: toDo);

                foreach (var recordGetter in majorRetriever(subBlockGetter))
                {
                    toDo.Add(() =>
                    {
                        WriteMajor(
                            streamPackage with { Stream = null!, Path = subBlockDir },
                            metaData, kernel, majorWriter, recordGetter);
                    });
                }
            }
        }
    }

    public static void ReadFolderPerRecordIntoXYBlocks<TKernel, TReadObject, TObject, TBlock, TSubBlock, TMajor>(
        StreamPackage streamPackage,
        TObject obj,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TObject> objReader,
        ReadNamedInto<TKernel, TReadObject, TBlock> blockReader,
        Action<TBlock, P2Int16, IEnumerable<TSubBlock>> blockSet,
        ReadNamedInto<TKernel, TReadObject, TSubBlock> subBlockReader,
        Action<TSubBlock, P2Int16, IEnumerable<TMajor>> subBlockSet,
        Read<TKernel, TReadObject, TMajor> majorReader,
        Action<TObject, IEnumerable<TBlock>> groupSetter,
        List<Action> toDo)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajor : class, IMajorRecordGetter
        where TBlock : class, new()
        where TSubBlock : class, new()
    {
        ReadPathToWork(streamPackage, obj, RecordFileName(kernel.ExpectedExtension), metaData, kernel, objReader, toDo);

        var dataFileName = TypicalGroupFileName(kernel.ExpectedExtension);
        
        toDo.Add(() =>
        {
            var blocks = streamPackage.FileSystem.Directory.GetDirectories(streamPackage.Path!)
                .AsParallel()
                .Select(blockDir =>
                {
                    if (!P2Int16.TryParse(Path.GetFileName(blockDir), out var blockNum))
                    {
                        return default;
                    }

                    var blockDataPath = Path.Combine(blockDir, dataFileName);
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
                            if (!P2Int16.TryParse(Path.GetFileName(subBlockDir), out var subBlockNum))
                            {
                                return default;
                            }

                            var subBlockDataPath = Path.Combine(subBlockDir, dataFileName);

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
                                .Where(x => !x.Equals(subBlockDataPath, StringComparison.OrdinalIgnoreCase))
                                .AsParallel()
                                .Select(recordFile =>
                                {
                                    using (var recordStream = streamPackage.FileSystem.File.OpenRead(recordFile))
                                    {
                                        var reader = kernel.GetNewObject(streamPackage with { Stream = recordStream });

                                        return (recordFile, majorReader(reader, kernel, metaData));
                                    }
                                })
                                .OrderBy(x => x.recordFile)
                                .Select(x => x.Item2)
                                .NotNull()
                                .ToArray();
                            
                            subBlockSet(subBlock, subBlockNum, records);
                            
                            return (subBlockDir, subBlock);
                        })
                        .OrderBy(x => x.subBlockDir)
                        .Select(x => x.subBlock)
                        .NotNull()
                        .ToArray();
                    
                    blockSet(block, blockNum, subBlocks);

                    return (blockDir, block);
                })
                .OrderBy(x => x.blockDir)
                .Select(x => x.block)
                .NotNull()
                .ToArray();
            groupSetter(obj, blocks);
        });
    }

    public static string FileNameProvider(IMajorRecordGetter recordGetter, string expectedExtension)
    {
        var edid = recordGetter.EditorID;
        return $"{edid ?? recordGetter.FormKey.ToFilesafeString()}{expectedExtension}";
    }
    
    public static void ReadIntoListGroup<TKernel, TReadObject, TGroup, TObject>(
        TReadObject reader,
        TGroup group,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TGroup : class, IListGroup<TObject>
        where TObject : class, ILoquiObject
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        group.Clear();
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "Records":
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = itemReader(reader, kernel, metaData);
                        group.Add(item);
                    }
                    kernel.EndListSection(reader);
                    break;
                default:
                    groupReader(reader, group, kernel, metaData, name);
                    break;
            }
        }
    }
    
    public static void ReadIntoArray<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TObject[] arr,
        TKernel kernel,
        SerializationMetaData metaData,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        int i = 0;
        kernel.StartListSection(reader);
        while (i < arr.Length && kernel.TryHasNextItem(reader))
        {
            var item = itemReader(reader, kernel, metaData);
            arr[i] = item;
            i++;
        }

        if (kernel.TryHasNextItem(reader))
        {
            throw new DataMisalignedException($"Array had more items than the max allowed: {arr.Length}");
        }

        if (i < arr.Length)
        {
            throw new DataMisalignedException($"Array was not filled entirely with items. {i} < {arr.Length}");
        }
        
        kernel.EndListSection(reader);
    }

    public static void ReadIntoSlice<TKernel, TReadObject, TObject>(
        TReadObject reader,
        MemorySlice<TObject> arr,
        TKernel kernel,
        SerializationMetaData metaData,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        int i = 0;
        kernel.StartListSection(reader);
        while (i < arr.Length && kernel.TryHasNextItem(reader))
        {
            var item = itemReader(reader, kernel, metaData);
            arr[i] = item;
            i++;
        }

        if (kernel.TryHasNextItem(reader))
        {
            throw new DataMisalignedException($"Array had more items than the max allowed: {arr.Length}");
        }

        if (i < arr.Length)
        {
            throw new DataMisalignedException($"Array was not filled entirely with items. {i} < {arr.Length}");
        }
        
        kernel.EndListSection(reader);
    }

    public static MemorySlice<TObject> ReadSlice<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TKernel kernel,
        SerializationMetaData metaData,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        int i = 0;
        List<TObject> ret = new();
        kernel.StartListSection(reader);
        while (kernel.TryHasNextItem(reader))
        {
            var item = itemReader(reader, kernel, metaData);
            ret.Add(item);
            i++;
        }
        
        kernel.EndListSection(reader);

        return ret.ToArray();
    }

    public static T StripNull<T>(T? item, string name)
        where T : class
    {
        if (item == null)
        {
            throw new NullReferenceException($"{name} was null");
        }

        return item;
    }

    public static T StripNull<T>(T? item, string name)
        where T : struct
    {
        if (item == null)
        {
            throw new NullReferenceException($"{name} was null");
        }

        return item.Value;
    }

    public static GenderedItem<TObject> ReadGenderedItem<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TKernel kernel,
        SerializationMetaData metaData,
        GenderedItem<TObject> ret,
        ReadNamed<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "Male":
                    ret.Male = itemReader(reader, kernel, metaData, "Male");
                    break;
                case "Female":
                    ret.Female = itemReader(reader, kernel, metaData, "Female");
                    break;
            }
        }

        return ret;
    }

    public static void WriteGendered<TKernel, TWriteObject, TObject>(
        TWriteObject writer,
        string? fieldName, 
        IGenderedItemGetter<TObject> item,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TObject> itemWriter)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteLoqui(writer, fieldName, item, metaData, (w, o, k, m) =>
        {
            k.WriteWithName(w, "Male", o.Male, m, itemWriter);
            k.WriteWithName(w, "Female", o.Female, m, itemWriter);
        });
    }

    public static void ReadFormLinkOrIndex<TKernel, TReadObject, TMajorGetter>(
        TReadObject reader,
        TKernel kernel,
        IFormLinkOrIndex<TMajorGetter> item,
        SerializationMetaData metaData)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (item.UsesLink())
        {
            item.Link.SetTo(kernel.ReadFormKey(reader));
        }
        else
        {
            item.Index = kernel.ReadUInt32(reader);
        }
    }

    public static void WriteFormLinkOrIndex<TKernel, TWriteObject, TMajorGetter>(
        TWriteObject writer,
        string? fieldName, 
        IFormLinkOrIndexGetter<TMajorGetter> item,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (item.UsesLink())
        {
            kernel.WriteFormKey(writer, fieldName, item.Link.FormKey, FormKey.Null);
        }
        else
        {
            kernel.WriteUInt32(writer, fieldName, item.Index.Value, 0);
        }
    }

    public static void WriteMajorRecordList<TKernel, TWriteObject, TMajorGetter>(
        StreamPackage streamPackage,
        string? fieldName, 
        IReadOnlyList<TMajorGetter> list,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TMajorGetter> itemWriter,
        List<Action> toDo)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TMajorGetter : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));

        if (list.Count == 0) return;
        
        var dir = Path.Combine(streamPackage.Path!, fieldName);
        streamPackage.FileSystem.Directory.CreateDirectory(dir);
        
        foreach (var recordGetter in list)
        {
            toDo.Add(() =>
            {
                WriteMajor(streamPackage with { Stream = null!, Path = fieldName }, metaData, kernel, itemWriter, recordGetter);
            });
        }
    }
    
    public static void ReadMajorRecordList<TKernel, TReadObject, TMajorRecord>(
        StreamPackage streamPackage,
        IList<TMajorRecord> list,
        SerializationMetaData metaData,
        TKernel kernel,
        Read<TKernel, TReadObject, TMajorRecord> itemReader,
        List<Action> toDo)
        where TMajorRecord : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        toDo.Add(() =>
        {
            var records = streamPackage.FileSystem.Directory.GetFiles(streamPackage.Path!)
                .AsParallel()
                .Select(x =>
                {
                    using var stream = streamPackage.FileSystem.File.OpenRead(x);

                    var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

                    return (x, itemReader(reader, kernel, metaData));
                })
                .OrderBy(x => x)
                .Select(x => x.Item2)
                .ToArray();
            list.SetTo(records);
        });
    }
}