using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static void WriteFolderPerRecord<TKernel, TWriteObject, TGroup, TObject>(
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
        
        var fileName = RecordFileName(kernel.ExpectedExtension);

        int? i = withNumbering ? 1 : null;
        foreach (var recordGetter in group)
        {
            int? number = i;
            toDo.Add(() =>
            {
                var recordDirName = FileNameProvider(recordGetter, string.Empty, number);

                var dir = Path.Combine(groupDir, recordDirName);
                streamPackage.FileSystem.Directory.CreateDirectory(dir);

                var recordPath = Path.Combine(dir, fileName);
                using var stream = streamPackage.FileSystem.File.Create(recordPath);
                var recordStreamPackage = streamPackage with { Stream = stream, Path = dir };
                var recordWriter = kernel.GetNewObject(recordStreamPackage);
                itemWriter(recordWriter, recordGetter, kernel, metaData);
                kernel.Finalize(recordStreamPackage, recordWriter);
            });
            i++;
        }
    }

    public static void ReadFolderPerRecord<TKernel, TReadObject, TGroup, TObject>(
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
        
        ReadGroupHeaderPathToWork(
            streamPackage, group, metaData, kernel, groupReader, toDo);
        
        var fileName = RecordFileName(kernel.ExpectedExtension);

        toDo.Add(() =>
        {
            var records = streamPackage.FileSystem.Directory.GetDirectories(streamPackage.Path!)
                .AsParallel()
                .Select(recordDir =>
                {
                    var recordPath = Path.Combine(recordDir, fileName);
                    
                    using var stream = streamPackage.FileSystem.File.OpenRead(recordPath);

                    var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

                    return (Path.GetFileName(recordDir), itemReader(reader, kernel, metaData));
                })
                .ToArray()
                .OrderBy(x => TryGetNumber(x.Item1))
                .Select(x => x.Item2);
            group.RecordCache.SetTo(
                x => x.FormKey, records);
        });
    }
}