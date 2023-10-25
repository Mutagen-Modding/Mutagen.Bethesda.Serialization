using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Fallout4;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Tests;

public class NewtonsoftSerializationTests : ASerializationTests
{
    public override async Task Serialize(IFallout4ModGetter mod, Stream stream, ICreateStream streamCreator)
    {
        await MutagenJsonConverter.Instance.Serialize(mod, stream, streamCreator: streamCreator);
    }

    public override async Task<IFallout4ModGetter> Deserialize(Stream stream, ModKey modKey, GameRelease release, ICreateStream streamCreator)
    {
        return await MutagenJsonConverter.Instance.Deserialize(stream, modKey, release.ToFallout4Release(), streamCreator: streamCreator);
    }
}