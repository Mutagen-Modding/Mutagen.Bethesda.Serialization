//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Noggog;

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
        kernel.WriteBytes(writer, "SomeBytes", item.SomeBytes, default(byte[]));
        if (item.SomeBytes2 is {} checkedSomeBytes2
            && checkedSomeBytes2.Length > 0)
        {
            kernel.StartListSection(writer, "SomeBytes2");
            foreach (var listItem in checkedSomeBytes2)
            {
                kernel.WriteUInt8(writer, null, listItem, default(byte), checkDefaults: false);
            }
            kernel.EndListSection(writer);
        }
        kernel.WriteBytes(writer, "SomeBytes3", item.SomeBytes3, default(byte[]?));
        if (item.SomeBytes4 is {} checkedSomeBytes4)
        {
            kernel.StartListSection(writer, "SomeBytes4");
            foreach (var listItem in checkedSomeBytes4)
            {
                kernel.WriteUInt8(writer, null, listItem, default(byte), checkDefaults: false);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeBytes5 is {} checkedSomeBytes5)
        {
            kernel.StartListSection(writer, "SomeBytes5");
            foreach (var listItem in checkedSomeBytes5)
            {
                kernel.WriteUInt8(writer, null, listItem, default(byte), checkDefaults: false);
            }
            kernel.EndListSection(writer);
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes, default(byte[]))) return true;
        if (item.SomeBytes2.Length > 0) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes3, default(byte[]?))) return true;
        if (item.SomeBytes4?.Length > 0) return true;
        if (item.SomeBytes5?.Length > 0) return true;
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
            case "SomeBytes":
                obj.SomeBytes = kernel.ReadBytes(reader);
                break;
            case "SomeBytes2":
                {
                    SerializationHelper.ReadIntoSlice(
                        reader: reader,
                        arr: obj.SomeBytes2,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: (r, k, m) =>
                        {
                            return SerializationHelper.StripNull(kernel.ReadUInt8(r), name: "SomeBytes2");
                        });
                }
                break;
            case "SomeBytes3":
                obj.SomeBytes3 = kernel.ReadBytes(reader);
                break;
            case "SomeBytes4":
                {
                    obj.SomeBytes4 = SerializationHelper.ReadSlice(
                        reader: reader,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: (r, k, m) =>
                        {
                            return SerializationHelper.StripNull(kernel.ReadUInt8(r), name: "SomeBytes4");
                        });
                }
                break;
            case "SomeBytes5":
                {
                    obj.SomeBytes5 = SerializationHelper.ReadSlice(
                        reader: reader,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: (r, k, m) =>
                        {
                            return SerializationHelper.StripNull(kernel.ReadUInt8(r), name: "SomeBytes5");
                        });
                }
                break;
            default:
                break;
        }
    }

}

