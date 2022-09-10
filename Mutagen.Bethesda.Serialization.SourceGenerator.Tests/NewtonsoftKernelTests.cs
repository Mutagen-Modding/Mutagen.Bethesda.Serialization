using Mutagen.Bethesda.Serialization.Newtonsoft;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

[UsesVerify]
public class NewtonsoftKernelTests : AKernelTest<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>
{
}