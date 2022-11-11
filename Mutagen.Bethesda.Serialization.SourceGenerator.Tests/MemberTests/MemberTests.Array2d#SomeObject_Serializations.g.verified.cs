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
            case "SomeArray":
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dYSection(reader))
                        {
                            kernel.StartArray2dXSection(reader);
                            var item = kernel.ReadString(reader);
                            obj.SomeArray[x, y] = item;
                            kernel.EndArray2dXSection(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray2":
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dYSection(reader))
                        {
                            kernel.StartArray2dXSection(reader);
                            var item = kernel.ReadString(reader);
                            obj.SomeArray2[x, y] = item;
                            kernel.EndArray2dXSection(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray3":
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dYSection(reader))
                        {
                            kernel.StartArray2dXSection(reader);
                            var item = kernel.ReadString(reader);
                            obj.SomeArray3[x, y] = item;
                            kernel.EndArray2dXSection(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            default:
                break;
        }
    }

}

