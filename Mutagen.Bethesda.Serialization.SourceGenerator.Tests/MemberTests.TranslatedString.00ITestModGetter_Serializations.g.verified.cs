//HintName: ITestModGetter_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class ITestModGetter_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.WriteTranslatedString(writer, item.TranslatedString);
        kernel.WriteTranslatedString(writer, item.TranslatedString2);
        kernel.WriteTranslatedString(writer, item.TranslatedString3);
        kernel.WriteTranslatedString(writer, item.TranslatedString4);
        kernel.WriteTranslatedString(writer, item.TranslatedString5);
        kernel.WriteTranslatedString(writer, item.TranslatedString6);
        kernel.WriteTranslatedString(writer, item.TranslatedString7);
        kernel.WriteTranslatedString(writer, item.TranslatedString8);
        kernel.WriteTranslatedString(writer, item.TranslatedString9);
        kernel.WriteTranslatedString(writer, item.TranslatedString10);
        kernel.WriteTranslatedString(writer, item.TranslatedString11);
        kernel.WriteTranslatedString(writer, item.TranslatedString12);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

