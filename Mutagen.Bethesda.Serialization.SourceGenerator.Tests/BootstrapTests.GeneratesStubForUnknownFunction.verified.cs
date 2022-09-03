//HintName: MutagenJsonConverterStubMixIn.g.cs
using Mutagen.Bethesda.Plugins.Records;
using Loqui;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public static class MutagenJsonConverterMixIns
{
    public static void Serialize(
        this Mutagen.Bethesda.Serialization.Newtonsoft.MutagenJsonConverter converterBootstrap,
        ILoquiObject mod,
        Stream stream)
    {
        throw new NotImplementedException();
    }

    public static ILoquiObject Deserialize(
        this Mutagen.Bethesda.Serialization.Newtonsoft.MutagenJsonConverter converterBootstrap,
        Stream stream)
    {
        throw new NotImplementedException();
    }

}

