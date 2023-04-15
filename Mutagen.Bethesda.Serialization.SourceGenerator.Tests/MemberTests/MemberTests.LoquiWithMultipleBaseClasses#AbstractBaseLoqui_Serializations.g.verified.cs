//HintName: AbstractBaseLoqui_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class AbstractBaseLoqui_Serialization
{
    public static void SerializeWithCheck<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        kernel.WriteType(writer, LoquiRegistration.StaticRegister.GetRegister(item.GetType()).ClassType);
        switch (item)
        {
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiAGetter SubclassLoquiAGetter:
                Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiA_Serialization.Serialize(writer, SubclassLoquiAGetter, kernel, metaData);
                break;
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiBGetter SubclassLoquiBGetter:
                Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiB_Serialization.Serialize(writer, SubclassLoquiBGetter, kernel, metaData);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
    }

    public static bool HasSerializationItemsWithCheck(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
        SerializationMetaData metaData)
    {
        return true;
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui DeserializeWithCheck<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        var type = kernel.GetNextType(reader, "Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        switch (type.Name)
        {
            case "SubclassLoquiA":
                return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiA_Serialization.Deserialize(reader, kernel, metaData);
            case "SubclassLoquiB":
                return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiB_Serialization.Deserialize(reader, kernel, metaData);
            default:
                throw new NotImplementedException();
        }
    }

    public static void DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoqui obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            default:
                break;
        }
    }
    
    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoqui obj,
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

