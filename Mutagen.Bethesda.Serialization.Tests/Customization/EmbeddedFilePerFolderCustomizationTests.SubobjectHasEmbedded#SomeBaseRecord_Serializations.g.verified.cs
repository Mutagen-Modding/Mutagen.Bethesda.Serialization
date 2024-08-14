//HintName: SomeBaseRecord_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.Exceptions;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
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

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeBaseRecord_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeBaseRecordGetter item,
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeBaseRecordGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        await SerializationHelper.WriteRecordAsFolder(
            streamPackage: writer.StreamPackage,
            obj: item.SomeRecord,
            fieldName: "SomeRecord",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            hasSerializationItems: static (i, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeRecord_Serialization.HasSerializationItems(i, m));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeBaseRecordGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeRecord_Serialization.HasSerializationItems(item.SomeRecord, metaData)) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeBaseRecord> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeBaseRecord();
        await DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeBaseRecord obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        obj.SomeRecord = await SerializationHelper.ReadRecordAsFolder<ISerializationReaderKernel<TReadObject>, TReadObject, SomeRecord>(
            streamPackage: reader.StreamPackage,
            fieldName: "SomeRecord",
            metaData: metaData,
            kernel: kernel,
            itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeRecord_Serialization.Deserialize<TReadObject>)).StripNull("SomeRecord"));
    }

}

