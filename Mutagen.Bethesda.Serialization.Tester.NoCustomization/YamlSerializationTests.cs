using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests;

public class YamlSerializationTests : ASerializationTests
{
    public override async Task Serialize(ISkyrimModGetter mod, Stream stream, ICreateStream streamCreator)
    {
        await MutagenYamlConverter.Instance.Serialize(mod, stream, streamCreator: streamCreator);
    }

    public override async Task<ISkyrimModGetter> Deserialize(Stream stream, ICreateStream streamCreator)
    {
        return await MutagenYamlConverter.Instance.Deserialize(stream, ModKey, SkyrimRelease.SkyrimSE, streamCreator: streamCreator);
    }
}