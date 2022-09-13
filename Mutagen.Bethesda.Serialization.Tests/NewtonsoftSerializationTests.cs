using Mutagen.Bethesda.Serialization.Newtonsoft;
using Newtonsoft.Json.Linq;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class NewtonsoftSerializationTests : ASerializationTests<
    NewtonsoftJsonSerializationReaderKernel, 
    JTokenReader,
    NewtonsoftJsonSerializationWriterKernel,
    JsonWritingUnit>
{
}