﻿//HintName: ListGroup_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class ListGroup_Serialization
{
    public static void Serialize<TKernel, TWriteObject, T>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IListGroupGetter<T> item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where T : class, IMajorRecordInternal
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        SerializeFields<TKernel, TWriteObject, T>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static void SerializeFields<TKernel, TWriteObject, T>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IListGroupGetter<T> item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where T : class, IMajorRecordInternal
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        kernel.WriteInt32(writer, "SomeInt", item.SomeInt, default(int));
        kernel.WriteInt32(writer, "SomeInt2", item.SomeInt2, default(int));
    }

    public static bool HasSerializationItems<T>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IListGroupGetter<T>? item,
        SerializationMetaData metaData)
        where T : class, IMajorRecordInternal
    {
        if (item == null) return false;
        if (!EqualityComparer<int>.Default.Equals(item.SomeInt, default(int))) return true;
        if (item.Items.Count > 0) return true;
        if (!EqualityComparer<int>.Default.Equals(item.SomeInt2, default(int))) return true;
        return false;
    }

    public static void DeserializeSingleFieldInto<TReadObject, T>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IListGroup<T> obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
        where T : class, IMajorRecordInternal
    {
        switch (name)
        {
            case "SomeInt":
                obj.SomeInt = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeInt");
                break;
            case "SomeInt2":
                obj.SomeInt2 = SerializationHelper.StripNull(kernel.ReadInt32(reader), name: "SomeInt2");
                break;
            default:
                break;
        }
    }
    
    public static void DeserializeInto<TReadObject, T>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IListGroup<T> obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
        where T : class, IMajorRecordInternal
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            DeserializeSingleFieldInto(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

    }

}
