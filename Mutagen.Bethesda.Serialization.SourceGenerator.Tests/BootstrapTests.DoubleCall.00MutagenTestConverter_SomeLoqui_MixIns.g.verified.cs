//HintName: MutagenTestConverter_SomeLoqui_MixIns.g.cs
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public static class MutagenTestConverterSomeLoquiMixIns
{
    private readonly static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel ReaderKernel = new();
    private readonly static MutagenSerializationWriterKernel<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestWriter> WriterKernel = new();

    public static void Serialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiGetter item,
        Stream stream)
    {
        var writer = WriterKernel.GetNewObject(stream);
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestKernel, TestWriter>(writer, item, WriterKernel);
        WriterKernel.Finalize(stream, writer);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoqui Deserialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Stream stream,
        SerializationMetaData metaData)
    {
        return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TestReader>(
            ReaderKernel.GetNewObject(stream),
            ReaderKernel,
            metaData: metaData);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoqui DeserializeInto(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MutagenTestConverter converterBootstrap,
        Stream stream,
        ISomeLoqui obj,
        SerializationMetaData metaData)
    {
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.DeserializeInto<TestReader>(
            ReaderKernel.GetNewObject(stream),
            ReaderKernel,
            obj: obj,
            metaData: metaData);
    }

}

