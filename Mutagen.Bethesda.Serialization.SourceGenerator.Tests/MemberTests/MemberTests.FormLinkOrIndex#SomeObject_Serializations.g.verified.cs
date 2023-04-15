//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static void SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        SerializationHelper.WriteFormLinkOrIndex(writer, "SomeFormKey", item.SomeFormKey, metaData, kernel);
        SerializationHelper.WriteFormLinkOrIndex(writer, "SomeFormKey3", item.SomeFormKey3, metaData, kernel);
        SerializationHelper.WriteFormLinkOrIndex(writer, "SomeFormKey5", item.SomeFormKey5, metaData, kernel);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!FormLinkOrIndex<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>.EqualityComparer.Equals(item.SomeFormKey, default(FormLinkOrIndex<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!FormLinkOrIndex<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>.EqualityComparer.Equals(item.SomeFormKey3, default(FormLinkOrIndex<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!FormLinkOrIndex<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>.EqualityComparer.Equals(item.SomeFormKey5, default(FormLinkOrIndex<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static void DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            case "SomeFormKey":
                SerializationHelper.ReadFormLinkOrIndex(reader, kernel, obj.SomeFormKey, metaData);
                break;
            case "SomeFormKey3":
                SerializationHelper.ReadFormLinkOrIndex(reader, kernel, obj.SomeFormKey3, metaData);
                break;
            case "SomeFormKey5":
                SerializationHelper.ReadFormLinkOrIndex(reader, kernel, obj.SomeFormKey5, metaData);
                break;
            default:
                break;
        }
    }
    
    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
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

