//HintName: Group_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class Group_Serialization
{
    public static void Serialize<TWriteObject, T>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IGroupGetter<T> item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
        where T : class, IMajorRecordInternal
    {
        kernel.WriteInt32(writer, "SomeInt", item.SomeInt);
        kernel.WriteInt32(writer, "SomeInt2", item.SomeInt2);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IGroup<T> Deserialize<TReadObject, T>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
        where T : class, IMajorRecordInternal
    {
        throw new NotImplementedException();
    }

}

