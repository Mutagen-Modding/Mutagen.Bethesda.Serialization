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
        kernel.WriteFloat(writer, "SomeMember0", item.SomeMember0);
        kernel.WriteFloat(writer, "SomeMember1", item.SomeMember1);
        kernel.WriteFloat(writer, "SomeMember2", item.SomeMember2);
        kernel.WriteFloat(writer, "SomeMember3", item.SomeMember3);
        kernel.WriteFloat(writer, "SomeMember4", item.SomeMember4);
        kernel.WriteFloat(writer, "SomeMember5", item.SomeMember5);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

