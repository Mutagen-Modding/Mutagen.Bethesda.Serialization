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
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum>(writer, "SomeEnum", item.SomeEnum, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum>(writer, "SomeEnum2", item.SomeEnum2, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum?));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2>(writer, "SomeEnum3", item.SomeEnum3, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2>(writer, "SomeEnum4", item.SomeEnum4, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2?));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3>(writer, "SomeEnum5", item.SomeEnum5, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3>(writer, "SomeEnum6", item.SomeEnum6, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3?));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum>.Default.Equals(item.SomeEnum, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum>>.Default.Equals(item.SomeEnum2, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2>.Default.Equals(item.SomeEnum3, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2>>.Default.Equals(item.SomeEnum4, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3>.Default.Equals(item.SomeEnum5, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3>>.Default.Equals(item.SomeEnum6, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3>))) return true;
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
            case "SomeEnum":
                obj.SomeEnum = SerializationHelper.StripNull(kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum>(reader), name: "SomeEnum");
                break;
            case "SomeEnum2":
                obj.SomeEnum2 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum>(reader);
                break;
            case "SomeEnum3":
                obj.SomeEnum3 = SerializationHelper.StripNull(kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2>(reader), name: "SomeEnum3");
                break;
            case "SomeEnum4":
                obj.SomeEnum4 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum2>(reader);
                break;
            case "SomeEnum5":
                obj.SomeEnum5 = SerializationHelper.StripNull(kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3>(reader), name: "SomeEnum5");
                break;
            case "SomeEnum6":
                obj.SomeEnum6 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject.MyEnum3>(reader);
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

