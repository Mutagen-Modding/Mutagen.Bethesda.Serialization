//HintName: BaseLoqui_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class BaseLoqui_Serialization
{
    public static void SerializeWithCheck<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        switch (item)
        {
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeLoquiWithBaseGetter SomeLoquiWithBaseGetter:
                SomeLoquiWithBase_Serialization.Serialize(SomeLoquiWithBaseGetter, writer, kernel);
                break;
            case Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoquiGetter:
                BaseLoqui_Serialization.Serialize(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoquiGetter, writer, kernel);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.BaseLoqui Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

