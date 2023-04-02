namespace Mutagen.Bethesda.Serialization.Yaml;

public partial class MutagenYamlConverter
    : IMutagenSerializationBootstrap<
        YamlSerializationReaderKernel, YamlReadingUnit,
        YamlSerializationWriterKernel, YamlWritingUnit>
{
    public static readonly MutagenYamlConverter Instance = new();
    
    private MutagenYamlConverter()
    {
    }
}