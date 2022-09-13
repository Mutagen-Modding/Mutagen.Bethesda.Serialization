//HintName: AbstractBaseLoqui_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class AbstractBaseLoqui_Serialization
{
    public static void SerializeWithCheck<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        switch (item)
        {
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiAGetter SubclassLoquiAGetter:
                Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiA_Serialization.Serialize(writer, SubclassLoquiAGetter, kernel);
                break;
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISubclassLoquiBGetter SubclassLoquiBGetter:
                Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubclassLoquiB_Serialization.Serialize(writer, SubclassLoquiBGetter, kernel);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static void Serialize<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoquiGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.IAbstractBaseLoqui Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

