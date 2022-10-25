using Newtonsoft.Json;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class MutagenJsonConverter 
    : IMutagenSerializationBootstrap<
        NewtonsoftJsonSerializationReaderKernel, JsonTextReader,
        NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>
{
    public static readonly MutagenJsonConverter Instance = new();
    
    private MutagenJsonConverter()
    {
    }
}