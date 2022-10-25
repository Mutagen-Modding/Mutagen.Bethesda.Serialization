//HintName: Group_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class Group_Serialization
{
    public static void Serialize<TKernel, TWriteObject, T>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IGroupGetter<T> item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where T : class, IMajorRecordInternal
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteInt32(writer, "SomeInt", item.SomeInt, default(int));
        kernel.WriteInt32(writer, "SomeInt2", item.SomeInt2, default(int));
    }

    public static bool HasSerializationItems<T>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IGroupGetter<T> item,
        SerializationMetaData metaData)
        where T : class, IMajorRecordInternal
    {
        return true;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group<T> Deserialize<TReadObject, T>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where T : class, IMajorRecordInternal
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group<T>();
        DeserializeInto<TReadObject, T>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static void DeserializeInto<TReadObject, T>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IGroup<T> obj,
        SerializationMetaData metaData)
        where T : class, IMajorRecordInternal
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "SomeInt":
                    obj.SomeInt = kernel.ReadInt32(reader);
                    break;
                case "Items":
                    break;
                case "SomeInt2":
                    obj.SomeInt2 = kernel.ReadInt32(reader);
                    break;
                default:
                    break;
            }
        }

    }

}

