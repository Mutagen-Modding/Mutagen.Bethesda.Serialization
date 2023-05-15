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

#pragma warning disable CA1998 // No awaits used
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
        kernel.WriteInt64(writer, "SomeMember0", item.SomeMember0, default(long));
        kernel.WriteInt64(writer, "SomeMember1", item.SomeMember1, default(Int64));
        kernel.WriteInt64(writer, "SomeMember2", item.SomeMember2, default(long?));
        kernel.WriteInt64(writer, "SomeMember3", item.SomeMember3, default(Int64?));
        kernel.WriteInt64(writer, "SomeMember4", item.SomeMember4, default(Nullable<long>));
        kernel.WriteInt64(writer, "SomeMember5", item.SomeMember5, default(Nullable<Int64>));
        kernel.WriteInt64(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteInt64(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteInt64(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteInt64(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteInt64(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteInt64(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<long>.Default.Equals(item.SomeMember0, default(long))) return true;
        if (!EqualityComparer<Int64>.Default.Equals(item.SomeMember1, default(Int64))) return true;
        if (!EqualityComparer<long?>.Default.Equals(item.SomeMember2, default(long?))) return true;
        if (!EqualityComparer<Int64?>.Default.Equals(item.SomeMember3, default(Int64?))) return true;
        if (!EqualityComparer<Nullable<long>>.Default.Equals(item.SomeMember4, default(Nullable<long>))) return true;
        if (!EqualityComparer<Nullable<Int64>>.Default.Equals(item.SomeMember5, default(Nullable<Int64>))) return true;
        if (!EqualityComparer<long>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Int64>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<long?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Int64?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<long>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Int64>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
            case "SomeMember0":
                obj.SomeMember0 = SerializationHelper.StripNull(kernel.ReadInt64(reader), name: "SomeMember0");
                break;
            case "SomeMember1":
                obj.SomeMember1 = SerializationHelper.StripNull(kernel.ReadInt64(reader), name: "SomeMember1");
                break;
            case "SomeMember2":
                obj.SomeMember2 = kernel.ReadInt64(reader);
                break;
            case "SomeMember3":
                obj.SomeMember3 = kernel.ReadInt64(reader);
                break;
            case "SomeMember4":
                obj.SomeMember4 = kernel.ReadInt64(reader);
                break;
            case "SomeMember5":
                obj.SomeMember5 = kernel.ReadInt64(reader);
                break;
            case "SomeMember6":
                obj.SomeMember6 = SerializationHelper.StripNull(kernel.ReadInt64(reader), name: "SomeMember6");
                break;
            case "SomeMember7":
                obj.SomeMember7 = SerializationHelper.StripNull(kernel.ReadInt64(reader), name: "SomeMember7");
                break;
            case "SomeMember8":
                obj.SomeMember8 = kernel.ReadInt64(reader);
                break;
            case "SomeMember9":
                obj.SomeMember9 = kernel.ReadInt64(reader);
                break;
            case "SomeMember10":
                obj.SomeMember10 = kernel.ReadInt64(reader);
                break;
            case "SomeMember11":
                obj.SomeMember11 = kernel.ReadInt64(reader);
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

