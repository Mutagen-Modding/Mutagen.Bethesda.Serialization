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
        kernel.WriteUInt8(writer, "SomeMember0", item.SomeMember0, default(byte));
        kernel.WriteUInt8(writer, "SomeMember1", item.SomeMember1, default(Byte));
        kernel.WriteUInt8(writer, "SomeMember2", item.SomeMember2, default(UInt8));
        kernel.WriteUInt8(writer, "SomeMember3", item.SomeMember3, default(byte?));
        kernel.WriteUInt8(writer, "SomeMember4", item.SomeMember4, default(Byte?));
        kernel.WriteUInt8(writer, "SomeMember5", item.SomeMember5, default(UInt8?));
        kernel.WriteUInt8(writer, "SomeMember6", item.SomeMember6, default(Nullable<byte>));
        kernel.WriteUInt8(writer, "SomeMember7", item.SomeMember7, default(Nullable<Byte>));
        kernel.WriteUInt8(writer, "SomeMember8", item.SomeMember8, default(Nullable<UInt8>));
        kernel.WriteUInt8(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember9Default);
        kernel.WriteUInt8(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember10Default);
        kernel.WriteUInt8(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember11Default);
        kernel.WriteUInt8(writer, "SomeMember12", item.SomeMember12, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember12Default);
        kernel.WriteUInt8(writer, "SomeMember13", item.SomeMember13, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember13Default);
        kernel.WriteUInt8(writer, "SomeMember14", item.SomeMember14, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember14Default);
        kernel.WriteUInt8(writer, "SomeMember15", item.SomeMember15, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember15Default);
        kernel.WriteUInt8(writer, "SomeMember16", item.SomeMember16, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember16Default);
        kernel.WriteUInt8(writer, "SomeMember17", item.SomeMember17, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember17Default);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        if (!EqualityComparer<byte>.Default.Equals(item.SomeMember0, default(byte))) return true;
        if (!EqualityComparer<Byte>.Default.Equals(item.SomeMember1, default(Byte))) return true;
        if (!EqualityComparer<UInt8>.Default.Equals(item.SomeMember2, default(UInt8))) return true;
        if (!EqualityComparer<byte?>.Default.Equals(item.SomeMember3, default(byte?))) return true;
        if (!EqualityComparer<Byte?>.Default.Equals(item.SomeMember4, default(Byte?))) return true;
        if (!EqualityComparer<UInt8?>.Default.Equals(item.SomeMember5, default(UInt8?))) return true;
        if (!EqualityComparer<Nullable<byte>>.Default.Equals(item.SomeMember6, default(Nullable<byte>))) return true;
        if (!EqualityComparer<Nullable<Byte>>.Default.Equals(item.SomeMember7, default(Nullable<Byte>))) return true;
        if (!EqualityComparer<Nullable<UInt8>>.Default.Equals(item.SomeMember8, default(Nullable<UInt8>))) return true;
        if (!EqualityComparer<byte>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember9Default)) return true;
        if (!EqualityComparer<Byte>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember10Default)) return true;
        if (!EqualityComparer<UInt8>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember11Default)) return true;
        if (!EqualityComparer<byte?>.Default.Equals(item.SomeMember12, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember12Default)) return true;
        if (!EqualityComparer<Byte?>.Default.Equals(item.SomeMember13, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember13Default)) return true;
        if (!EqualityComparer<UInt8?>.Default.Equals(item.SomeMember14, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember14Default)) return true;
        if (!EqualityComparer<Nullable<byte>>.Default.Equals(item.SomeMember15, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember15Default)) return true;
        if (!EqualityComparer<Nullable<Byte>>.Default.Equals(item.SomeMember16, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember16Default)) return true;
        if (!EqualityComparer<Nullable<UInt8>>.Default.Equals(item.SomeMember17, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember17Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

