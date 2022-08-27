//HintName: ITestModGetter_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class ITestModGetter_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.WriteFormKey(writer, "SomeFormKey", item.SomeFormKey.FormKey);
        kernel.WriteFormKey(writer, "SomeFormKey2", item.SomeFormKey2.FormKey);
        kernel.WriteFormKey(writer, "SomeFormKey3", item.SomeFormKey3.FormKey);
        kernel.WriteFormKey(writer, "SomeFormKey4", item.SomeFormKey4.FormKey);
        kernel.WriteFormKey(writer, "SomeFormKey5", item.SomeFormKey5.FormKey);
        kernel.WriteFormKey(writer, "SomeFormKey6", item.SomeFormKey6.FormKey);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

