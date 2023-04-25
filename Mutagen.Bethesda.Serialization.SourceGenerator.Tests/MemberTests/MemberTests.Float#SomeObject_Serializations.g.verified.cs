//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.WorkEngine;
using System.Threading.Tasks;

#nullable enable

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
        kernel.WriteFloat(writer, "SomeMember0", item.SomeMember0, default(float));
        kernel.WriteFloat(writer, "SomeMember1", item.SomeMember1, default(Single));
        kernel.WriteFloat(writer, "SomeMember2", item.SomeMember2, default(float?));
        kernel.WriteFloat(writer, "SomeMember3", item.SomeMember3, default(Single?));
        kernel.WriteFloat(writer, "SomeMember4", item.SomeMember4, default(Nullable<float>));
        kernel.WriteFloat(writer, "SomeMember5", item.SomeMember5, default(Nullable<Single>));
        kernel.WriteFloat(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteFloat(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteFloat(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteFloat(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteFloat(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteFloat(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<float>.Default.Equals(item.SomeMember0, default(float))) return true;
        if (!EqualityComparer<Single>.Default.Equals(item.SomeMember1, default(Single))) return true;
        if (!EqualityComparer<float?>.Default.Equals(item.SomeMember2, default(float?))) return true;
        if (!EqualityComparer<Single?>.Default.Equals(item.SomeMember3, default(Single?))) return true;
        if (!EqualityComparer<Nullable<float>>.Default.Equals(item.SomeMember4, default(Nullable<float>))) return true;
        if (!EqualityComparer<Nullable<Single>>.Default.Equals(item.SomeMember5, default(Nullable<Single>))) return true;
        if (!EqualityComparer<float>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Single>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<float?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Single?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<float>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Single>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
                obj.SomeMember0 = SerializationHelper.StripNull(kernel.ReadFloat(reader), name: "SomeMember0");
                break;
            case "SomeMember1":
                obj.SomeMember1 = SerializationHelper.StripNull(kernel.ReadFloat(reader), name: "SomeMember1");
                break;
            case "SomeMember2":
                obj.SomeMember2 = kernel.ReadFloat(reader);
                break;
            case "SomeMember3":
                obj.SomeMember3 = kernel.ReadFloat(reader);
                break;
            case "SomeMember4":
                obj.SomeMember4 = kernel.ReadFloat(reader);
                break;
            case "SomeMember5":
                obj.SomeMember5 = kernel.ReadFloat(reader);
                break;
            case "SomeMember6":
                obj.SomeMember6 = SerializationHelper.StripNull(kernel.ReadFloat(reader), name: "SomeMember6");
                break;
            case "SomeMember7":
                obj.SomeMember7 = SerializationHelper.StripNull(kernel.ReadFloat(reader), name: "SomeMember7");
                break;
            case "SomeMember8":
                obj.SomeMember8 = kernel.ReadFloat(reader);
                break;
            case "SomeMember9":
                obj.SomeMember9 = kernel.ReadFloat(reader);
                break;
            case "SomeMember10":
                obj.SomeMember10 = kernel.ReadFloat(reader);
                break;
            case "SomeMember11":
                obj.SomeMember11 = kernel.ReadFloat(reader);
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

