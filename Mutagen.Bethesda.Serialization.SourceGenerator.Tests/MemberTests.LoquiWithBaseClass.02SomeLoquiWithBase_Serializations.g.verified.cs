//HintName: SomeLoquiWithBase_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeLoquiWithBase_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoquiWithBase item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        BaseLoqui_Serialization.Serialize<TWriteObject>(item, writer, kernel);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoquiWithBase Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

