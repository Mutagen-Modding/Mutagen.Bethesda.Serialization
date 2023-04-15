using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    private static void WriteMajor<TKernel, TWriteObject, TObject>(
        StreamPackage streamPackage,
        SerializationMetaData metaData, 
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TObject> itemWriter,
        TObject recordGetter,
        int? numbering) where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TObject : class, IMajorRecordGetter
        where TWriteObject : IContainStreamPackage
    {
        var fileName = FileNameProvider(recordGetter, kernel.ExpectedExtension, numbering);
        var recordPath = Path.Combine(streamPackage.Path!, fileName);
        using var stream = streamPackage.FileSystem.File.Create(recordPath);
        var recordStreamPackage = streamPackage with { Stream = stream };
        var recordWriter = kernel.GetNewObject(recordStreamPackage);
        itemWriter(recordWriter, recordGetter, kernel, metaData);
        kernel.Finalize(recordStreamPackage, recordWriter);
    }
}