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
        kernel.WriteInt8(writer, "SomeMember0", item.SomeMember0);
        kernel.WriteInt8(writer, "SomeMember1", item.SomeMember1);
        kernel.WriteInt8(writer, "SomeMember2", item.SomeMember2);
        kernel.WriteInt8(writer, "SomeMember3", item.SomeMember3);
        kernel.WriteInt8(writer, "SomeMember4", item.SomeMember4);
        kernel.WriteInt8(writer, "SomeMember5", item.SomeMember5);
        kernel.WriteInt8(writer, "SomeMember6", item.SomeMember6);
        kernel.WriteInt8(writer, "SomeMember7", item.SomeMember7);
        kernel.WriteInt8(writer, "SomeMember8", item.SomeMember8);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

