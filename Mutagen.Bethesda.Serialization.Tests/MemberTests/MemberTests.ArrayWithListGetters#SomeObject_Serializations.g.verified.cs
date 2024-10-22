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
        if (item.SomeArray is {} checkedSomeArray
            && checkedSomeArray.Count > 0)
        {
            kernel.StartListSection(writer, "SomeArray");
            foreach (var listItem in checkedSomeArray)
            {
                throw new NotImplementedException("Unknown type: CloudLayer for field ");
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeArray2 is {} checkedSomeArray2)
        {
            kernel.StartListSection(writer, "SomeArray2");
            foreach (var listItem in checkedSomeArray2)
            {
                throw new NotImplementedException("Unknown type: CloudLayer for field ");
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeArray3 is {} checkedSomeArray3
            && checkedSomeArray3.Count > 0)
        {
            kernel.StartListSection(writer, "SomeArray3");
            foreach (var listItem in checkedSomeArray3)
            {
                throw new NotImplementedException("Unknown type: CloudLayer for field ");
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
        if (item.SomeArray.Count > 0) return true;
        if (item.SomeArray2?.Count > 0) return true;
        if (item.SomeArray3.Count > 0) return true;
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
            case "SomeArray":
                {
                    obj.SomeArray = await SerializationHelper.ReadArray<ISerializationReaderKernel<TReadObject>, TReadObject, CloudLayer>(
                        reader: reader,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: static async (r, k, m) =>
                        {
                            throw new NotImplementedException("Unknown type: CloudLayer for field SomeArray");
                        });
                }
                break;
            case "SomeArray2":
                {
                    obj.SomeArray2 = await SerializationHelper.ReadArray<ISerializationReaderKernel<TReadObject>, TReadObject, CloudLayer>(
                        reader: reader,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: static async (r, k, m) =>
                        {
                            throw new NotImplementedException("Unknown type: CloudLayer for field SomeArray2");
                        });
                }
                break;
            case "SomeArray3":
                {
                    await SerializationHelper.ReadIntoArray<ISerializationReaderKernel<TReadObject>, TReadObject, CloudLayer>(
                        reader: reader,
                        arr: obj.SomeArray3,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: static async (r, k, m) =>
                        {
                            throw new NotImplementedException("Unknown type: CloudLayer for field SomeArray3");
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

