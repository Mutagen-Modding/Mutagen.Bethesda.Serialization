//HintName: SubclassLoquiB_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SubclassLoquiB_Serialization
{
    public static void Serialize<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiBGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiB Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

