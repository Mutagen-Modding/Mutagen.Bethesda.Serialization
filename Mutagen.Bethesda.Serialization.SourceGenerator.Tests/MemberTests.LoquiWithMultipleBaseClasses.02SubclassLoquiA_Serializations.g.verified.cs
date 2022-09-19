//HintName: SubclassLoquiA_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SubclassLoquiA_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiAGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.Serialize<TKernel, TWriteObject>(writer, item, kernel);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiAGetter item)
    {
        if (Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui_Serialization.HasSerializationItems(item)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiA Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

