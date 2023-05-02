//HintName: SomeObject_Serializations.g.cs
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
        var tasks = new List<Task>();
        tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList,
            fieldName: "SomeList",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false));
        tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList2,
            fieldName: "SomeList2",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false));
        tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList3,
            fieldName: "SomeList3",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false));
        if (item.SomeList4 is {} checkedSomeList4
            && checkedSomeList4.Length > 0)
        {
            kernel.StartListSection(writer, "SomeList4");
            foreach (var listItem in checkedSomeList4)
            {
                await kernel.WriteLoqui(writer, null, listItem, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
            }
            kernel.EndListSection(writer);
        }
        await Task.WhenAll(tasks.ToArray());
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (item.SomeList.Count > 0) return true;
        if (item.SomeList2.Count > 0) return true;
        if (item.SomeList3.Count > 0) return true;
        if (item.SomeList4.Length > 0) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
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
        switch (name)
        {
            case "SomeList4":
                {
                    obj.SomeList4 = await SerializationHelper.ReadArray<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
                        reader: reader,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: static async (r, k, m) =>
                        {
                            return SerializationHelper.StripNull(await k.ReadLoqui(r, m, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>(r, k, m)), name: "SomeList4");
                        });
                }
                break;
            default:
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
        var tasks = new List<Task>();
        while (kernel.TryGetNextField(reader, out var name))
        {
            await DeserializeSingleFieldInto(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

        tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeList",
            list: obj.SomeList,
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>)).StripNull("SomeList")));
        tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeList2",
            list: obj.SomeList2,
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>)).StripNull("SomeList2")));
        tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeList3",
            list: obj.SomeList3,
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>)).StripNull("SomeList3")));
        await Task.WhenAll(tasks.ToArray());
    }

}

