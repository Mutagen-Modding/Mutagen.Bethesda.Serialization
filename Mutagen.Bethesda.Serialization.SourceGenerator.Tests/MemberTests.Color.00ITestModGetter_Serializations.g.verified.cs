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
        kernel.WriteColor(writer, "SomeMember0", item.SomeMember0);
        kernel.WriteColor(writer, "SomeMember1", item.SomeMember1);
        kernel.WriteColor(writer, "SomeMember2", item.SomeMember2);
        kernel.WriteColor(writer, "SomeMember3", item.SomeMember3);
        kernel.WriteColor(writer, "SomeMember4", item.SomeMember4);
        kernel.WriteColor(writer, "SomeMember5", item.SomeMember5);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

