namespace Mutagen.Bethesda.Serialization;

public interface IMutagenSerializationBootstrap<TReaderKernel, TReaderObject, TWriterKernel, TWriterObject>
    where TReaderKernel : ISerializationReaderKernel<TReaderObject>
    where TWriterKernel : ISerializationWriterKernel<TWriterObject>
{
}