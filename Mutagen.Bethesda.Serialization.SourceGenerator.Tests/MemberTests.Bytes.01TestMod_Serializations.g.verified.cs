//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteBytes(writer, "SomeBytes", item.SomeBytes, default(byte[]));
        kernel.WriteBytes(writer, "SomeBytes2", item.SomeBytes2, default(Noggog.ReadOnlyMemorySlice<byte>));
        kernel.WriteBytes(writer, "SomeBytes3", item.SomeBytes3, default(byte[]?));
        kernel.WriteBytes(writer, "SomeBytes4", item.SomeBytes4, default(Noggog.ReadOnlyMemorySlice<byte>?));
        kernel.WriteBytes(writer, "SomeBytes5", item.SomeBytes5, default(Nullable<Noggog.ReadOnlyMemorySlice<byte>>));
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

