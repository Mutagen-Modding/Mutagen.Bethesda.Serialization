using Mutagen.Bethesda.Serialization.Newtonsoft;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.KernelTests.Newtonsoft;

[UsesVerify]
public class NewtonsoftKernelTests : AKernelTest<
    NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit,
    NewtonsoftJsonSerializationReaderKernel, JsonReadingUnit>
{
}