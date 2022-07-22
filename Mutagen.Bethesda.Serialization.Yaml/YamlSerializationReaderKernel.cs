using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationReaderKernel : ISerializationReaderKernel<YamlNode>
{
    public string GetString(YamlNode reader)
    {
        return reader.Anchor.Value;
    }
}