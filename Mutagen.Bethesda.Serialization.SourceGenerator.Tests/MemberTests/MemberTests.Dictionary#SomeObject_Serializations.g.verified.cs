//HintName: SomeObject_Serializations.g.cs
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
        kernel.StartDictionarySection(writer, "SomeDict");
        foreach (var kv in item.SomeDict)
        {
            kernel.StartDictionaryItem(writer);
            kernel.StartDictionaryKey(writer);
            kernel.WriteInt32(writer, null, kv.Key, default(int));
            kernel.EndDictionaryKey(writer);
            kernel.StartDictionaryValue(writer);
            kernel.WriteString(writer, null, kv.Value, default(string));
            kernel.EndDictionaryValue(writer);
            kernel.EndDictionaryItem(writer);
        }
        kernel.EndDictionarySection(writer);
        kernel.StartDictionarySection(writer, "SomeDict1");
        foreach (var kv in item.SomeDict1)
        {
            kernel.StartDictionaryItem(writer);
            kernel.StartDictionaryKey(writer);
            kernel.WriteInt32(writer, null, kv.Key, default(int));
            kernel.EndDictionaryKey(writer);
            kernel.StartDictionaryValue(writer);
            kernel.WriteString(writer, null, kv.Value, default(string));
            kernel.EndDictionaryValue(writer);
            kernel.EndDictionaryItem(writer);
        }
        kernel.EndDictionarySection(writer);
        kernel.StartDictionarySection(writer, "SomeDict2");
        foreach (var kv in item.SomeDict2)
        {
            kernel.StartDictionaryItem(writer);
            kernel.StartDictionaryKey(writer);
            kernel.WriteInt32(writer, null, kv.Key, default(int));
            kernel.EndDictionaryKey(writer);
            kernel.StartDictionaryValue(writer);
            kernel.WriteString(writer, null, kv.Value, default(string));
            kernel.EndDictionaryValue(writer);
            kernel.EndDictionaryItem(writer);
        }
        kernel.EndDictionarySection(writer);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (item.SomeDict.Count > 0) return true;
        if (item.SomeDict1.Count > 0) return true;
        if (item.SomeDict2.Count > 0) return true;
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
                case "SomeDict":
                    kernel.StartDictionarySection(reader);
                    while (kernel.TryHasNextDictionaryItem(reader))
                    {
                        var key = kernel.ReadInt32(reader);
                        var val = kernel.ReadString(reader);
                        obj.SomeDict[key] = val;
                    }
                    kernel.EndDictionarySection(reader);
                    break;
                case "SomeDict1":
                    kernel.StartDictionarySection(reader);
                    while (kernel.TryHasNextDictionaryItem(reader))
                    {
                        var key = kernel.ReadInt32(reader);
                        var val = kernel.ReadString(reader);
                        obj.SomeDict1[key] = val;
                    }
                    kernel.EndDictionarySection(reader);
                    break;
                case "SomeDict2":
                    kernel.StartDictionarySection(reader);
                    while (kernel.TryHasNextDictionaryItem(reader))
                    {
                        var key = kernel.ReadInt32(reader);
                        var val = kernel.ReadString(reader);
                        obj.SomeDict2[key] = val;
                    }
                    kernel.EndDictionarySection(reader);
                    break;
                default:
                    break;
            }
        }

    }

}

