namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class MutagenJsonConverter 
    : IMutagenSerializationBootstrap<
        NewtonsoftJsonSerializationReaderKernel, JsonReadingUnit,
        NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>
{
    public static readonly MutagenJsonConverter Instance = new();
    
    private MutagenJsonConverter()
    {
    }

    public Exception ConvertException(Exception ex)
    {
        return ex;
    }
}