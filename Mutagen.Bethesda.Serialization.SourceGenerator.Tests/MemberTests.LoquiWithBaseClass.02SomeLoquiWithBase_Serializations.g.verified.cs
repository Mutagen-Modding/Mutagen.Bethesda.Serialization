//HintName: SomeLoquiWithBase_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeLoquiWithBase_Serialization
{
    public static void Serialize<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBase Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

