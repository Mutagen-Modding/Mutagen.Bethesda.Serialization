//HintName: AbstractBaseLoqui_Serializations.g.cs
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

internal static class AbstractBaseLoqui_Serialization
{
    public static async Task SerializeWithCheck<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        kernel.WriteType(writer, LoquiRegistration.StaticRegister.GetRegister(item.GetType()).ClassType);
        switch (item)
        {
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiAGetter SubclassLoquiAGetter:
                await Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiA_Serialization.Serialize(writer, SubclassLoquiAGetter, kernel, metaData);
                break;
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiBGetter SubclassLoquiBGetter:
                await Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiB_Serialization.Serialize(writer, SubclassLoquiBGetter, kernel, metaData);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
    }

    public static bool HasSerializationItemsWithCheck(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter? item,
        SerializationMetaData metaData)
    {
        return true;
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui> DeserializeWithCheck<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        var type = kernel.GetNextType(reader, "Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        switch (type.Name)
        {
            case "SubclassLoquiA":
                return await Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiA_Serialization.Deserialize<TReadObject>(reader, kernel, metaData);
            case "SubclassLoquiB":
                return await Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiB_Serialization.Deserialize<TReadObject>(reader, kernel, metaData);
            default:
                throw new NotImplementedException();
        }
    }

    public static async Task DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoqui obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        switch (name)
        {
            default:
                kernel.Skip(reader);
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoqui obj,
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

