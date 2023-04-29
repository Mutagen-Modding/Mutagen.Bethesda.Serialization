//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Strings;
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
        kernel.WriteTranslatedString(writer, "TranslatedString", item.TranslatedString, default(TranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString2", item.TranslatedString2, default(ITranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString3", item.TranslatedString3, default(ITranslatedStringGetter));
        kernel.WriteTranslatedString(writer, "TranslatedString4", item.TranslatedString4, default(TranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString5", item.TranslatedString5, default(ITranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString6", item.TranslatedString6, default(ITranslatedStringGetter?));
        kernel.WriteTranslatedString(writer, "TranslatedString7", item.TranslatedString7, default(Mutagen.Bethesda.Strings.TranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString8", item.TranslatedString8, default(Mutagen.Bethesda.Strings.ITranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString9", item.TranslatedString9, default(Mutagen.Bethesda.Strings.ITranslatedStringGetter));
        kernel.WriteTranslatedString(writer, "TranslatedString10", item.TranslatedString10, default(Mutagen.Bethesda.Strings.TranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString11", item.TranslatedString11, default(Mutagen.Bethesda.Strings.ITranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString12", item.TranslatedString12, default(Mutagen.Bethesda.Strings.ITranslatedStringGetter?));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString2, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString3, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString4, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString5, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString6, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString7, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString8, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString9, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString10, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString11, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString12, default(ITranslatedStringGetter?))) return true;
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
            case "TranslatedString":
                obj.TranslatedString = SerializationHelper.StripNull(kernel.ReadTranslatedString(reader), name: "TranslatedString");
                break;
            case "TranslatedString2":
                obj.TranslatedString2 = SerializationHelper.StripNull(kernel.ReadTranslatedString(reader), name: "TranslatedString2");
                break;
            case "TranslatedString3":
                obj.TranslatedString3 = SerializationHelper.StripNull(kernel.ReadTranslatedString(reader), name: "TranslatedString3");
                break;
            case "TranslatedString4":
                obj.TranslatedString4 = kernel.ReadTranslatedString(reader);
                break;
            case "TranslatedString5":
                obj.TranslatedString5 = kernel.ReadTranslatedString(reader);
                break;
            case "TranslatedString6":
                obj.TranslatedString6 = kernel.ReadTranslatedString(reader);
                break;
            case "TranslatedString7":
                obj.TranslatedString7 = SerializationHelper.StripNull(kernel.ReadTranslatedString(reader), name: "TranslatedString7");
                break;
            case "TranslatedString8":
                obj.TranslatedString8 = SerializationHelper.StripNull(kernel.ReadTranslatedString(reader), name: "TranslatedString8");
                break;
            case "TranslatedString9":
                obj.TranslatedString9 = SerializationHelper.StripNull(kernel.ReadTranslatedString(reader), name: "TranslatedString9");
                break;
            case "TranslatedString10":
                obj.TranslatedString10 = kernel.ReadTranslatedString(reader);
                break;
            case "TranslatedString11":
                obj.TranslatedString11 = kernel.ReadTranslatedString(reader);
                break;
            case "TranslatedString12":
                obj.TranslatedString12 = kernel.ReadTranslatedString(reader);
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

