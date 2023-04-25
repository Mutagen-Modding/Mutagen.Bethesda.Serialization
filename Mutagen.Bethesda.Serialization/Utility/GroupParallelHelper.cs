using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    private static async Task<string> WriteGroupRecordData<TKernel, TWriteObject, TGroup>(
        StreamPackage streamPackage,
        TGroup group, 
        string folderName,
        string fileName,
        SerializationMetaData metaData, 
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TGroup> groupWriter) 
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

        await metaData.WorkDropoff.EnqueueAndWait(() =>
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
    
    public static async Task ReadFilePerRecord<TKernel, TReadObject, TGroup, TObject>(
        StreamPackage streamPackage,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        ReadAsync<TKernel, TReadObject, TObject> itemReader)
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
        
        var groupHeaderPath = await ReadGroupHeaderPathToWork(
            streamPackage, group, metaData, kernel, groupReader);
        var groupHeaderFileName = Path.GetFileName(groupHeaderPath);

        var files = streamPackage.FileSystem.Directory
                .GetFiles(streamPackage.Path!)
                .Where(x => !groupHeaderFileName.AsSpan().Equals(Path.GetFileName(x.AsSpan()), StringComparison.OrdinalIgnoreCase));

        var records = await metaData.WorkDropoff.EnqueueAndWait(
            files,
            async recordPath =>
            {
                using var stream = streamPackage.FileSystem.File.OpenRead(recordPath);

                var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

                return (Path.GetFileName(recordPath), await itemReader(reader, kernel, metaData));
            });
        
        group.RecordCache.SetTo(
            x => x.FormKey, 
            records
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.Item2));
    }
    
    private static async Task<string> ReadGroupHeaderPathToWork<TKernel, TReadObject, TGroup>(
        StreamPackage streamPackage,
        TGroup group,
        SerializationMetaData metaData, 
        TKernel kernel, 
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader)
        where TGroup : class, IClearable
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        group.Clear();
        return await ReadPathToWork(streamPackage, group, TypicalGroupFileName(kernel.ExpectedExtension), metaData, kernel, groupReader);
    }
    
    public static async Task WriteFilePerRecord<TKernel, TWriteObject, TGroup, TObject>(
        StreamPackage streamPackage,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TGroup> groupWriter,
        WriteAsync<TKernel, TWriteObject, TObject> itemWriter,
        bool withNumbering)
        where TGroup : class, IReadOnlyCollection<TObject>
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TObject : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));
        
        var groupDir = await WriteGroupRecordData(
            streamPackage: streamPackage,
            @group: group,
            folderName: fieldName,
            fileName: TypicalGroupFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: groupWriter);

        var subPackage = streamPackage with { Stream = null!, Path = groupDir };

        await metaData.WorkDropoff.EnqueueAndWait(
            group.WithIndex(),
            async recordGetter =>
            {
                await WriteMajor(
                    subPackage,
                    metaData, kernel, itemWriter, recordGetter.Item,
                    numbering: withNumbering ? recordGetter.Index : null);
            });
    }
}