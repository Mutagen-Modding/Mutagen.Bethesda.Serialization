using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests;

public class YamlSerializationTests : ASerializationTests
{
    public override void Serialize(ISkyrimModGetter mod, Stream stream)
    {
        MutagenYamlConverter.Instance.Serialize(mod, stream);
    }

    public override ISkyrimModGetter Deserialize(Stream stream)
    {
        return MutagenYamlConverter.Instance.Deserialize(stream, ModKey, SkyrimRelease.SkyrimSE);
    }
}