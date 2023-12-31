//HintName: MutagenYamlConverterStubMixIn.g.cs
using Mutagen.Bethesda.Plugins.Records;
using System.IO;
using Loqui;

namespace Mutagen.Bethesda.Serialization.Yaml;

public static class MutagenYamlConverterMixIns
{
    public static async Task Serialize(
        this Mutagen.Bethesda.Serialization.Yaml.MutagenYamlConverter converterBootstrap,
        IModGetter mod,
        Stream stream)
    {
        throw new NotImplementedException();
    }

    public static async Task Serialize(
        this Mutagen.Bethesda.Serialization.Yaml.MutagenYamlConverter converterBootstrap,
        IModGetter mod,
        string path)
    {
        throw new NotImplementedException();
    }

    public static async Task<IModGetter> Deserialize(
        this Mutagen.Bethesda.Serialization.Yaml.MutagenYamlConverter converterBootstrap,
        Stream stream)
    {
        throw new NotImplementedException();
    }

}

