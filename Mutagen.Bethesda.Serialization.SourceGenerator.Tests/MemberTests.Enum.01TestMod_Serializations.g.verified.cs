//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.WriteEnum(writer, "SomeEnum", item.SomeEnum);
        kernel.WriteEnum(writer, "SomeEnum2", item.SomeEnum2);
        kernel.WriteEnum(writer, "SomeEnum3", item.SomeEnum3);
        kernel.WriteEnum(writer, "SomeEnum4", item.SomeEnum4);
        kernel.WriteEnum(writer, "SomeEnum5", item.SomeEnum5);
        kernel.WriteEnum(writer, "SomeEnum6", item.SomeEnum6);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

