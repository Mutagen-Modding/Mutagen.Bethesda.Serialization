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
        CompilationUnit compilation,
        ITypeSymbol obj, 
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string writerAccessor,
        string kernelAccessor, 
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"{kernelAccessor}.WriteTranslatedString({writerAccessor}, {(fieldName == null ? "null" : $"\"{fieldName}\"")}, {fieldAccessor});");
    }

    public void GenerateForDeserialize(
        CompilationUnit compilation,
        ITypeSymbol obj, 
        IPropertySymbol propertySymbol,
        string itemAccessor,
        string writerAccessor, 
        string kernelAccessor, 
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        throw new NotImplementedException();
    }
}