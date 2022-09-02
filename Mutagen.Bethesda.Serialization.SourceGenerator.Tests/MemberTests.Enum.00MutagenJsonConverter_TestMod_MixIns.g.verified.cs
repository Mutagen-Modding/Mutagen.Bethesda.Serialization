//HintName: MutagenJsonConverter_TestMod_MixIns.g.cs
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Newtonsoft.Json.Linq;
namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public static class MutagenJsonConverterTestModMixIns
{
    private readonly static Mutagen.Bethesda.Serialization.Newtonsoft.NewtonsoftJsonSerializationReaderKernel ReaderKernel = new();
    private readonly static Mutagen.Bethesda.Serialization.Newtonsoft.NewtonsoftJsonSerializationWriterKernel WriterKernel = new();

    public static void Serialize(
        this Mutagen.Bethesda.Serialization.Newtonsoft.MutagenJsonConverter converterBootstrap,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter mod,
        Stream stream)
    {
        var writer = WriterKernel.GetNewObject(stream);
        TestMod_Serialization.Serialize<JTokenWriter>(mod, writer, WriterKernel);
        WriterKernel.Finalize(stream, writer);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize(
        this Mutagen.Bethesda.Serialization.Newtonsoft.MutagenJsonConverter converterBootstrap,
        Stream stream)
    {
        return TestMod_Serialization.Deserialize<JTokenReader>(ReaderKernel.GetNewObject(stream), ReaderKernel);
    }

}

