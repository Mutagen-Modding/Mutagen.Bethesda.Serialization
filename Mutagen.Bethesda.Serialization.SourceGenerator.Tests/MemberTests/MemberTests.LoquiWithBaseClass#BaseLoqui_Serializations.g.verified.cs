//HintName: BaseLoqui_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class BaseLoqui_Serialization
{
    public static void SerializeWithCheck<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoquiGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteType(writer, LoquiRegistration.StaticRegister.GetRegister(item.GetType()).ClassType);
        switch (item)
        {
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter SomeLoquiWithBaseGetter:
                Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoquiWithBase_Serialization.Serialize(writer, SomeLoquiWithBaseGetter, kernel, metaData);
                break;
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoquiGetter IBaseLoquiGetter:
                Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.Serialize(writer, IBaseLoquiGetter, kernel, metaData);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoquiGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
    }

    public static bool HasSerializationItemsWithCheck(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoquiGetter item,
        SerializationMetaData metaData)
    {
        switch (item)
        {
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter SomeLoquiWithBaseGetter:
                return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoquiWithBase_Serialization.HasSerializationItems(SomeLoquiWithBaseGetter, metaData);
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoquiGetter IBaseLoquiGetter:
                return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.HasSerializationItems(IBaseLoquiGetter, metaData);
            default:
                throw new NotImplementedException();
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoquiGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui DeserializeWithCheck<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
    {
        switch (kernel.GetNextType(reader, "Mutagen.Bethesda.Serialization.SourceGenerator.Tests").Name)
        {
            case "SomeLoquiWithBase":
                return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoquiWithBase_Serialization.Deserialize(reader, kernel, metaData);
            case "BaseLoqui":
                return Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.Deserialize(reader, kernel, metaData);
            default:
                throw new NotImplementedException();
        }
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui();
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoqui obj,
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IBaseLoqui obj,
        SerializationMetaData metaData,
        string name)
    {
        switch (name)
        {
            default:
                break;
        }
    }

}

