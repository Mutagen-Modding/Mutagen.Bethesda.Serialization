//HintName: MutagenTestConverter_TestMod_MixIns.g.cs
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public static class MutagenTestConverterTestModMixIns
{
    private readonly static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel ReaderKernel = new();
    private readonly static MutagenSerializationWriterKernel<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestWriter> WriterKernel = new();

    public static void Serialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        Stream stream)
    {
        var writer = WriterKernel.GetNewObject(stream);
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod_Serialization.Serialize<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel, TestWriter>(writer, item, WriterKernel);
        WriterKernel.Finalize(stream, writer);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Stream stream)
    {
        return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod_Serialization.Deserialize<TestReader>(
            ReaderKernel.GetNewObject(stream),
            ReaderKernel);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod DeserializeInto(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Stream stream,
        ITestMod obj)
    {
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod_Serialization.DeserializeInto<TestReader>(
            ReaderKernel.GetNewObject(stream),
            ReaderKernel,
            obj: obj);
    }

}

