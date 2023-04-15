using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static void WriteMajorRecordList<TKernel, TWriteObject, TMajorGetter>(
        StreamPackage streamPackage,
        string? fieldName, 
        IReadOnlyList<TMajorGetter> list,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TMajorGetter> itemWriter,
        bool withNumbering,
        List<Action> toDo)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TMajorGetter : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));

        if (list.Count == 0) return;
        
        var dir = Path.Combine(streamPackage.Path!, fieldName);
        streamPackage.FileSystem.Directory.CreateDirectory(dir);

        int? i = withNumbering ? 1 : null;
        foreach (var recordGetter in list)
        {
            int? number = i;
            toDo.Add(() =>
            {
                WriteMajor(
                    streamPackage with { Stream = null!, Path = dir },
                    metaData, 
                    kernel,
                    itemWriter,
                    recordGetter,
                    number);
            });
            i++;
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
                .ToArray()
                .OrderBy(x => x)
                .Select(x => x.Item2);
            list.SetTo(records);
        });
    }
}