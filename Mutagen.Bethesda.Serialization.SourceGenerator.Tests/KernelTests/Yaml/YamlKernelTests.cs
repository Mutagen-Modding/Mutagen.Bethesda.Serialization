using Mutagen.Bethesda.Serialization.Yaml;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.KernelTests.Yaml;

[UsesVerify]
public class YamlKernelTests : AKernelTest<
    YamlSerializationWriterKernel, YamlWritingUnit, 
    YamlSerializationReaderKernel, YamlReadingUnit>
{
}