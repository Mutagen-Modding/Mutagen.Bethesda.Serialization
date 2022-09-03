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
            var SomeArrayA2Writer = kernel.StartArray2dSection(writer, "SomeArray");
            foreach (var kv in item.SomeArray)
            {
                var itemWriter = kernel.StartArray2dItem(SomeArrayA2Writer, kv.Key.X, kv.Key.Y);
                kernel.WriteString(itemWriter, null, kv.Value);
                kernel.StopArray2dItem();
            }
            kernel.StopArray2dSectionSection();
        }
        {
            var SomeArray2A2Writer = kernel.StartArray2dSection(writer, "SomeArray2");
            foreach (var kv in item.SomeArray2)
            {
                var itemWriter = kernel.StartArray2dItem(SomeArray2A2Writer, kv.Key.X, kv.Key.Y);
                kernel.WriteString(itemWriter, null, kv.Value);
                kernel.StopArray2dItem();
            }
            kernel.StopArray2dSectionSection();
        }
        {
            var SomeArray3A2Writer = kernel.StartArray2dSection(writer, "SomeArray3");
            foreach (var kv in item.SomeArray3)
            {
                var itemWriter = kernel.StartArray2dItem(SomeArray3A2Writer, kv.Key.X, kv.Key.Y);
                kernel.WriteString(itemWriter, null, kv.Value);
                kernel.StopArray2dItem();
            }
            kernel.StopArray2dSectionSection();
        }
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

