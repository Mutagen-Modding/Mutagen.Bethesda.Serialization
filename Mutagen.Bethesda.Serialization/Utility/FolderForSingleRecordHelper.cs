using Mutagen.Bethesda.Serialization.Streams;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task WriteRecordAsFolder<TKernel, TWriteObject, TObject>(
        StreamPackage streamPackage,
        TObject? obj,
        string? fieldName,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TObject> itemWriter)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        if (fieldName == null) throw new ArgumentNullException(paramName: nameof(fieldName));
        if (obj == null) return;
        
        await WriteGroupRecordData(
            streamPackage: streamPackage,
            @group: obj,
            folderName: fieldName,
            fileName: RecordDataFileName(kernel.ExpectedExtension),
            metaData: metaData,
            kernel: kernel,
            groupWriter: itemWriter);
    }
    
    public static async Task<TObject?> ReadRecordAsFolder<TKernel, TReadObject, TObject>(
        StreamPackage streamPackage,
        string? fieldName,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadAsync<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        if (streamPackage.Path == null) return default;
        if (fieldName != null)
        {
            var subDir = Path.Combine(streamPackage.Path, fieldName);
            streamPackage = streamPackage with { Path = subDir };
        }
        
        if (!metaData.FileSystem.Directory.Exists(streamPackage.Path)) return default;
        var recordPath = Path.Combine(streamPackage.Path!, RecordDataFileName(kernel.ExpectedExtension));

        using var stream = metaData.FileSystem.File.OpenRead(recordPath);

        var reader = kernel.GetNewObject(streamPackage with { Stream = stream });

        return await itemReader(reader, kernel, metaData);
    }
}