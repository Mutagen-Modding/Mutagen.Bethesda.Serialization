using Newtonsoft.Json.Linq;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class NewtonsoftJsonSerializationReaderKernel : ISerializationReaderKernel<JTokenReader>
{
    public string GetString(JTokenReader reader)
    {
        return reader.CurrentToken?.ToString() ?? String.Empty;
    }
}