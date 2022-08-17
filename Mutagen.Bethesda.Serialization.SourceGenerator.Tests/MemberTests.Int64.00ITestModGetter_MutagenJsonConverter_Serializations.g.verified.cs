//HintName: ITestModGetter_MutagenJsonConverter_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public static class ITestModGetter_MutagenJsonConverter_MixIns
{
    public static void Serialize(
        this Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        Newtonsoft.Json.Linq.JTokenWriter writer,
        ISerializationWriterKernel<Newtonsoft.Json.Linq.JTokenWriter> kernel)
    {
        kernel.WriteInt64(writer, item.SomeLong);
        kernel.WriteInt64(writer, item.SomeLong2);
    }
    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize(
        Newtonsoft.Json.Linq.JTokenReader reader,
        ISerializationReaderKernel<Newtonsoft.Json.Linq.JTokenReader> kernel)
    {
        throw new NotImplementedException();
    }
}

