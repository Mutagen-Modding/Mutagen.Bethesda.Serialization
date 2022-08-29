//HintName: SubclassLoquiA_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SubclassLoquiA_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiA item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        AbstractBaseLoqui_Serialization.Serialize<TWriteObject>(item, writer, kernel);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiA Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

