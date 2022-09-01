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
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum>(writer, "SomeEnum", item.SomeEnum);
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum>(writer, "SomeEnum2", item.SomeEnum2);
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2>(writer, "SomeEnum3", item.SomeEnum3);
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2>(writer, "SomeEnum4", item.SomeEnum4);
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3>(writer, "SomeEnum5", item.SomeEnum5);
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3>(writer, "SomeEnum6", item.SomeEnum6);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

