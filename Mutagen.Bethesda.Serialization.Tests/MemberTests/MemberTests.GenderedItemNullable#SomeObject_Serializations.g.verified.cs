//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.Exceptions;
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
        await kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGendered",
            item: item.SomeGendered,
            serializationMetaData: metaData,
            writeCall: static async (w, o, k, m) =>
            {
                if (o.Male is {} checkedMale)
                {
                    k.StartListSection(w, "Male");
                    foreach (var listItem in checkedMale)
                    {
                        k.WriteInt32(w, null, listItem, default(int), checkDefaults: false);
                    }
                    k.EndListSection(w);
                }
                if (o.Female is {} checkedFemale)
                {
                    k.StartListSection(w, "Female");
                    foreach (var listItem in checkedFemale)
                    {
                        k.WriteInt32(w, null, listItem, default(int), checkDefaults: false);
                    }
                    k.EndListSection(w);
                }
            });
        await kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGendered2",
            item: item.SomeGendered2,
            serializationMetaData: metaData,
            writeCall: static async (w, o, k, m) =>
            {
                if (o.Male is {} checkedMale)
                {
                    k.StartListSection(w, "Male");
                    foreach (var listItem in checkedMale)
                    {
                        k.WriteInt32(w, null, listItem, default(int), checkDefaults: false);
                    }
                    k.EndListSection(w);
                }
                if (o.Female is {} checkedFemale)
                {
                    k.StartListSection(w, "Female");
                    foreach (var listItem in checkedFemale)
                    {
                        k.WriteInt32(w, null, listItem, default(int), checkDefaults: false);
                    }
                    k.EndListSection(w);
                }
            });
        await kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGendered3",
            item: item.SomeGendered3,
            serializationMetaData: metaData,
            writeCall: static async (w, o, k, m) =>
            {
                if (o.Male is {} checkedMale)
                {
                    k.StartListSection(w, "Male");
                    foreach (var listItem in checkedMale)
                    {
                        k.WriteInt32(w, null, listItem, default(int), checkDefaults: false);
                    }
                    k.EndListSection(w);
                }
                if (o.Female is {} checkedFemale)
                {
                    k.StartListSection(w, "Female");
                    foreach (var listItem in checkedFemale)
                    {
                        k.WriteInt32(w, null, listItem, default(int), checkDefaults: false);
                    }
                    k.EndListSection(w);
                }
            });
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        if (item.SomeGendered.Male?.Count > 0) return true;
        if (item.SomeGendered.Female?.Count > 0) return true;
        if (item.SomeGendered2.Male?.Count > 0) return true;
        if (item.SomeGendered2.Female?.Count > 0) return true;
        if (item.SomeGendered3.Male?.Count > 0) return true;
        if (item.SomeGendered3.Female?.Count > 0) return true;
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
            case "SomeGendered":
                obj.SomeGendered = SerializationHelper.StripNull(await kernel.ReadLoqui(     reader: reader,     serializationMetaData: metaData,     readCall: static (r, k, m) =>     {         return SerializationHelper.ReadGenderedItem<ISerializationReaderKernel<TReadObject>, TReadObject, Noggog.ExtendedList<int>?>(             reader: r,             kernel: k,             metaData: m,             ret: new GenderedItem<Noggog.ExtendedList<int>?>(default, default),             itemReader: static async (r2, k2, m2, n) =>             {                 var ret = new ExtendedList<int>();                 k2.StartListSection(r2);                 while (k2.TryHasNextItem(r2))                 {                     var item = SerializationHelper.StripNull(k2.ReadInt32(r2), name: "n");                     ret.Add(item);                 }                 k2.EndListSection(r2);                 return ret;             });     }), name: "SomeGendered");
                break;
            case "SomeGendered2":
                obj.SomeGendered2 = SerializationHelper.StripNull(await kernel.ReadLoqui(     reader: reader,     serializationMetaData: metaData,     readCall: static (r, k, m) =>     {         return SerializationHelper.ReadGenderedItem<ISerializationReaderKernel<TReadObject>, TReadObject, Noggog.ExtendedList<int>?>(             reader: r,             kernel: k,             metaData: m,             ret: new GenderedItem<Noggog.ExtendedList<int>?>(default, default),             itemReader: static async (r2, k2, m2, n) =>             {                 var ret = new ExtendedList<int>();                 k2.StartListSection(r2);                 while (k2.TryHasNextItem(r2))                 {                     var item = SerializationHelper.StripNull(k2.ReadInt32(r2), name: "n");                     ret.Add(item);                 }                 k2.EndListSection(r2);                 return ret;             });     }), name: "SomeGendered2");
                break;
            case "SomeGendered3":
                obj.SomeGendered3 = SerializationHelper.StripNull(await kernel.ReadLoqui(     reader: reader,     serializationMetaData: metaData,     readCall: static (r, k, m) =>     {         return SerializationHelper.ReadGenderedItem<ISerializationReaderKernel<TReadObject>, TReadObject, Noggog.ExtendedList<int>?>(             reader: r,             kernel: k,             metaData: m,             ret: new GenderedItem<Noggog.ExtendedList<int>?>(default, default),             itemReader: static async (r2, k2, m2, n) =>             {                 var ret = new ExtendedList<int>();                 k2.StartListSection(r2);                 while (k2.TryHasNextItem(r2))                 {                     var item = SerializationHelper.StripNull(k2.ReadInt32(r2), name: "n");                     ret.Add(item);                 }                 k2.EndListSection(r2);                 return ret;             });     }), name: "SomeGendered3");
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

