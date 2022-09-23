//HintName: Group_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

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

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IGroup<T> Deserialize<TReadObject, T>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
        where T : class, IMajorRecordInternal
    {
        while (kernel.TryGetNextField(out var name))
        {
            switch (name)
            {
                case: "SomeInt":
                    item.SomeInt = kernel.ReadInt32(writer);
                case: "Items":
                case: "SomeInt2":
                    item.SomeInt2 = kernel.ReadInt32(writer);
                default:
                    break;
            }
        }
    }

}

