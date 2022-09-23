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

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        while (kernel.TryGetNextField(out var name))
        {
            switch (name)
            {
                case: "SomeDict":
                    kernel.StartDictionarySection(writer);
                    while (kernel.TryHasNextDictionaryItem(writer))
                    {
                        var key = kernel.ReadInt32(writer);
                        var val = kernel.ReadString(writer);
                        item.SomeDict[key] = val;
                    }
                    kernel.EndDictionarySection(writer);
                case: "SomeDict1":
                    kernel.StartDictionarySection(writer);
                    while (kernel.TryHasNextDictionaryItem(writer))
                    {
                        var key = kernel.ReadInt32(writer);
                        var val = kernel.ReadString(writer);
                        item.SomeDict1[key] = val;
                    }
                    kernel.EndDictionarySection(writer);
                case: "SomeDict2":
                    kernel.StartDictionarySection(writer);
                    while (kernel.TryHasNextDictionaryItem(writer))
                    {
                        var key = kernel.ReadInt32(writer);
                        var val = kernel.ReadString(writer);
                        item.SomeDict2[key] = val;
                    }
                    kernel.EndDictionarySection(writer);
                default:
                    break;
            }
        }
    }

}

