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
        kernel.WriteRecordType(writer, "SomeMember0", item.SomeMember0);
        kernel.WriteRecordType(writer, "SomeMember1", item.SomeMember1);
        kernel.WriteRecordType(writer, "SomeMember2", item.SomeMember2);
        kernel.WriteRecordType(writer, "SomeMember3", item.SomeMember3);
        kernel.WriteRecordType(writer, "SomeMember4", item.SomeMember4);
        kernel.WriteRecordType(writer, "SomeMember5", item.SomeMember5);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

