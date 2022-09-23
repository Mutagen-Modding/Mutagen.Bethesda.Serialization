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
        kernel.StartArray2dSection(writer, "SomeArray");
        for (int y = 0; y < item.SomeArray.Height; y++)
        {
            kernel.StartArray2dYSection(writer);
            for (int x = 0; x < item.SomeArray.Width; x++)
            {
                kernel.StartArray2dXSection(writer);
                kernel.WriteString(writer, null, item.SomeArray[x, y], default(string));
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
                kernel.WriteString(writer, null, item.SomeArray2[x, y], default(string));
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
                kernel.WriteString(writer, null, item.SomeArray3[x, y], default(string));
                kernel.EndArray2dXSection(writer);
            }
            kernel.EndArray2dYSection(writer);
        }
        kernel.EndArray2dSection(writer);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        return true;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case: "SomeArray":
                    {
                        kernel.StartArray2dSection(writer);
                        while (kernel.TryHasNextArray2dItem(writer, out int x, out int y))
                        {
                            var item = kernel.ReadString(writer);
                            item.SomeArray.Set(x, y, item);
                        }
                        kernel.EndArray2dSection(writer);
                    }
                case: "SomeArray2":
                    {
                        kernel.StartArray2dSection(writer);
                        while (kernel.TryHasNextArray2dItem(writer, out int x, out int y))
                        {
                            var item = kernel.ReadString(writer);
                            item.SomeArray2.Set(x, y, item);
                        }
                        kernel.EndArray2dSection(writer);
                    }
                case: "SomeArray3":
                    {
                        kernel.StartArray2dSection(writer);
                        while (kernel.TryHasNextArray2dItem(writer, out int x, out int y))
                        {
                            var item = kernel.ReadString(writer);
                            item.SomeArray3.Set(x, y, item);
                        }
                        kernel.EndArray2dSection(writer);
                    }
                default:
                    break;
            }
        }
    }

}

