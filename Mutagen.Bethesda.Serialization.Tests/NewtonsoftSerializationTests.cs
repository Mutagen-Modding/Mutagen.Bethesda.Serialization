using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests;

public class NewtonsoftSerializationTests : ASerializationTests
{
    public override async Task Serialize(ISkyrimModGetter mod, Stream stream)
    {
        await MutagenJsonConverter.Instance.Serialize(mod, stream);
    }

    public override async Task<ISkyrimModGetter> Deserialize(Stream stream)
    {
        return await MutagenJsonConverter.Instance.Deserialize(stream, ModKey, SkyrimRelease.SkyrimSE);
    }
}