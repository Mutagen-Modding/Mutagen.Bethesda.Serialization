//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
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
        var tasks = new List<Task>();
        tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordWithNestedGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList,
            fieldName: "SomeList",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false,
            eachRecordInFolder: true));
        tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordWithNestedGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList2,
            fieldName: "SomeList2",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false,
            eachRecordInFolder: true));
        tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordWithNestedGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList3,
            fieldName: "SomeList3",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false,
            eachRecordInFolder: true));
        if (item.SomeList4 is {} checkedSomeList4
            && checkedSomeList4.Length > 0)
        {
            kernel.StartListSection(writer, "SomeList4");
            foreach (var listItem in checkedSomeList4)
            {
                await SerializationHelper.WriteRecordAsFolder(
                    streamPackage: writer.StreamPackage,
                    obj: listItem,
                    fieldName: null,
                    metaData: metaData,
                    kernel: kernel,
                    itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
                    hasSerializationItems: static (i, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.HasSerializationItems(i, m));
            }
            kernel.EndListSection(writer);
        }
        await Task.WhenAll(tasks.ToArray());
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
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
            case "SomeList4":
                {
                    obj.SomeList4 = await SerializationHelper.ReadArray<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested>(
                        reader: reader,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: static async (r, k, m) =>
                        {
                        });
                }
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
        var tasks = new List<Task>();
        while (kernel.TryGetNextField(reader, out var name))
        {
            await DeserializeSingleFieldInto<TReadObject>(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

        tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeList",
            list: obj.SomeList,
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.Deserialize<TReadObject>)).StripNull("SomeList"),
            eachRecordInFolder: true));
        tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeList2",
            list: obj.SomeList2,
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.Deserialize<TReadObject>)).StripNull("SomeList2"),
            eachRecordInFolder: true));
        tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeList3",
            list: obj.SomeList3,
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecordWithNested_Serialization.Deserialize<TReadObject>)).StripNull("SomeList3"),
            eachRecordInFolder: true));
        await Task.WhenAll(tasks.ToArray());
    }

}

