//HintName: SomeMajorRecord_Serializations.g.cs
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

internal static class SomeMajorRecord_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeMajorRecordGetter item,
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeMajorRecordGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        kernel.WriteInt32(writer, "SomeMember1", item.SomeMember1, default(int));
        kernel.WriteInt32(writer, "SomeMember2", item.SomeMember2, default(int));
        kernel.WriteInt32(writer, "SomeMember3", item.SomeMember3, default(int));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeMajorRecordGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<int>.Default.Equals(item.SomeMember1, default(int))) return true;
        if (!EqualityComparer<int>.Default.Equals(item.SomeMember2, default(int))) return true;
        if (!EqualityComparer<int>.Default.Equals(item.SomeMember3, default(int))) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeMajorRecord> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeMajorRecord();
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeMajorRecord obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            case "SomeMember1":
                obj.SomeMember1 = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeMember1");
                break;
            case "SomeMember2":
                obj.SomeMember2 = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeMember2");
                break;
            case "SomeMember3":
                obj.SomeMember3 = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeMember3");
                break;
            default:
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeMajorRecord obj,
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

