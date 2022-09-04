//HintName: MutagenTestConverterStubMixIn.g.cs
using Mutagen.Bethesda.Plugins.Records;
using Loqui;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public static class MutagenTestConverterMixIns
{
    public static void Serialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        ILoquiObject mod,
        Stream stream)
    {
        throw new NotImplementedException();
    }

    public static ILoquiObject Deserialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Stream stream)
    {
        throw new NotImplementedException();
    }

}

