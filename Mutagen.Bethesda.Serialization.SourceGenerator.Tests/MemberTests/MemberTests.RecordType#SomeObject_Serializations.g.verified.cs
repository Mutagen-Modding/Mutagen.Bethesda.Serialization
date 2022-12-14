//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
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
        kernel.WriteRecordType(writer, "SomeMember0", item.SomeMember0, default(RecordType));
        kernel.WriteRecordType(writer, "SomeMember1", item.SomeMember1, default(Mutagen.Bethesda.Plugins.RecordType));
        kernel.WriteRecordType(writer, "SomeMember2", item.SomeMember2, default(RecordType?));
        kernel.WriteRecordType(writer, "SomeMember3", item.SomeMember3, default(Mutagen.Bethesda.Plugins.RecordType?));
        kernel.WriteRecordType(writer, "SomeMember4", item.SomeMember4, default(Nullable<RecordType>));
        kernel.WriteRecordType(writer, "SomeMember5", item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.RecordType>));
        kernel.WriteRecordType(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteRecordType(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteRecordType(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteRecordType(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteRecordType(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteRecordType(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<RecordType>.Default.Equals(item.SomeMember0, default(RecordType))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType>.Default.Equals(item.SomeMember1, default(Mutagen.Bethesda.Plugins.RecordType))) return true;
        if (!EqualityComparer<RecordType?>.Default.Equals(item.SomeMember2, default(RecordType?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType?>.Default.Equals(item.SomeMember3, default(Mutagen.Bethesda.Plugins.RecordType?))) return true;
        if (!EqualityComparer<Nullable<RecordType>>.Default.Equals(item.SomeMember4, default(Nullable<RecordType>))) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.RecordType>>.Default.Equals(item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.RecordType>))) return true;
        if (!EqualityComparer<RecordType>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<RecordType?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<RecordType>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.RecordType>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
            DeserializeSingleFieldInto(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

    }

    public static void DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name)
    {
        switch (name)
        {
            case "SomeMember0":
                obj.SomeMember0 = SerializationHelper.StripNull(kernel.ReadRecordType(reader), name: "SomeMember0");
                break;
            case "SomeMember1":
                obj.SomeMember1 = SerializationHelper.StripNull(kernel.ReadRecordType(reader), name: "SomeMember1");
                break;
            case "SomeMember2":
                obj.SomeMember2 = kernel.ReadRecordType(reader);
                break;
            case "SomeMember3":
                obj.SomeMember3 = kernel.ReadRecordType(reader);
                break;
            case "SomeMember4":
                obj.SomeMember4 = kernel.ReadRecordType(reader);
                break;
            case "SomeMember5":
                obj.SomeMember5 = kernel.ReadRecordType(reader);
                break;
            case "SomeMember6":
                obj.SomeMember6 = SerializationHelper.StripNull(kernel.ReadRecordType(reader), name: "SomeMember6");
                break;
            case "SomeMember7":
                obj.SomeMember7 = SerializationHelper.StripNull(kernel.ReadRecordType(reader), name: "SomeMember7");
                break;
            case "SomeMember8":
                obj.SomeMember8 = kernel.ReadRecordType(reader);
                break;
            case "SomeMember9":
                obj.SomeMember9 = kernel.ReadRecordType(reader);
                break;
            case "SomeMember10":
                obj.SomeMember10 = kernel.ReadRecordType(reader);
                break;
            case "SomeMember11":
                obj.SomeMember11 = kernel.ReadRecordType(reader);
                break;
            default:
                break;
        }
    }

}

