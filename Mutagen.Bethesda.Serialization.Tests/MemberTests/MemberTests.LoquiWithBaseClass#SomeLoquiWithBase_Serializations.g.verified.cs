//HintName: SomeLoquiWithBase_Serializations.g.cs
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

internal static class SomeLoquiWithBase_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter item,
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        await Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.Serialize<TKernel, TWriteObject>(writer, item, kernel, metaData);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.HasSerializationItems(item, metaData)) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoquiWithBase> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoquiWithBase();
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBase obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        switch (name)
        {
            default:
                await Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.DeserializeSingleFieldInto<TReadObject>(reader, kernel, obj, metaData, name);
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBase obj,
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

