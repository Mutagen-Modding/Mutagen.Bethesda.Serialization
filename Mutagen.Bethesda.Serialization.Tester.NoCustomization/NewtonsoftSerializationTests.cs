using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests;

public class NewtonsoftSerializationTests : ASerializationTests
{
    public override async Task Serialize(ISkyrimModGetter mod, Stream stream, ICreateStream streamCreator)
    {
        await MutagenJsonConverter.Instance.Serialize(mod, stream, streamCreator: streamCreator);
    }

    public override async Task<ISkyrimModGetter> Deserialize(Stream stream, ModKey modKey, GameRelease release, ICreateStream streamCreator)
    {
        return await MutagenJsonConverter.Instance.Deserialize(stream, modKey, release.ToSkyrimRelease(), streamCreator: streamCreator);
    }
}