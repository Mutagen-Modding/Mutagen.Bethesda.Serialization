namespace Mutagen.Bethesda.Serialization;

public interface ISerializationReaderKernel<TReaderObject>
{
    public string GetString(TReaderObject reader);
}