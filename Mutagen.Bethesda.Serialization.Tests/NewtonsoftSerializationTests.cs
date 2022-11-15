using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests;

public class NewtonsoftSerializationTests : ASerializationTests
{
    public override void Serialize(ISkyrimModGetter mod, Stream stream)
    {
        MutagenJsonConverter.Instance.Serialize(mod, stream);
    }

    public override ISkyrimModGetter Deserialize(Stream stream)
    {
        return MutagenJsonConverter.Instance.Deserialize(stream, ModKey, SkyrimRelease.SkyrimSE);
    }
}