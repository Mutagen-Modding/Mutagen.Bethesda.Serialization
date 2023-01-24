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

}

