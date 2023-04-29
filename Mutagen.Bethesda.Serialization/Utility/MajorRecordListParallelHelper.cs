using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task WriteMajorRecordList<TKernel, TWriteObject, TMajorGetter>(
        StreamPackage streamPackage,
        string? fieldName, 
        IReadOnlyList<TMajorGetter> list,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TMajorGetter> itemWriter,
        bool withNumbering)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TMajorGetter : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));

        if (list.Count == 0) return;
        
        var dir = Path.Combine(streamPackage.Path!, fieldName);
        metaData.FileSystem.Directory.CreateDirectory(dir);
        streamPackage = streamPackage with { Stream = null!, Path = dir };

        await metaData.WorkDropoff.EnqueueAndWait(
            list.WithIndex(),
            async recordGetter =>
            {
                await WriteMajor(
                    streamPackage,
                    metaData, 
                    kernel,
                    itemWriter,
                    recordGetter.Item,
                    withNumbering ? recordGetter.Index : null);
            });
    }
    
    public static async Task ReadMajorRecordList<TKernel, TReadObject, TMajorRecord>(
        StreamPackage streamPackage,
        string? fieldName, 
        IList<TMajorRecord> list,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadAsync<TKernel, TReadObject, TMajorRecord> itemReader)
        where TMajorRecord : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));
        
        var dir = Path.Combine(streamPackage.Path!, fieldName);
        
        if (!metaData.FileSystem.Directory.Exists(dir))
        {
            list.Clear();
            return;
        }
        
        streamPackage = streamPackage with { Stream = null!, Path = dir };
        
        var groupHeaderFileName = TypicalGroupFileName(kernel.ExpectedExtension);
        
        var records = await metaData.WorkDropoff.EnqueueAndWait(
            metaData.FileSystem.Directory.GetFiles(streamPackage.Path!)
                .Where(x =>
                {
                    var fileName = Path.GetFileName(x.AsSpan());
                    var ext = Path.GetExtension(fileName);
                    return ext.Equals(kernel.ExpectedExtension.AsSpan(), StringComparison.OrdinalIgnoreCase)
                           && !fileName.Equals(groupHeaderFileName.AsSpan(), StringComparison.OrdinalIgnoreCase);
                }),
            async x =>
            {
                using var stream = metaData.FileSystem.File.OpenRead(x);

                var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

                return (Path.GetFileName(x), await itemReader(reader, kernel, metaData));
            });
        list.SetTo(records
            .OrderBy(x => TryGetNumber(x.Item1))
            .Select(x => x.Item2));
    }
}