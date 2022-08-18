//HintName: MutagenJsonConverter_ITestModGetter_MixIns.g.cs
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public static class MutagenJsonConverterITestModGetterMixIns
{
    private readonly static Mutagen.Bethesda.Serialization.Newtonsoft.NewtonsoftJsonSerializationReaderKernel ReaderKernel = new();
    private readonly static Mutagen.Bethesda.Serialization.Newtonsoft.NewtonsoftJsonSerializationWriterKernel WriterKernel = new();

    public static string Serialize(
        this Mutagen.Bethesda.Serialization.Newtonsoft.MutagenJsonConverter converterBootstrap,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter mod)
    {
        ITestModGetter_Serialization.Serialize<Newtonsoft.Json.Linq.JTokenWriter>(mod, WriterKernel.GetNewObject(), WriterKernel);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize(
        this Mutagen.Bethesda.Serialization.Newtonsoft.MutagenJsonConverter converterBootstrap,
        string str)
    {
        ITestModGetter_Serialization.Deserialize<Newtonsoft.Json.Linq.JTokenReader>(mod, ReaderKernel.GetNewObject(), ReaderKernel);
    }

}

