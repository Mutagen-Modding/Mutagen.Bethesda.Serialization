//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.WorkEngine;
using System.IO.Abstractions;
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
        await kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGenderedInt",
            item: item.SomeGenderedInt,
            serializationMetaData: metaData,
            writeCall: static async (w, o, k, m) =>
            {
                k.WriteString(w, "Male", o.Male, default(string), checkDefaults: false);
                k.WriteString(w, "Female", o.Female, default(string), checkDefaults: false);
            });
        await kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGenderedInt2",
            item: item.SomeGenderedInt2,
            serializationMetaData: metaData,
            writeCall: static async (w, o, k, m) =>
            {
                k.WriteString(w, "Male", o.Male, default(string), checkDefaults: false);
                k.WriteString(w, "Female", o.Female, default(string), checkDefaults: false);
            });
        await kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGenderedInt3",
            item: item.SomeGenderedInt3,
            serializationMetaData: metaData,
            writeCall: static async (w, o, k, m) =>
            {
                k.WriteString(w, "Male", o.Male, default(string), checkDefaults: false);
                k.WriteString(w, "Female", o.Female, default(string), checkDefaults: false);
            });
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt.Female, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt2.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt2.Female, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt3.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt3.Female, default(string))) return true;
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
            case "SomeGenderedInt":
                obj.SomeGenderedInt = await kernel.ReadLoqui(
                    reader: reader,
                    serializationMetaData: metaData,
                    readCall: static (r, k, m) =>
                    {
                        return SerializationHelper.ReadGenderedItem<ISerializationReaderKernel<TReadObject>, TReadObject, string>(
                            reader: r,
                            kernel: k,
                            metaData: m,
                            ret: new GenderedItem<string>(default(string), default(string)),
                            itemReader: static async (r2, k2, m2, n) =>
                            {
                                return SerializationHelper.StripNull(k2.ReadString(r2), name: "n");
                            });
                    });
                break;
            case "SomeGenderedInt2":
                obj.SomeGenderedInt2 = await kernel.ReadLoqui(
                    reader: reader,
                    serializationMetaData: metaData,
                    readCall: static (r, k, m) =>
                    {
                        return SerializationHelper.ReadGenderedItem<ISerializationReaderKernel<TReadObject>, TReadObject, string>(
                            reader: r,
                            kernel: k,
                            metaData: m,
                            ret: new GenderedItem<string>(default(string), default(string)),
                            itemReader: static async (r2, k2, m2, n) =>
                            {
                                return SerializationHelper.StripNull(k2.ReadString(r2), name: "n");
                            });
                    });
                break;
            case "SomeGenderedInt3":
                obj.SomeGenderedInt3 = await kernel.ReadLoqui(
                    reader: reader,
                    serializationMetaData: metaData,
                    readCall: static (r, k, m) =>
                    {
                        return SerializationHelper.ReadGenderedItem<ISerializationReaderKernel<TReadObject>, TReadObject, string>(
                            reader: r,
                            kernel: k,
                            metaData: m,
                            ret: new GenderedItem<string>(default(string), default(string)),
                            itemReader: static async (r2, k2, m2, n) =>
                            {
                                return SerializationHelper.StripNull(k2.ReadString(r2), name: "n");
                            });
                    });
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

