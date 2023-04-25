using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public interface IPassthroughTest
{
    Task JsonSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff);
    Task<ISkyrimModGetter> JsonDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff);
    Task YamlSerialize(ISkyrimModGetter modGetter, StreamPackage stream, IWorkDropoff workDropoff);
    Task<ISkyrimModGetter> YamlDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release, IWorkDropoff workDropoff);
}