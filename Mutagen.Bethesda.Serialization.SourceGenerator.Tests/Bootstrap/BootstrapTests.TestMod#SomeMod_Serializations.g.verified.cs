//HintName: SomeMod_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.WorkEngine;
using System.IO.Abstractions;
using System.Threading.Tasks;

#nullable enable

#pragma warning disable CA1998 // No awaits used
#pragma warning disable CS0618 // Obsolete

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeMod_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        await SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static async Task SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        kernel.WriteInt32(writer, "SomeInt", item.SomeInt, default(int));
        if (item.SomeList is {} checkedSomeList
            && checkedSomeList.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList");
            foreach (var listItem in checkedSomeList)
            {
                kernel.WriteInt32(writer, null, listItem, default(int), checkDefaults: false);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeObject is {} SomeObjectChecked
            && Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.HasSerializationItems(SomeObjectChecked, metaData))
        {
            await kernel.WriteLoqui(writer, "SomeObject", SomeObjectChecked, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<int>.Default.Equals(item.SomeInt, default(int))) return true;
        if (item.SomeList.Count > 0) return true;
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.HasSerializationItems(item.SomeObject, metaData)) return true;
        return false;
    }

    public static async Task DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            case "SomeInt":
                obj.SomeInt = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeInt");
                break;
            case "SomeList":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeList");
                    obj.SomeList.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            case "SomeObject":
                obj.SomeObject = SerializationHelper.StripNull(await kernel.ReadLoqui(reader, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TReadObject>(r, k, m)), name: "SomeObject");
                break;
            default:
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
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

