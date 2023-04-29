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

    public override async Task<ISkyrimModGetter> Deserialize(Stream stream, ICreateStream streamCreator)
    {
        return await MutagenJsonConverter.Instance.Deserialize(stream, ModKey, SkyrimRelease.SkyrimSE, streamCreator: streamCreator);
    }
}