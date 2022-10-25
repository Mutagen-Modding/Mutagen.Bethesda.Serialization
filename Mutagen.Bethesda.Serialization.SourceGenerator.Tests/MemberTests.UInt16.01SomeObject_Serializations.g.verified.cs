//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

#nullable enable

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
        kernel.WriteUInt16(writer, "SomeMember0", item.SomeMember0, default(ushort));
        kernel.WriteUInt16(writer, "SomeMember1", item.SomeMember1, default(UInt16));
        kernel.WriteUInt16(writer, "SomeMember2", item.SomeMember2, default(ushort?));
        kernel.WriteUInt16(writer, "SomeMember3", item.SomeMember3, default(UInt16?));
        kernel.WriteUInt16(writer, "SomeMember4", item.SomeMember4, default(Nullable<ushort>));
        kernel.WriteUInt16(writer, "SomeMember5", item.SomeMember5, default(Nullable<UInt16>));
        kernel.WriteUInt16(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteUInt16(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteUInt16(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteUInt16(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteUInt16(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteUInt16(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<ushort>.Default.Equals(item.SomeMember0, default(ushort))) return true;
        if (!EqualityComparer<UInt16>.Default.Equals(item.SomeMember1, default(UInt16))) return true;
        if (!EqualityComparer<ushort?>.Default.Equals(item.SomeMember2, default(ushort?))) return true;
        if (!EqualityComparer<UInt16?>.Default.Equals(item.SomeMember3, default(UInt16?))) return true;
        if (!EqualityComparer<Nullable<ushort>>.Default.Equals(item.SomeMember4, default(Nullable<ushort>))) return true;
        if (!EqualityComparer<Nullable<UInt16>>.Default.Equals(item.SomeMember5, default(Nullable<UInt16>))) return true;
        if (!EqualityComparer<ushort>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<UInt16>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<ushort?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<UInt16?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<ushort>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<UInt16>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "SomeMember0":
                    obj.SomeMember0 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember1":
                    obj.SomeMember1 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember2":
                    obj.SomeMember2 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember3":
                    obj.SomeMember3 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember4":
                    obj.SomeMember4 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember5":
                    obj.SomeMember5 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember6":
                    obj.SomeMember6 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember7":
                    obj.SomeMember7 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember8":
                    obj.SomeMember8 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember9":
                    obj.SomeMember9 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember10":
                    obj.SomeMember10 = kernel.ReadUInt16(reader);
                    break;
                case "SomeMember11":
                    obj.SomeMember11 = kernel.ReadUInt16(reader);
                    break;
                default:
                    break;
            }
        }

    }

}

