using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationWriterKernel : ISerializationWriterKernel<YamlNode>
{
    public void WriteString(YamlNode writer, string str)
    {
        throw new NotImplementedException();
    }
}