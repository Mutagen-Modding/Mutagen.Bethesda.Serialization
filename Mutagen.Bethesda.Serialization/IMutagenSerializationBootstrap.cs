namespace Mutagen.Bethesda.Serialization;

public interface IMutagenSerializationBootstrap<TReaderKernel, TReaderObject, TWriterKernel, TWriterObject>
    where TReaderKernel : ISerializationReaderKernel<TReaderObject>, new()
    where TWriterKernel : ISerializationWriterKernel<TWriterObject>, new()
{
}