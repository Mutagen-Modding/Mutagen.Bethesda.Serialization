//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.WriteString(writer, "SomeFormKey", item.SomeFormKey?.RawPath);
        kernel.WriteString(writer, "SomeFormKey2", item.SomeFormKey2?.RawPath);
        kernel.WriteString(writer, "SomeFormKey3", item.SomeFormKey3?.RawPath);
        kernel.WriteString(writer, "SomeFormKey4", item.SomeFormKey4?.RawPath);
        kernel.WriteString(writer, "SomeFormKey5", item.SomeFormKey5?.RawPath);
        kernel.WriteString(writer, "SomeFormKey6", item.SomeFormKey6?.RawPath);
        kernel.WriteString(writer, "SomeFormKey7", item.SomeFormKey7?.RawPath);
        kernel.WriteString(writer, "SomeFormKey8", item.SomeFormKey8?.RawPath);
        kernel.WriteString(writer, "SomeFormKey9", item.SomeFormKey9?.RawPath);
        kernel.WriteString(writer, "SomeFormKey10", item.SomeFormKey10?.RawPath);
        kernel.WriteString(writer, "SomeFormKey11", item.SomeFormKey11?.RawPath);
        kernel.WriteString(writer, "SomeFormKey12", item.SomeFormKey12?.RawPath);
        kernel.WriteString(writer, "SomeFormKey13", item.SomeFormKey13?.RawPath);
        kernel.WriteString(writer, "SomeFormKey14", item.SomeFormKey14?.RawPath);
        kernel.WriteString(writer, "SomeFormKey15", item.SomeFormKey15?.RawPath);
        kernel.WriteString(writer, "SomeFormKey16", item.SomeFormKey16?.RawPath);
        kernel.WriteString(writer, "SomeFormKey17", item.SomeFormKey17?.RawPath);
        kernel.WriteString(writer, "SomeFormKey18", item.SomeFormKey18?.RawPath);
        kernel.WriteString(writer, "SomeFormKey19", item.SomeFormKey19?.RawPath);
        kernel.WriteString(writer, "SomeFormKey20", item.SomeFormKey20?.RawPath);
        kernel.WriteString(writer, "SomeFormKey21", item.SomeFormKey21?.RawPath);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

