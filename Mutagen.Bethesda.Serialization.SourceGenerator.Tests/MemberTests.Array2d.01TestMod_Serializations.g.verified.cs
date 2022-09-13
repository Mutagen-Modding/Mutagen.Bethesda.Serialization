//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.StartArray2dSection(writer, "SomeArray");
        for (int y = 0; y < item.SomeArray.Height; y++)
        {
            kernel.StartArray2dYSection(writer);
            for (int x = 0; x < item.SomeArray.Width; x++)
            {
                kernel.StartArray2dXSection(writer);
                kernel.WriteString(writer, null, item.SomeArray[x, y]);
                kernel.EndArray2dXSection(writer);
            }
            kernel.EndArray2dYSection(writer);
        }
        kernel.EndArray2dSection(writer);
        kernel.StartArray2dSection(writer, "SomeArray2");
        for (int y = 0; y < item.SomeArray2.Height; y++)
        {
            kernel.StartArray2dYSection(writer);
            for (int x = 0; x < item.SomeArray2.Width; x++)
            {
                kernel.StartArray2dXSection(writer);
                kernel.WriteString(writer, null, item.SomeArray2[x, y]);
                kernel.EndArray2dXSection(writer);
            }
            kernel.EndArray2dYSection(writer);
        }
        kernel.EndArray2dSection(writer);
        kernel.StartArray2dSection(writer, "SomeArray3");
        for (int y = 0; y < item.SomeArray3.Height; y++)
        {
            kernel.StartArray2dYSection(writer);
            for (int x = 0; x < item.SomeArray3.Width; x++)
            {
                kernel.StartArray2dXSection(writer);
                kernel.WriteString(writer, null, item.SomeArray3[x, y]);
                kernel.EndArray2dXSection(writer);
            }
            kernel.EndArray2dYSection(writer);
        }
        kernel.EndArray2dSection(writer);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

