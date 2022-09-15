using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public partial class MutagenYamlConverter
    : IMutagenSerializationBootstrap<
        YamlSerializationReaderKernel, YamlNode,
        YamlSerializationWriterKernel, YamlWritingUnit>
{
    public static readonly MutagenYamlConverter Instance = new();
    
    private MutagenYamlConverter()
    {
    }
}