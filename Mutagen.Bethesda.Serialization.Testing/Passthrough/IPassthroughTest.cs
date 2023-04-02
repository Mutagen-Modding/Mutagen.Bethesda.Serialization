using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public interface IPassthroughTest
{
    void JsonSerialize(ISkyrimModGetter modGetter, StreamPackage stream);
    ISkyrimModGetter JsonDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release);
    void YamlSerialize(ISkyrimModGetter modGetter, StreamPackage stream);
    ISkyrimModGetter YamlDeserialize(StreamPackage stream, ModKey modKey, SkyrimRelease release);
}