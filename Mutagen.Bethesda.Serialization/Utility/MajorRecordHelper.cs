using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    private static async Task WriteMajor<TKernel, TWriteObject, TObject>(
        StreamPackage streamPackage,
        SerializationMetaData metaData, 
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TObject> itemWriter,
        TObject recordGetter,
        int? numbering) where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TObject : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        var fileName = RecordFileNameProvider(recordGetter, kernel.ExpectedExtension, numbering);
        var recordPath = Path.Combine(streamPackage.Path!, fileName);
        using var stream = streamPackage.FileSystem.File.Create(recordPath);
        var recordStreamPackage = streamPackage with { Stream = stream };
        var recordWriter = kernel.GetNewObject(recordStreamPackage);
        await itemWriter(recordWriter, recordGetter, kernel, metaData);
        kernel.Finalize(recordStreamPackage, recordWriter);
    }
}