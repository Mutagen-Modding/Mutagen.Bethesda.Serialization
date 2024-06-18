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
        kernel.WriteBytes(writer, "SomeBytes", item.SomeBytes, default(byte[]));
        kernel.WriteBytes(writer, "SomeBytes2", item.SomeBytes2, default(byte[]));
        kernel.WriteBytes(writer, "SomeBytes3", item.SomeBytes3, default(byte[]));
        kernel.WriteBytes(writer, "SomeBytes4", item.SomeBytes4, default(byte[]?));
        kernel.WriteBytes(writer, "SomeBytes5", item.SomeBytes5, default(byte[]?));
        kernel.WriteBytes(writer, "SomeBytes6", item.SomeBytes6, default(byte[]?));
        kernel.WriteBytes(writer, "SomeBytes7", item.SomeBytes7, default(byte[]?));
        kernel.WriteBytes(writer, "SomeBytes8", item.SomeBytes8, default(byte[]));
        kernel.WriteBytes(writer, "SomeBytes9", item.SomeBytes9, default(byte[]));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes, default(byte[]))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes2, default(byte[]))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes3, default(byte[]))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes4, default(byte[]?))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes5, default(byte[]?))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes6, default(byte[]?))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes7, default(byte[]?))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes8, default(byte[]))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes9, default(byte[]))) return true;
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
            case "SomeBytes":
                obj.SomeBytes = SerializationHelper.StripNull(kernel.ReadBytes(reader), name: "SomeBytes");
                break;
            case "SomeBytes2":
                obj.SomeBytes2 = SerializationHelper.StripNull(kernel.ReadBytes(reader), name: "SomeBytes2");
                break;
            case "SomeBytes3":
                obj.SomeBytes3 = SerializationHelper.StripNull(kernel.ReadBytes(reader), name: "SomeBytes3");
                break;
            case "SomeBytes4":
                obj.SomeBytes4 = kernel.ReadBytes(reader);
                break;
            case "SomeBytes5":
                obj.SomeBytes5 = kernel.ReadBytes(reader);
                break;
            case "SomeBytes6":
                obj.SomeBytes6 = kernel.ReadBytes(reader);
                break;
            case "SomeBytes7":
                obj.SomeBytes7 = kernel.ReadBytes(reader);
                break;
            case "SomeBytes8":
                obj.SomeBytes8 = SerializationHelper.StripNull(kernel.ReadBytes(reader), name: "SomeBytes8");
                break;
            case "SomeBytes9":
                obj.SomeBytes9 = SerializationHelper.StripNull(kernel.ReadBytes(reader), name: "SomeBytes9");
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

