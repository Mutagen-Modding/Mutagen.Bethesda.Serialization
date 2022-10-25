//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Noggog;

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
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes, default(byte[]))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes2, default(Noggog.ReadOnlyMemorySlice<byte>))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes3, default(byte[]?))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes4, default(Noggog.ReadOnlyMemorySlice<byte>?))) return true;
        if (!MemorySliceExt.SequenceEqual<byte>(item.SomeBytes5, default(Nullable<Noggog.ReadOnlyMemorySlice<byte>>))) return true;
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
            switch (name)
            {
                case "SomeBytes":
                    obj.SomeBytes = kernel.ReadBytes(reader);
                    break;
                case "SomeBytes2":
                    obj.SomeBytes2 = kernel.ReadBytes(reader);
                    break;
                case "SomeBytes3":
                    obj.SomeBytes3 = kernel.ReadBytes(reader);
                    break;
                case "SomeBytes4":
                    obj.SomeBytes4 = kernel.ReadBytes(reader);
                    break;
                case "SomeBytes5":
                    obj.SomeBytes5 = kernel.ReadBytes(reader);
                    break;
                default:
                    break;
            }
        }

    }

}

