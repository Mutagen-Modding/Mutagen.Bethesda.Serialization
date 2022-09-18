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
        kernel.WriteUInt32(writer, "SomeMember0", item.SomeMember0, default(uint));
        kernel.WriteUInt32(writer, "SomeMember1", item.SomeMember1, default(UInt32));
        kernel.WriteUInt32(writer, "SomeMember2", item.SomeMember2, default(uint?));
        kernel.WriteUInt32(writer, "SomeMember3", item.SomeMember3, default(UInt32?));
        kernel.WriteUInt32(writer, "SomeMember4", item.SomeMember4, default(Nullable<uint>));
        kernel.WriteUInt32(writer, "SomeMember5", item.SomeMember5, default(Nullable<UInt32>));
        kernel.WriteUInt32(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember6Default);
        kernel.WriteUInt32(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember7Default);
        kernel.WriteUInt32(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember8Default);
        kernel.WriteUInt32(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember9Default);
        kernel.WriteUInt32(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember10Default);
        kernel.WriteUInt32(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember11Default);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        if (!EqualityComparer<uint>.Default.Equals(item.SomeMember0, default(uint))) return true;
        if (!EqualityComparer<UInt32>.Default.Equals(item.SomeMember1, default(UInt32))) return true;
        if (!EqualityComparer<uint?>.Default.Equals(item.SomeMember2, default(uint?))) return true;
        if (!EqualityComparer<UInt32?>.Default.Equals(item.SomeMember3, default(UInt32?))) return true;
        if (!EqualityComparer<Nullable<uint>>.Default.Equals(item.SomeMember4, default(Nullable<uint>))) return true;
        if (!EqualityComparer<Nullable<UInt32>>.Default.Equals(item.SomeMember5, default(Nullable<UInt32>))) return true;
        if (!EqualityComparer<uint>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember6Default)) return true;
        if (!EqualityComparer<UInt32>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember7Default)) return true;
        if (!EqualityComparer<uint?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember8Default)) return true;
        if (!EqualityComparer<UInt32?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<uint>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<UInt32>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

