using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task WriteFolderPerRecord<TKernel, TWriteObject, TGroup, TObject>(
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
        
        var fileName = RecordDataFileName(kernel.ExpectedExtension);

        await metaData.WorkDropoff.EnqueueAndWait(
            group.WithIndex(),
            async recordGetter =>
            {
                var recordDirName = RecordFileNameProvider(recordGetter.Item, string.Empty, withNumbering ? recordGetter.Index : null);

                var dir = Path.Combine(groupDir, recordDirName);
                metaData.FileSystem.Directory.CreateDirectory(dir);

                var recordPath = Path.Combine(dir, fileName);
                using var stream = metaData.StreamCreator.GetStreamFor(metaData.FileSystem, recordPath, write: true);
                var recordStreamPackage = streamPackage with { Stream = stream, Path = dir };
                var recordWriter = kernel.GetNewObject(recordStreamPackage);
                await itemWriter(recordWriter, recordGetter.Item, kernel, metaData);
                kernel.Finalize(recordStreamPackage, recordWriter);
            });
    }

    public static async Task ReadFolderPerRecord<TKernel, TReadObject, TGroup, TObject>(
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
        
        await ReadGroupHeaderPathToWork(
            streamPackage, group, metaData, kernel, groupReader);

        if (metaData.FileSystem.Directory.Exists(streamPackage.Path))
        {
            var fileName = RecordDataFileName(kernel.ExpectedExtension);

            var records = await metaData.WorkDropoff.EnqueueAndWait(
                metaData.FileSystem.Directory.GetDirectories(streamPackage.Path!),
                async recordDir =>
                {
                    var recordPath = Path.Combine(recordDir, fileName);

                    using var stream = metaData.FileSystem.File.OpenRead(recordPath);

                    var reader = kernel.GetNewObject(streamPackage with
                    {
                        Stream = stream,
                        Path = recordDir
                    });

                    return (Path.GetFileName(recordDir), await itemReader(reader, kernel, metaData));
                });
        
            group.RecordCache.SetTo(
                x => x.FormKey,
                records
                    .OrderBy(x => TryGetNumber(x.Item1))
                    .Select(x => x.Item2));
        }
    }
}