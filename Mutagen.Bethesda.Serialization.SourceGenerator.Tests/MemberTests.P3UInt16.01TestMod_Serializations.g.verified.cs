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
        kernel.WriteP3UInt16(writer, "SomeMember0", item.SomeMember0, default(Noggog.P3UInt16));
        kernel.WriteP3UInt16(writer, "SomeMember1", item.SomeMember1, default(Noggog.P3UInt16?));
        kernel.WriteP3UInt16(writer, "SomeMember2", item.SomeMember2, default(Nullable<Noggog.P3UInt16>));
        kernel.WriteP3UInt16(writer, "SomeMember3", item.SomeMember3, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember3Default);
        kernel.WriteP3UInt16(writer, "SomeMember4", item.SomeMember4, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember4Default);
        kernel.WriteP3UInt16(writer, "SomeMember5", item.SomeMember5, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember5Default);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        if (!EqualityComparer<Noggog.P3UInt16>.Default.Equals(item.SomeMember0, default(Noggog.P3UInt16))) return true;
        if (!EqualityComparer<Noggog.P3UInt16?>.Default.Equals(item.SomeMember1, default(Noggog.P3UInt16?))) return true;
        if (!EqualityComparer<Nullable<Noggog.P3UInt16>>.Default.Equals(item.SomeMember2, default(Nullable<Noggog.P3UInt16>))) return true;
        if (!EqualityComparer<Noggog.P3UInt16>.Default.Equals(item.SomeMember3, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember3Default)) return true;
        if (!EqualityComparer<Noggog.P3UInt16?>.Default.Equals(item.SomeMember4, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember4Default)) return true;
        if (!EqualityComparer<Nullable<Noggog.P3UInt16>>.Default.Equals(item.SomeMember5, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember5Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

