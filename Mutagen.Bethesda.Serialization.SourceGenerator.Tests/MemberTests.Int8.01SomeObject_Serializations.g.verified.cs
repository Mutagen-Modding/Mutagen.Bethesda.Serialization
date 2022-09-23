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
        kernel.WriteInt8(writer, "SomeMember0", item.SomeMember0, default(sbyte));
        kernel.WriteInt8(writer, "SomeMember1", item.SomeMember1, default(SByte));
        kernel.WriteInt8(writer, "SomeMember2", item.SomeMember2, default(Int8));
        kernel.WriteInt8(writer, "SomeMember3", item.SomeMember3, default(sbyte?));
        kernel.WriteInt8(writer, "SomeMember4", item.SomeMember4, default(SByte?));
        kernel.WriteInt8(writer, "SomeMember5", item.SomeMember5, default(Int8?));
        kernel.WriteInt8(writer, "SomeMember6", item.SomeMember6, default(Nullable<sbyte>));
        kernel.WriteInt8(writer, "SomeMember7", item.SomeMember7, default(Nullable<SByte>));
        kernel.WriteInt8(writer, "SomeMember8", item.SomeMember8, default(Nullable<Int8>));
        kernel.WriteInt8(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteInt8(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteInt8(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
        kernel.WriteInt8(writer, "SomeMember12", item.SomeMember12, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember12Default);
        kernel.WriteInt8(writer, "SomeMember13", item.SomeMember13, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember13Default);
        kernel.WriteInt8(writer, "SomeMember14", item.SomeMember14, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember14Default);
        kernel.WriteInt8(writer, "SomeMember15", item.SomeMember15, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember15Default);
        kernel.WriteInt8(writer, "SomeMember16", item.SomeMember16, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember16Default);
        kernel.WriteInt8(writer, "SomeMember17", item.SomeMember17, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember17Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<sbyte>.Default.Equals(item.SomeMember0, default(sbyte))) return true;
        if (!EqualityComparer<SByte>.Default.Equals(item.SomeMember1, default(SByte))) return true;
        if (!EqualityComparer<Int8>.Default.Equals(item.SomeMember2, default(Int8))) return true;
        if (!EqualityComparer<sbyte?>.Default.Equals(item.SomeMember3, default(sbyte?))) return true;
        if (!EqualityComparer<SByte?>.Default.Equals(item.SomeMember4, default(SByte?))) return true;
        if (!EqualityComparer<Int8?>.Default.Equals(item.SomeMember5, default(Int8?))) return true;
        if (!EqualityComparer<Nullable<sbyte>>.Default.Equals(item.SomeMember6, default(Nullable<sbyte>))) return true;
        if (!EqualityComparer<Nullable<SByte>>.Default.Equals(item.SomeMember7, default(Nullable<SByte>))) return true;
        if (!EqualityComparer<Nullable<Int8>>.Default.Equals(item.SomeMember8, default(Nullable<Int8>))) return true;
        if (!EqualityComparer<sbyte>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<SByte>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Int8>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
        if (!EqualityComparer<sbyte?>.Default.Equals(item.SomeMember12, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember12Default)) return true;
        if (!EqualityComparer<SByte?>.Default.Equals(item.SomeMember13, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember13Default)) return true;
        if (!EqualityComparer<Int8?>.Default.Equals(item.SomeMember14, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember14Default)) return true;
        if (!EqualityComparer<Nullable<sbyte>>.Default.Equals(item.SomeMember15, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember15Default)) return true;
        if (!EqualityComparer<Nullable<SByte>>.Default.Equals(item.SomeMember16, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember16Default)) return true;
        if (!EqualityComparer<Nullable<Int8>>.Default.Equals(item.SomeMember17, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember17Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        while (kernel.TryGetNextField(out var name))
        {
            switch (name)
            {
                case: "SomeMember0":
                    item.SomeMember0 = kernel.ReadInt8(writer);
                case: "SomeMember1":
                    item.SomeMember1 = kernel.ReadInt8(writer);
                case: "SomeMember2":
                    item.SomeMember2 = kernel.ReadInt8(writer);
                case: "SomeMember3":
                    item.SomeMember3 = kernel.ReadInt8(writer);
                case: "SomeMember4":
                    item.SomeMember4 = kernel.ReadInt8(writer);
                case: "SomeMember5":
                    item.SomeMember5 = kernel.ReadInt8(writer);
                case: "SomeMember6":
                    item.SomeMember6 = kernel.ReadInt8(writer);
                case: "SomeMember7":
                    item.SomeMember7 = kernel.ReadInt8(writer);
                case: "SomeMember8":
                    item.SomeMember8 = kernel.ReadInt8(writer);
                case: "SomeMember9":
                    item.SomeMember9 = kernel.ReadInt8(writer);
                case: "SomeMember10":
                    item.SomeMember10 = kernel.ReadInt8(writer);
                case: "SomeMember11":
                    item.SomeMember11 = kernel.ReadInt8(writer);
                case: "SomeMember12":
                    item.SomeMember12 = kernel.ReadInt8(writer);
                case: "SomeMember13":
                    item.SomeMember13 = kernel.ReadInt8(writer);
                case: "SomeMember14":
                    item.SomeMember14 = kernel.ReadInt8(writer);
                case: "SomeMember15":
                    item.SomeMember15 = kernel.ReadInt8(writer);
                case: "SomeMember16":
                    item.SomeMember16 = kernel.ReadInt8(writer);
                case: "SomeMember17":
                    item.SomeMember17 = kernel.ReadInt8(writer);
                default:
                    break;
            }
        }
    }

}

