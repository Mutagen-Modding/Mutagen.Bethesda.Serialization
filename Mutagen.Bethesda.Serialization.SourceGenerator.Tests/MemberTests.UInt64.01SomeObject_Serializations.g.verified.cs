//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteUInt64(writer, "SomeMember0", item.SomeMember0, default(ulong));
        kernel.WriteUInt64(writer, "SomeMember1", item.SomeMember1, default(UInt64));
        kernel.WriteUInt64(writer, "SomeMember2", item.SomeMember2, default(ulong?));
        kernel.WriteUInt64(writer, "SomeMember3", item.SomeMember3, default(UInt64?));
        kernel.WriteUInt64(writer, "SomeMember4", item.SomeMember4, default(Nullable<ulong>));
        kernel.WriteUInt64(writer, "SomeMember5", item.SomeMember5, default(Nullable<UInt64>));
        kernel.WriteUInt64(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteUInt64(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteUInt64(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteUInt64(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteUInt64(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteUInt64(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<ulong>.Default.Equals(item.SomeMember0, default(ulong))) return true;
        if (!EqualityComparer<UInt64>.Default.Equals(item.SomeMember1, default(UInt64))) return true;
        if (!EqualityComparer<ulong?>.Default.Equals(item.SomeMember2, default(ulong?))) return true;
        if (!EqualityComparer<UInt64?>.Default.Equals(item.SomeMember3, default(UInt64?))) return true;
        if (!EqualityComparer<Nullable<ulong>>.Default.Equals(item.SomeMember4, default(Nullable<ulong>))) return true;
        if (!EqualityComparer<Nullable<UInt64>>.Default.Equals(item.SomeMember5, default(Nullable<UInt64>))) return true;
        if (!EqualityComparer<ulong>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<UInt64>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<ulong?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<UInt64?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<ulong>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<UInt64>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case: "SomeMember0":
                    item.SomeMember0 = kernel.ReadUInt64(writer);
                case: "SomeMember1":
                    item.SomeMember1 = kernel.ReadUInt64(writer);
                case: "SomeMember2":
                    item.SomeMember2 = kernel.ReadUInt64(writer);
                case: "SomeMember3":
                    item.SomeMember3 = kernel.ReadUInt64(writer);
                case: "SomeMember4":
                    item.SomeMember4 = kernel.ReadUInt64(writer);
                case: "SomeMember5":
                    item.SomeMember5 = kernel.ReadUInt64(writer);
                case: "SomeMember6":
                    item.SomeMember6 = kernel.ReadUInt64(writer);
                case: "SomeMember7":
                    item.SomeMember7 = kernel.ReadUInt64(writer);
                case: "SomeMember8":
                    item.SomeMember8 = kernel.ReadUInt64(writer);
                case: "SomeMember9":
                    item.SomeMember9 = kernel.ReadUInt64(writer);
                case: "SomeMember10":
                    item.SomeMember10 = kernel.ReadUInt64(writer);
                case: "SomeMember11":
                    item.SomeMember11 = kernel.ReadUInt64(writer);
                default:
                    break;
            }
        }
    }

}

