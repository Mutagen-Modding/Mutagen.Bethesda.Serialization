using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Exceptions;
using Mutagen.Bethesda.Serialization.Streams;
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
        WriteAsync<TKernel, TWriteObject, TGroup> writer,
        HasSerializationItems<TGroup> hasSerializationItems) 
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        if (streamPackage.Path == null)
        {
            throw new ArgumentException("Stream had no anchor path");
        }
        
        var groupDir = Path.Combine(streamPackage.Path, folderName);

        if (!hasSerializationItems(group, metaData)) return groupDir;

        metaData.FileSystem.Directory.CreateDirectory(groupDir);

        await metaData.WorkDropoff.EnqueueAndWait(() =>
        {
            var dataPath = Path.Combine(groupDir, fileName);
            try
            {
                using var stream = metaData.StreamCreator.GetStreamFor(metaData.FileSystem, dataPath, write: true);
                var groupRecStreamPackage = new StreamPackage(stream, groupDir);
                var dataWriter = kernel.GetNewObject(groupRecStreamPackage);
                writer(dataWriter, group, kernel, metaData);
                kernel.Finalize(groupRecStreamPackage, dataWriter);
            }
            catch (Exception e)
            {
                throw FilePathedException.Enrich(kernel.ConvertException(e), dataPath);
            }
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
        if (!metaData.FileSystem.Directory.Exists(streamPackage.Path)) return;
        
        var groupHeaderPath = await ReadGroupHeaderPathToWork(
            streamPackage, group, metaData, kernel, groupReader);
        var groupHeaderFileName = Path.GetFileName(groupHeaderPath);

        var files = metaData.FileSystem.Directory
                .GetFiles(streamPackage.Path!)
                .Where(x =>
                {
                    var fileName = Path.GetFileName(x.AsSpan());
                    var ext = Path.GetExtension(fileName);
                    return ext.Equals(kernel.ExpectedExtension.AsSpan(), StringComparison.OrdinalIgnoreCase)
                        && !fileName.Equals(groupHeaderFileName.AsSpan(), StringComparison.OrdinalIgnoreCase);
                });

        var records = await metaData.WorkDropoff.EnqueueAndWait(
            files,
            async recordPath =>
            {
                try
                {
                    using var stream = metaData.FileSystem.File.OpenRead(recordPath);

                    var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

                    return (Path.GetFileName(recordPath), await itemReader(reader, kernel, metaData));
                }
                catch (Exception e)
                {
                    throw FilePathedException.Enrich(kernel.ConvertException(e), recordPath);
                }
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
        HasSerializationItems<TGroup> groupHasSerializationItems,
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
            writer: groupWriter,
            hasSerializationItems: groupHasSerializationItems);

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