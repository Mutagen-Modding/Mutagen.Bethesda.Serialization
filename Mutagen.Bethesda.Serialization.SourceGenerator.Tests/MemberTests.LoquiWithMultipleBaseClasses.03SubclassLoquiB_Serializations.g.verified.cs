//HintName: SubclassLoquiB_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SubclassLoquiB_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiB item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        AbstractBaseLoqui_Serialization.Serialize<TWriteObject>(item, writer, kernel);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiB Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

