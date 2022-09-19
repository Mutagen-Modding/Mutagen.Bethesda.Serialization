//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
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

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<TranslatedString>.Default.Equals(item.TranslatedString, default(TranslatedString))) return true;
        if (!EqualityComparer<ITranslatedString>.Default.Equals(item.TranslatedString2, default(ITranslatedString))) return true;
        if (!EqualityComparer<ITranslatedStringGetter>.Default.Equals(item.TranslatedString3, default(ITranslatedStringGetter))) return true;
        if (!EqualityComparer<TranslatedString?>.Default.Equals(item.TranslatedString4, default(TranslatedString?))) return true;
        if (!EqualityComparer<ITranslatedString?>.Default.Equals(item.TranslatedString5, default(ITranslatedString?))) return true;
        if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item.TranslatedString6, default(ITranslatedStringGetter?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Strings.TranslatedString>.Default.Equals(item.TranslatedString7, default(Mutagen.Bethesda.Strings.TranslatedString))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Strings.ITranslatedString>.Default.Equals(item.TranslatedString8, default(Mutagen.Bethesda.Strings.ITranslatedString))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Strings.ITranslatedStringGetter>.Default.Equals(item.TranslatedString9, default(Mutagen.Bethesda.Strings.ITranslatedStringGetter))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Strings.TranslatedString?>.Default.Equals(item.TranslatedString10, default(Mutagen.Bethesda.Strings.TranslatedString?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Strings.ITranslatedString?>.Default.Equals(item.TranslatedString11, default(Mutagen.Bethesda.Strings.ITranslatedString?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Strings.ITranslatedStringGetter?>.Default.Equals(item.TranslatedString12, default(Mutagen.Bethesda.Strings.ITranslatedStringGetter?))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

