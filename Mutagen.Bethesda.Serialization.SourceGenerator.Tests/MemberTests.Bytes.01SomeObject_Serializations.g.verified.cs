//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Noggog;

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
        kernel.WriteBytes(writer, "SomeBytes", item.SomeBytes, default(byte[]));
        kernel.WriteBytes(writer, "SomeBytes2", item.SomeBytes2, default(Noggog.ReadOnlyMemorySlice<byte>));
        kernel.WriteBytes(writer, "SomeBytes3", item.SomeBytes3, default(byte[]?));
        kernel.WriteBytes(writer, "SomeBytes4", item.SomeBytes4, default(Noggog.ReadOnlyMemorySlice<byte>?));
        kernel.WriteBytes(writer, "SomeBytes5", item.SomeBytes5, default(Nullable<Noggog.ReadOnlyMemorySlice<byte>>));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!MemorySliceExt.Equal<byte>(item.SomeBytes, default(byte[]))) return true;
        if (!MemorySliceExt.Equal<byte>(item.SomeBytes2, default(Noggog.ReadOnlyMemorySlice<byte>))) return true;
        if (!MemorySliceExt.Equal<byte>(item.SomeBytes3, default(byte[]?))) return true;
        if (!MemorySliceExt.Equal<byte>(item.SomeBytes4, default(Noggog.ReadOnlyMemorySlice<byte>?))) return true;
        if (!MemorySliceExt.Equal<byte>(item.SomeBytes5, default(Nullable<Noggog.ReadOnlyMemorySlice<byte>>))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

