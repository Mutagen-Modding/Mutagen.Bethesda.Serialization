using Newtonsoft.Json.Linq;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class MutagenJsonConverter : IMutagenSerializationBootstrap<NewtonsoftJsonSerializationReaderKernel, JTokenReader>
{
    public static readonly MutagenJsonConverter Instance = new();
    
    private MutagenJsonConverter()
    {
    }
}