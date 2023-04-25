using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests;

public class YamlSerializationTests : ASerializationTests
{
    public override async Task Serialize(ISkyrimModGetter mod, Stream stream)
    {
        await MutagenYamlConverter.Instance.Serialize(mod, stream);
    }

    public override async Task<ISkyrimModGetter> Deserialize(Stream stream)
    {
        return await MutagenYamlConverter.Instance.Deserialize(stream, ModKey, SkyrimRelease.SkyrimSE);
    }
}