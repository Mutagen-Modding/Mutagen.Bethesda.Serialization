using YamlDotNet.Core;

namespace Mutagen.Bethesda.Serialization.Yaml;

public partial class MutagenYamlConverter
    : IMutagenSerializationBootstrap<
        YamlSerializationReaderKernel, Parser,
        YamlSerializationWriterKernel, YamlWritingUnit>
{
    public static readonly MutagenYamlConverter Instance = new();
    
    private MutagenYamlConverter()
    {
    }
}