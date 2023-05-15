namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task MetaWriter<TKernel, TWriteObject, TMeta>(
        TWriteObject writer,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData,
        TMeta? meta,
        WriteAsync<TKernel, TWriteObject, TMeta>? metaWriter)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        if (meta == null || metaWriter == null) return;
        await kernel.WriteLoqui(writer, nameof(TMeta), meta, metaData, metaWriter);
    }
}