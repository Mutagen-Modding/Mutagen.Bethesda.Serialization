//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
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
        kernel.WriteFormKey(writer, "SomeMember0", item.SomeMember0, default(FormKey));
        kernel.WriteFormKey(writer, "SomeMember1", item.SomeMember1, default(Mutagen.Bethesda.Plugins.FormKey));
        kernel.WriteFormKey(writer, "SomeMember2", item.SomeMember2, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeMember3", item.SomeMember3, default(Mutagen.Bethesda.Plugins.FormKey?));
        kernel.WriteFormKey(writer, "SomeMember4", item.SomeMember4, default(Nullable<FormKey>));
        kernel.WriteFormKey(writer, "SomeMember5", item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.FormKey>));
        kernel.WriteFormKey(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteFormKey(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteFormKey(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteFormKey(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteFormKey(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteFormKey(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<FormKey>.Default.Equals(item.SomeMember0, default(FormKey))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey>.Default.Equals(item.SomeMember1, default(Mutagen.Bethesda.Plugins.FormKey))) return true;
        if (!EqualityComparer<FormKey?>.Default.Equals(item.SomeMember2, default(FormKey?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey?>.Default.Equals(item.SomeMember3, default(Mutagen.Bethesda.Plugins.FormKey?))) return true;
        if (!EqualityComparer<Nullable<FormKey>>.Default.Equals(item.SomeMember4, default(Nullable<FormKey>))) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.FormKey>>.Default.Equals(item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.FormKey>))) return true;
        if (!EqualityComparer<FormKey>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<FormKey?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<FormKey>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.FormKey>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
            case "SomeMember0":
                break;
            case "SomeMember1":
                break;
            case "SomeMember2":
                break;
            case "SomeMember3":
                break;
            case "SomeMember4":
                break;
            case "SomeMember5":
                break;
            case "SomeMember6":
                break;
            case "SomeMember7":
                break;
            case "SomeMember8":
                break;
            case "SomeMember9":
                break;
            case "SomeMember10":
                break;
            case "SomeMember11":
                break;
            default:
                break;
        }
    }

}

