//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteTranslatedString(writer, "TranslatedString", item.TranslatedString, default(TranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString2", item.TranslatedString2, default(ITranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString3", item.TranslatedString3, default(ITranslatedStringGetter));
        kernel.WriteTranslatedString(writer, "TranslatedString4", item.TranslatedString4, default(TranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString5", item.TranslatedString5, default(ITranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString6", item.TranslatedString6, default(ITranslatedStringGetter?));
        kernel.WriteTranslatedString(writer, "TranslatedString7", item.TranslatedString7, default(Mutagen.Bethesda.Strings.TranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString8", item.TranslatedString8, default(Mutagen.Bethesda.Strings.ITranslatedString));
        kernel.WriteTranslatedString(writer, "TranslatedString9", item.TranslatedString9, default(Mutagen.Bethesda.Strings.ITranslatedStringGetter));
        kernel.WriteTranslatedString(writer, "TranslatedString10", item.TranslatedString10, default(Mutagen.Bethesda.Strings.TranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString11", item.TranslatedString11, default(Mutagen.Bethesda.Strings.ITranslatedString?));
        kernel.WriteTranslatedString(writer, "TranslatedString12", item.TranslatedString12, default(Mutagen.Bethesda.Strings.ITranslatedStringGetter?));
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

