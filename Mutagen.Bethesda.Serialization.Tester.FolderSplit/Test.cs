using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Testing.Passthrough;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

public class Test : IPassthroughTest
{
    public async Task JsonSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff, ICreateStream streamCreator)
    {
        await MutagenJsonConverter.Instance.Serialize(modGetter, stream, workDropoff: workDropoff, streamCreator: streamCreator);
    }

    public async Task<ISkyrimModGetter> JsonDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff, ICreateStream streamCreator)
    {
        return await MutagenJsonConverter.Instance.Deserialize(stream, modKey, release, workDropoff: workDropoff, streamCreator: streamCreator);
    }

    public async Task YamlSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff, ICreateStream streamCreator)
    {
        await MutagenYamlConverter.Instance.Serialize(modGetter, stream, workDropoff: workDropoff, streamCreator: streamCreator);
    }

    public async Task<ISkyrimModGetter> YamlDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff, ICreateStream streamCreator)
    {
        return await MutagenYamlConverter.Instance.Deserialize(stream, modKey, release, workDropoff: workDropoff, streamCreator: streamCreator);
    }
}