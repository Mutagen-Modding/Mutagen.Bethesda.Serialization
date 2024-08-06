using YamlDotNet.Core;

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

    public Exception ConvertException(Exception ex)
    {
        if (ex is not YamlException yamlEx) return ex;
        return new SpriggitYamlException(yamlEx);
    }
}