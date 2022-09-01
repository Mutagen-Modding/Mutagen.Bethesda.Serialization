//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        {
            var SomeDictDictWriter = kernel.StartDictionarySection(writer, "SomeDict");
            foreach (var kv in item.SomeDict)
            {
                var itemWriter = kernel.StartDictionaryItem(SomeDictDictWriter);
                var keyWriter = kernel.StartDictionaryKey(itemWriter);
                kernel.WriteInt32(keyWriter, null, kv.Key);
                kernel.StopDictionaryKey();
                var valueWriter = kernel.StartDictionaryValue(itemWriter);
                kernel.WriteString(valueWriter, null, kv.Value);
                kernel.StopDictionaryValue();
                kernel.StopDictionaryItem();
            }
            kernel.StopDictionarySection();
        }
        {
            var SomeDict1DictWriter = kernel.StartDictionarySection(writer, "SomeDict1");
            foreach (var kv in item.SomeDict1)
            {
                var itemWriter = kernel.StartDictionaryItem(SomeDict1DictWriter);
                var keyWriter = kernel.StartDictionaryKey(itemWriter);
                kernel.WriteInt32(keyWriter, null, kv.Key);
                kernel.StopDictionaryKey();
                var valueWriter = kernel.StartDictionaryValue(itemWriter);
                kernel.WriteString(valueWriter, null, kv.Value);
                kernel.StopDictionaryValue();
                kernel.StopDictionaryItem();
            }
            kernel.StopDictionarySection();
        }
        {
            var SomeDict2DictWriter = kernel.StartDictionarySection(writer, "SomeDict2");
            foreach (var kv in item.SomeDict2)
            {
                var itemWriter = kernel.StartDictionaryItem(SomeDict2DictWriter);
                var keyWriter = kernel.StartDictionaryKey(itemWriter);
                kernel.WriteInt32(keyWriter, null, kv.Key);
                kernel.StopDictionaryKey();
                var valueWriter = kernel.StartDictionaryValue(itemWriter);
                kernel.WriteString(valueWriter, null, kv.Value);
                kernel.StopDictionaryValue();
                kernel.StopDictionaryItem();
            }
            kernel.StopDictionarySection();
        }
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

