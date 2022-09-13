//HintName: MutagenTestConverter_SomeLoqui_MixIns.g.cs
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public static class MutagenTestConverterSomeLoquiMixIns
{
    private readonly static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel ReaderKernel = new();
    private readonly static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel WriterKernel = new();

    public static void Serialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiGetter item,
        Stream stream)
    {
        var writer = WriterKernel.GetNewObject(stream);
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TestWriter>(writer, item, WriterKernel);
        WriterKernel.Finalize(stream, writer);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoqui Deserialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Stream stream)
    {
        return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TestReader>(ReaderKernel.GetNewObject(stream), ReaderKernel);
    }

}

