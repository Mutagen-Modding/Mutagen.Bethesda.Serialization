using Mutagen.Bethesda.Serialization.Newtonsoft;
using Newtonsoft.Json;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.KernelTests.Newtonsoft;

[UsesVerify]
public class NewtonsoftKernelTests : AKernelTest<
    NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit,
    NewtonsoftJsonSerializationReaderKernel, JsonTextReader>
{
}