using Newtonsoft.Json.Linq;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class NewtonsoftJsonSerializationWriterKernel : ISerializationWriterKernel<JTokenWriter>
{
    public void WriteString(JTokenWriter writer, string str)
    {
        throw new NotImplementedException();
    }
}