//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteFormKey(writer, "SomeFormKey", item.SomeFormKey.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey2", item.SomeFormKey2.FormKeyNullable, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeFormKey3", item.SomeFormKey3.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey4", item.SomeFormKey4.FormKeyNullable, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeFormKey5", item.SomeFormKey5.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey6", item.SomeFormKey6.FormKeyNullable, default(FormKey?));
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

