//HintName: TestMod_Serializations.g.cs
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
        kernel.WriteP3Int16(writer, "SomeMember0", item.SomeMember0, default(Noggog.P3Int16));
        kernel.WriteP3Int16(writer, "SomeMember1", item.SomeMember1, default(Noggog.P3Int16?));
        kernel.WriteP3Int16(writer, "SomeMember2", item.SomeMember2, default(Nullable<Noggog.P3Int16>));
        kernel.WriteP3Int16(writer, "SomeMember3", item.SomeMember3, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember3Default);
        kernel.WriteP3Int16(writer, "SomeMember4", item.SomeMember4, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember4Default);
        kernel.WriteP3Int16(writer, "SomeMember5", item.SomeMember5, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember5Default);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

