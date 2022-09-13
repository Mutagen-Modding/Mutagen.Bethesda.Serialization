//HintName: SomeLoqui_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeLoqui_Serialization
{
    public static void Serialize<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoqui Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

