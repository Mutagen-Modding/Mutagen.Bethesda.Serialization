using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
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
    
    public static void ReadFilePerRecord<TKernel, TReadObject, TGroup, TObject>(
        StreamPackage streamPackage,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> itemReader,
        List<Action> toDo)
        where TGroup : class, IGroup<TObject>
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        if (streamPackage.Path == null) return;
        if (fieldName != null)
        {
            var subDir = Path.Combine(streamPackage.Path, fieldName);
            streamPackage = streamPackage with { Path = subDir };
        }
        if (!streamPackage.FileSystem.Directory.Exists(streamPackage.Path)) return;
        
        var groupHeaderPath = ReadGroupHeaderPathToWork(
            streamPackage, group, metaData, kernel, groupReader, toDo);
        var groupHeaderFileName = Path.GetFileName(groupHeaderPath);

        toDo.Add(() =>
        {
            var records = streamPackage.FileSystem.Directory.GetFiles(streamPackage.Path!)
                .Where(x => !groupHeaderFileName.AsSpan().Equals(Path.GetFileName(x.AsSpan()), StringComparison.OrdinalIgnoreCase))
                .AsParallel()
                .Select(recordPath =>
                {
                    using var stream = streamPackage.FileSystem.File.OpenRead(recordPath);

                    var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

                    return (Path.GetFileName(recordPath), itemReader(reader, kernel, metaData));
                })
                .ToArray()
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.Item2);
            group.RecordCache.SetTo(
                x => x.FormKey, records);
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
    
    public static void WriteFilePerRecord<TKernel, TWriteObject, TGroup, TObject>(
        StreamPackage streamPackage,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        Write<TKernel, TWriteObject, TObject> itemWriter,
        bool withNumbering,
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

        var subPackage = streamPackage with { Stream = null!, Path = groupDir };

        int? i = withNumbering ? 1 : null;
        foreach (var recordGetter in group)
        {
            int? number = i;
            toDo.Add(() =>
            {
                WriteMajor(
                    subPackage,
                    metaData, kernel, itemWriter, recordGetter,
                    numbering: number);
            });
            i++;
        }
    }
}