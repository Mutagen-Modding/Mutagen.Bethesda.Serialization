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
        if (item.SomeArray is {} checkedSomeArray)
        {
            kernel.StartArray2dSection(writer, "SomeArray");
            for (int y = 0; y < checkedSomeArray.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray2 is {} checkedSomeArray2)
        {
            kernel.StartArray2dSection(writer, "SomeArray2");
            for (int y = 0; y < checkedSomeArray2.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray2.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray2[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray3 is {} checkedSomeArray3)
        {
            kernel.StartArray2dSection(writer, "SomeArray3");
            for (int y = 0; y < checkedSomeArray3.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray3.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray3[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray4 is {} checkedSomeArray4)
        {
            kernel.StartArray2dSection(writer, "SomeArray4");
            for (int y = 0; y < checkedSomeArray4.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray4.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray4[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray5 is {} checkedSomeArray5)
        {
            kernel.StartArray2dSection(writer, "SomeArray5");
            for (int y = 0; y < checkedSomeArray5.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray5.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray5[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray6 is {} checkedSomeArray6)
        {
            kernel.StartArray2dSection(writer, "SomeArray6");
            for (int y = 0; y < checkedSomeArray6.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray6.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray6[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
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
                if (obj.SomeArray is {} checkedSomeArray)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray");
                            obj.SomeArray[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray2":
                if (obj.SomeArray2 is {} checkedSomeArray2)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray2");
                            obj.SomeArray2[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray3":
                if (obj.SomeArray3 is {} checkedSomeArray3)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray3");
                            obj.SomeArray3[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray4":
                obj.SomeArray4 = new Array2d<String>(SomeObject.SomeArray4FixedSize);
                if (obj.SomeArray4 is {} checkedSomeArray4)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray4");
                            obj.SomeArray4[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray5":
                obj.SomeArray5 = new Array2d<String>(SomeObject.SomeArray5FixedSize);
                if (obj.SomeArray5 is {} checkedSomeArray5)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray5");
                            obj.SomeArray5[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray6":
                obj.SomeArray6 = new Array2d<String>(SomeObject.SomeArray6FixedSize);
                if (obj.SomeArray6 is {} checkedSomeArray6)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray6");
                            obj.SomeArray6[x, y] = item;
                            kernel.EndArray2dXItem(reader);
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

