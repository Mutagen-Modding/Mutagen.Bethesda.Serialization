using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Fallout4;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Tests;

public class YamlSerializationTests : ASerializationTests
{
    public override async Task Serialize(IFallout4ModGetter mod, Stream stream, ICreateStream streamCreator)
    {
        await MutagenYamlConverter.Instance.Serialize(mod, stream, streamCreator: streamCreator);
    }

    public override async Task<IFallout4ModGetter> Deserialize(Stream stream, ModKey modKey, GameRelease release, ICreateStream streamCreator)
    {
        return await MutagenYamlConverter.Instance.Deserialize(stream, modKey, release.ToFallout4Release(), streamCreator: streamCreator);
    }
}