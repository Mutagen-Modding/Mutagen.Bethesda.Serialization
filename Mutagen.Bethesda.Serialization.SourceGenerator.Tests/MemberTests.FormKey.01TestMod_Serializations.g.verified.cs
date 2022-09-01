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
        kernel.WriteFormKey(writer, "SomeMember0", item.SomeMember0);
        kernel.WriteFormKey(writer, "SomeMember1", item.SomeMember1);
        kernel.WriteFormKey(writer, "SomeMember2", item.SomeMember2);
        kernel.WriteFormKey(writer, "SomeMember3", item.SomeMember3);
        kernel.WriteFormKey(writer, "SomeMember4", item.SomeMember4);
        kernel.WriteFormKey(writer, "SomeMember5", item.SomeMember5);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

