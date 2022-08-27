using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class TranslatedStringFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => new string[]
    {
        "TranslatedString",
        "ITranslatedStringGetter",
        "ITranslatedString",
        "TranslatedString?",
        "ITranslatedStringGetter?",
        "ITranslatedString?",
        "Mutagen.Bethesda.Strings.TranslatedString",
        "Mutagen.Bethesda.Strings.ITranslatedStringGetter",
        "Mutagen.Bethesda.Strings.ITranslatedString",
        "Mutagen.Bethesda.Strings.TranslatedString?",
        "Mutagen.Bethesda.Strings.ITranslatedStringGetter?",
        "Mutagen.Bethesda.Strings.ITranslatedString?",
    };

    public bool Applicable(ITypeSymbol typeSymbol) => false;

    public void GenerateForSerialize(
        ITypeSymbol obj, 
        ITypeSymbol field,
        string fieldAccessor,
        string writerAccessor,
        string kernelAccessor, 
        StructuredStringBuilder sb)
    {
        sb.AppendLine($"{kernelAccessor}.WriteTranslatedString({writerAccessor}, {fieldAccessor});");
    }

    public void GenerateForDeserialize(ITypeSymbol obj, IPropertySymbol propertySymbol,
        string itemAccessor, string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}