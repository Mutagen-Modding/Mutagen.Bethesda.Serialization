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
        var metaData = new SerializationMetaData(item.GameRelease);
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

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        if (item.SomeDict.Count > 0) return true;
        if (item.SomeDict1.Count > 0) return true;
        if (item.SomeDict2.Count > 0) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

