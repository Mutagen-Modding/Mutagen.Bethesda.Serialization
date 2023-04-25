using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Testing.Passthrough;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

public class Test : IPassthroughTest
{
    public async Task JsonSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff)
    {
        await MutagenJsonConverter.Instance.Serialize(modGetter, stream, workDropoff: workDropoff);
    }

    public async Task<ISkyrimModGetter> JsonDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff)
    {
        return await MutagenJsonConverter.Instance.Deserialize(stream, modKey, release, workDropoff: workDropoff);
    }

    public async Task YamlSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff)
    {
        await MutagenYamlConverter.Instance.Serialize(modGetter, stream, workDropoff: workDropoff);
    }

    public async Task<ISkyrimModGetter> YamlDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff)
    {
        return await MutagenYamlConverter.Instance.Deserialize(stream, modKey, release, workDropoff: workDropoff);
    }
}