//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

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
        if (item.MyLoqui is {} MyLoquiChecked
            && Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.HasSerializationItemsWithCheck(MyLoquiChecked, metaData))
        {
            kernel.WriteLoqui(writer, "MyLoqui", MyLoquiChecked, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.SerializeWithCheck<TKernel, TWriteObject>(w, i, k, m));
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.HasSerializationItemsWithCheck(item.MyLoqui, metaData)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        while (kernel.TryGetNextField(out var name))
        {
            switch (name)
            {
                case: "MyLoqui":
                    item.MyLoqui = kernel.ReadLoqui(writer, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.DeserializeWithCheck<TKernel, TReadObject>(r, k, m));
                default:
                    break;
            }
        }
    }

}

