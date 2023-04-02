using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Testing.Passthrough;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tester.NoCustomization;

public class Test : IPassthroughTest
{
    public void JsonSerialize(ISkyrimModGetter modGetter, StreamPackage stream)
    {
        MutagenJsonConverter.Instance.Serialize(modGetter, stream.Stream);
    }

    public ISkyrimModGetter JsonDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release)
    {
        return MutagenJsonConverter.Instance.Deserialize(stream.Stream, modKey, release);
    }

    public void YamlSerialize(ISkyrimModGetter modGetter, StreamPackage stream)
    {
        MutagenYamlConverter.Instance.Serialize(modGetter, stream.Stream);
    }

    public ISkyrimModGetter YamlDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release)
    {
        return MutagenYamlConverter.Instance.Deserialize(stream.Stream, modKey, release);
    }
}