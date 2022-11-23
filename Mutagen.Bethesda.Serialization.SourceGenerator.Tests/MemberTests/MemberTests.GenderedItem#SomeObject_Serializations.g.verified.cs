//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

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
    {
        kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGenderedInt",
            item: item.SomeGenderedInt,
            serializationMetaData: metaData,
            writeCall: static (w, o, k, m) =>
            {
                k.WriteString(w, "Male", o.Male, default(string), checkDefaults: false);
                k.WriteString(w, "Female", o.Female, default(string), checkDefaults: false);
            });
        kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGenderedInt2",
            item: item.SomeGenderedInt2,
            serializationMetaData: metaData,
            writeCall: static (w, o, k, m) =>
            {
                k.WriteString(w, "Male", o.Male, default(string), checkDefaults: false);
                k.WriteString(w, "Female", o.Female, default(string), checkDefaults: false);
            });
        kernel.WriteLoqui(
            writer: writer,
            fieldName: "SomeGenderedInt3",
            item: item.SomeGenderedInt3,
            serializationMetaData: metaData,
            writeCall: static (w, o, k, m) =>
            {
                k.WriteString(w, "Male", o.Male, default(string), checkDefaults: false);
                k.WriteString(w, "Female", o.Female, default(string), checkDefaults: false);
            });
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt.Female, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt2.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt2.Female, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt3.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt3.Female, default(string))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
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

    public static void DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name)
    {
        switch (name)
        {
            case "SomeGenderedInt":
                obj.SomeGenderedInt = kernel.ReadLoqui(
                    reader: reader,
                    serializationMetaData: metaData,
                    readCall: static (r, k, m) =>
                    {
                        return SerializationHelper.ReadGenderedItem(
                            reader: r,
                            kernel: k,
                            metaData: m,
                            ret: new GenderedItem<string>(default(string), default(string)),
                            itemReader: static (r2, k2, m2, n) =>
                            {
                                return SerializationHelper.StripNull(k2.ReadString(r2), name: "n");
                            });
                    });
                break;
            case "SomeGenderedInt2":
                obj.SomeGenderedInt2 = kernel.ReadLoqui(
                    reader: reader,
                    serializationMetaData: metaData,
                    readCall: static (r, k, m) =>
                    {
                        return SerializationHelper.ReadGenderedItem(
                            reader: r,
                            kernel: k,
                            metaData: m,
                            ret: new GenderedItem<string>(default(string), default(string)),
                            itemReader: static (r2, k2, m2, n) =>
                            {
                                return SerializationHelper.StripNull(k2.ReadString(r2), name: "n");
                            });
                    });
                break;
            case "SomeGenderedInt3":
                obj.SomeGenderedInt3 = kernel.ReadLoqui(
                    reader: reader,
                    serializationMetaData: metaData,
                    readCall: static (r, k, m) =>
                    {
                        return SerializationHelper.ReadGenderedItem(
                            reader: r,
                            kernel: k,
                            metaData: m,
                            ret: new GenderedItem<string>(default(string), default(string)),
                            itemReader: static (r2, k2, m2, n) =>
                            {
                                return SerializationHelper.StripNull(k2.ReadString(r2), name: "n");
                            });
                    });
                break;
            default:
                break;
        }
    }

}

