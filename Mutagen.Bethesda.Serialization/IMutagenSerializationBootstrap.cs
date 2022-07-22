namespace Mutagen.Bethesda.Serialization;

public interface IMutagenSerializationBootstrap<TReaderKernel, TReaderObject>
    where TReaderKernel : ISerializationReaderKernel<TReaderObject>
{
}