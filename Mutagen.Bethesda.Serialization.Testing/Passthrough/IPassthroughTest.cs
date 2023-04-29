using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public interface IPassthroughTest
{
    Task JsonSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff, ICreateStream streamCreator);
    Task<ISkyrimModGetter> JsonDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff, ICreateStream streamCreator);
    Task YamlSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff, ICreateStream streamCreator);
    Task<ISkyrimModGetter> YamlDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff, ICreateStream streamCreator);
}