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
        var metaData = new SerializationMetaData(item.GameRelease);
        if (item.SomeObject is {} SomeObjectChecked
            && Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.HasSerializationItems(SomeObjectChecked, metaData))
        {
            kernel.WriteLoqui(writer, "SomeObject", SomeObjectChecked, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
        }
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.HasSerializationItems(item.SomeObject, metaData)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "SomeObject":
                    item.SomeObject = kernel.ReadLoqui(reader, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TKernel, TReadObject>(r, k, m));
                default:
                    break;
            }
        }
    }

}

