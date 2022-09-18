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
        if (item.MyLoqui is {} MyLoquiChecked
            && Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.HasSerializationItemsWithCheck(MyLoquiChecked))
        {
            kernel.WriteLoqui(writer, "MyLoqui", MyLoquiChecked, static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.SerializeWithCheck<TKernel, TWriteObject>(w, i, k));
        }
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.HasSerializationItemsWithCheck(item.MyLoqui)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

