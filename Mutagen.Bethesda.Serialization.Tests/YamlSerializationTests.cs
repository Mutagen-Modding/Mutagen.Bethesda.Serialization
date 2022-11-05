using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests;

public class YamlSerializationTests : ASerializationTests
{
    public override void Serialize(SkyrimMod mod, Stream stream)
    {
        MutagenYamlConverter.Instance.Serialize(mod, stream);
    }

    public override ISkyrimModGetter Deserialize(Stream stream)
    {
        return MutagenYamlConverter.Instance.Deserialize(stream, OutputModKey, SkyrimRelease.SkyrimSE);
    }
}