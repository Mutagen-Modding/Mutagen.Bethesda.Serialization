namespace Mutagen.Bethesda.Serialization;

public interface ISerializationReaderKernel<TReaderObject>
{
    public string GetString(TReaderObject reader);
}

public interface ISerializationWriterKernel<TWriterObject>
{
    public void WriteString(TWriterObject writer, string str);
}