using Mutagen.Bethesda.Serialization.Yaml;
using YamlDotNet.Core;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.KernelTests.Yaml;

[UsesVerify]
public class YamlKernelTests : AKernelTest<
    YamlSerializationWriterKernel, YamlWritingUnit, 
    YamlSerializationReaderKernel, Parser>
{
}