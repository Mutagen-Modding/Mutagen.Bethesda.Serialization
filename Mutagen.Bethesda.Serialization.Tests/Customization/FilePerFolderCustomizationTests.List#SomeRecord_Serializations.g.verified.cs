﻿//HintName: SomeRecord_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
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

internal static class SomeRecord_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeRecordGetter item,
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeRecordGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        var tasks = new List<Task>();
        kernel.WriteInt32(writer, "SomeMember1", item.SomeMember1, default(int));
        tasks.Add(SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList,
            fieldName: "SomeList",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false));
        kernel.WriteInt32(writer, "SomeMember2", item.SomeMember2, default(int));
        await Task.WhenAll(tasks.ToArray());
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeRecordGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        if (!EqualityComparer<int>.Default.Equals(item.SomeMember1, default(int))) return true;
        if (item.SomeList.Count > 0) return true;
        if (!EqualityComparer<int>.Default.Equals(item.SomeMember2, default(int))) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeRecord> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeRecord();
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeRecord obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        switch (name)
        {
            case "SomeMember1":
                obj.SomeMember1 = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeMember1");
                break;
            case "SomeMember2":
                obj.SomeMember2 = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeMember2");
                break;
            default:
                kernel.Skip(reader);
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeRecord obj,
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

        tasks.Add(SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeList",
            list: obj.SomeList,
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>)).StripNull("SomeList")));
        await Task.WhenAll(tasks.ToArray());
    }

}

