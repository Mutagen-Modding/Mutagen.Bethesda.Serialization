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
        if (item.SomeFormKeys is {} checkedSomeFormKeys
            && checkedSomeFormKeys.Count > 0)
        {
            kernel.StartListSection(writer, "SomeFormKeys");
            foreach (var listItem in checkedSomeFormKeys)
            {
                kernel.WriteFormKey(writer, null, listItem.FormKeyNullable, default(FormKey));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeFormKeys2 is {} checkedSomeFormKeys2
            && checkedSomeFormKeys2.Count > 0)
        {
            kernel.StartListSection(writer, "SomeFormKeys2");
            foreach (var listItem in checkedSomeFormKeys2)
            {
                kernel.WriteFormKey(writer, null, listItem.FormKeyNullable, default(FormKey));
            }
            kernel.EndListSection(writer);
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (item.SomeFormKeys.Count > 0) return true;
        if (item.SomeFormKeys2.Count > 0) return true;
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
            case "SomeFormKeys":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadFormKey(reader), "SomeFormKeys").AsLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>();
                    obj.SomeFormKeys.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeFormKeys2":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadFormKey(reader), "SomeFormKeys2").AsLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>();
                    obj.SomeFormKeys2.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            default:
                break;
        }
    }

}

