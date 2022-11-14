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
        if (item.SomeList is {} checkedSomeList
            && checkedSomeList.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList");
            foreach (var listItem in checkedSomeList)
            {
                kernel.WriteString(writer, null, listItem, default(string));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList2 is {} checkedSomeList2
            && checkedSomeList2.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList2");
            foreach (var listItem in checkedSomeList2)
            {
                kernel.WriteString(writer, null, listItem, default(string));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList3 is {} checkedSomeList3
            && checkedSomeList3.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList3");
            foreach (var listItem in checkedSomeList3)
            {
                kernel.WriteString(writer, null, listItem, default(string));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList4 is {} checkedSomeList4)
        {
            kernel.StartListSection(writer, "SomeList4");
            foreach (var listItem in checkedSomeList4)
            {
                kernel.WriteString(writer, null, listItem, default(string));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList5 is {} checkedSomeList5)
        {
            kernel.StartListSection(writer, "SomeList5");
            foreach (var listItem in checkedSomeList5)
            {
                kernel.WriteString(writer, null, listItem, default(string));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList6 is {} checkedSomeList6)
        {
            kernel.StartListSection(writer, "SomeList6");
            foreach (var listItem in checkedSomeList6)
            {
                kernel.WriteString(writer, null, listItem, default(string));
            }
            kernel.EndListSection(writer);
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (item.SomeList.Count > 0) return true;
        if (item.SomeList2.Count > 0) return true;
        if (item.SomeList3.Count > 0) return true;
        if (item.SomeList4?.Length > 0) return true;
        if (item.SomeList5?.Length > 0) return true;
        if (item.SomeList6?.Count > 0) return true;
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
            case "SomeList":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeList");
                    obj.SomeList.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeList2":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeList2");
                    obj.SomeList2.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeList3":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeList3");
                    obj.SomeList3.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeList4":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeList4");
                    obj.SomeList4.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeList5":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeList5");
                    obj.SomeList5.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeList6":
                obj.SomeList6 ??= new();
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeList6");
                    obj.SomeList6.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            default:
                break;
        }
    }

}

