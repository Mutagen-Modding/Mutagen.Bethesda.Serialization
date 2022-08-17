//HintName: ITestModGetter_MutagenJsonConverter_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class ITestModGetter_MutagenJsonConverter_Serialization
{
    public static void Serialize(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        Newtonsoft.Json.Linq.JTokenWriter writer,
        ISerializationWriterKernel<Newtonsoft.Json.Linq.JTokenWriter> kernel)
    {
        kernel.WriteUInt64(writer, item.SomeULong);
        kernel.WriteUInt64(writer, item.SomeULong2);
    }
    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize(
        Newtonsoft.Json.Linq.JTokenReader reader,
        ISerializationReaderKernel<Newtonsoft.Json.Linq.JTokenReader> kernel)
    {
        throw new NotImplementedException();
    }
}

