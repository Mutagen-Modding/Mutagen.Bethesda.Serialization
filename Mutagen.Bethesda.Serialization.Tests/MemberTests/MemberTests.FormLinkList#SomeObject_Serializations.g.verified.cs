//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;
using System.IO.Abstractions;
using System.Threading.Tasks;

#nullable enable

#pragma warning disable CS1998 // No awaits used
#pragma warning disable CS0618 // Obsolete

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        await SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static async Task SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item.SomeFormKeys is {} checkedSomeFormKeys
            && checkedSomeFormKeys.Count > 0)
        {
            kernel.StartListSection(writer, "SomeFormKeys");
            foreach (var listItem in checkedSomeFormKeys)
            {
                kernel.WriteFormKey(writer, null, listItem.FormKeyNullable, default(FormKey), checkDefaults: false);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeFormKeys2 is {} checkedSomeFormKeys2
            && checkedSomeFormKeys2.Count > 0)
        {
            kernel.StartListSection(writer, "SomeFormKeys2");
            foreach (var listItem in checkedSomeFormKeys2)
            {
                kernel.WriteFormKey(writer, null, listItem.FormKeyNullable, default(FormKey), checkDefaults: false);
            }
            kernel.EndListSection(writer);
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        if (item.SomeFormKeys.Count > 0) return true;
        if (item.SomeFormKeys2.Count > 0) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        await DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static async Task DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        switch (name)
        {
            case "SomeFormKeys":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = kernel.ReadFormKey(reader).StripNull("SomeFormKeys").ToLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>();
                    obj.SomeFormKeys.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeFormKeys2":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = kernel.ReadFormKey(reader).StripNull("SomeFormKeys2").ToLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>();
                    obj.SomeFormKeys2.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            default:
                kernel.Skip(reader);
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        while (kernel.TryGetNextField(reader, out var name))
        {
            await DeserializeSingleFieldInto<TReadObject>(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

    }

}

