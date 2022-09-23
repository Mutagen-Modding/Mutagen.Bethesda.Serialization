//HintName: SomeLoquiWithBase_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeLoquiWithBase_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.Serialize<TKernel, TWriteObject>(writer, item, kernel, metaData);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter item,
        SerializationMetaData metaData)
    {
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.HasSerializationItems(item, metaData)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBase Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui_Serialization.Serialize<TReadObject>(writer, item, kernel, metaData);
        while (kernel.TryGetNextField(out var name))
        {
            switch (name)
            {
                default:
                    break;
            }
        }
    }

}

