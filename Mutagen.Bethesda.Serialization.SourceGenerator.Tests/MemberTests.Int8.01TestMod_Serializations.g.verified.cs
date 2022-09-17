//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteInt8(writer, "SomeMember0", item.SomeMember0, default(sbyte));
        kernel.WriteInt8(writer, "SomeMember1", item.SomeMember1, default(SByte));
        kernel.WriteInt8(writer, "SomeMember2", item.SomeMember2, default(Int8));
        kernel.WriteInt8(writer, "SomeMember3", item.SomeMember3, default(sbyte?));
        kernel.WriteInt8(writer, "SomeMember4", item.SomeMember4, default(SByte?));
        kernel.WriteInt8(writer, "SomeMember5", item.SomeMember5, default(Int8?));
        kernel.WriteInt8(writer, "SomeMember6", item.SomeMember6, default(Nullable<sbyte>));
        kernel.WriteInt8(writer, "SomeMember7", item.SomeMember7, default(Nullable<SByte>));
        kernel.WriteInt8(writer, "SomeMember8", item.SomeMember8, default(Nullable<Int8>));
        kernel.WriteInt8(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember9Default);
        kernel.WriteInt8(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember10Default);
        kernel.WriteInt8(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember11Default);
        kernel.WriteInt8(writer, "SomeMember12", item.SomeMember12, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember12Default);
        kernel.WriteInt8(writer, "SomeMember13", item.SomeMember13, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember13Default);
        kernel.WriteInt8(writer, "SomeMember14", item.SomeMember14, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember14Default);
        kernel.WriteInt8(writer, "SomeMember15", item.SomeMember15, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember15Default);
        kernel.WriteInt8(writer, "SomeMember16", item.SomeMember16, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember16Default);
        kernel.WriteInt8(writer, "SomeMember17", item.SomeMember17, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember17Default);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

