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
        kernel.WriteFormKey(writer, "SomeFormKey", item.SomeFormKey.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey2", item.SomeFormKey2.FormKeyNullable, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeFormKey3", item.SomeFormKey3.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey4", item.SomeFormKey4.FormKeyNullable, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeFormKey5", item.SomeFormKey5.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey6", item.SomeFormKey6.FormKeyNullable, default(FormKey?));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<IFormLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeFormKey, default(IFormLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IFormLinkNullableGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeFormKey2, default(IFormLinkNullableGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IFormLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeFormKey3, default(IFormLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IFormLinkNullableGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeFormKey4, default(IFormLinkNullableGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IFormLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeFormKey5, default(IFormLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IFormLinkNullableGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeFormKey6, default(IFormLinkNullableGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
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
            case "SomeFormKey":
                obj.SomeFormKey.SetTo(kernel.ReadFormKey(reader));
                break;
            case "SomeFormKey2":
                obj.SomeFormKey2.SetTo(kernel.ReadFormKey(reader).StripNull("SomeFormKey2"));
                break;
            case "SomeFormKey3":
                obj.SomeFormKey3.SetTo(kernel.ReadFormKey(reader));
                break;
            case "SomeFormKey4":
                obj.SomeFormKey4.SetTo(kernel.ReadFormKey(reader).StripNull("SomeFormKey4"));
                break;
            case "SomeFormKey5":
                obj.SomeFormKey5.SetTo(kernel.ReadFormKey(reader));
                break;
            case "SomeFormKey6":
                obj.SomeFormKey6.SetTo(kernel.ReadFormKey(reader).StripNull("SomeFormKey6"));
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
            await DeserializeSingleFieldInto(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

    }

}

