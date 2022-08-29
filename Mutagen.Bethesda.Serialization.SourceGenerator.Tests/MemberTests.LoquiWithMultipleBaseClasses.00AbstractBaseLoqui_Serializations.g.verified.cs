//HintName: AbstractBaseLoqui_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class AbstractBaseLoqui_Serialization
{
    public static void SerializeWithCheck<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        switch (item)
        {
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiAGetter SubclassLoquiAGetter:
                SubclassLoquiA_Serialization.Serialize(SubclassLoquiAGetter, writer, kernel);
                break;
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiBGetter SubclassLoquiBGetter:
                SubclassLoquiB_Serialization.Serialize(SubclassLoquiBGetter, writer, kernel);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.AbstractBaseLoqui Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

