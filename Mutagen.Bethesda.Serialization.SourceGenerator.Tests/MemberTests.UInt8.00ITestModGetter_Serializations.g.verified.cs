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
        kernel.WriteUInt8(writer, item.SomeMember0);
        kernel.WriteUInt8(writer, item.SomeMember1);
        kernel.WriteUInt8(writer, item.SomeMember2);
        kernel.WriteUInt8(writer, item.SomeMember3);
        kernel.WriteUInt8(writer, item.SomeMember4);
        kernel.WriteUInt8(writer, item.SomeMember5);
        kernel.WriteUInt8(writer, item.SomeMember6);
        kernel.WriteUInt8(writer, item.SomeMember7);
        kernel.WriteUInt8(writer, item.SomeMember8);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

