using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

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

    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol, 
        string? fieldName) => false;

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "Mutagen.Bethesda.Strings";
    }
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public void GenerateForSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj, 
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor, 
        string metaAccessor,
        bool isInsideCollection,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        using (var c = sb.Call($"{kernelAccessor}.WriteTranslatedString", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add(fieldAccessor);
            if (defaultValueAccessor != null)
            {
                c.Add(defaultValueAccessor);
            }
            else
            {
                c.Add($"default({field})");
            }

            if (isInsideCollection)
            {
                c.Add("checkDefaults: false");
            }
        }
    }

    public bool HasVariableHasSerialize => true;

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if (!EqualityComparer<ITranslatedStringGetter?>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default(ITranslatedStringGetter?)"})) return true;");
    }

    public void GenerateForDeserializeSingleFieldInto(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string readerAccessor,
        string kernelAccessor,
        string metaAccessor,
        bool insideCollection,
        bool canSet,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        Utility.WrapStripNull(
            field, 
            fieldName,
            fieldAccessor, 
            readerAccessor, 
            kernelAccessor,
            insideCollection,
            sb,
            AddReadCall);
    }

    private void AddReadCall(
        StructuredStringBuilder sb,
        ITypeSymbol? field,
        string kernelAccessor,
        string readerAccessor,
        string fieldAccessor)
    {
        using (var c = sb.Call($"{fieldAccessor}{kernelAccessor}.ReadTranslatedString", linePerArgument: false))
        {
            c.Add(readerAccessor);
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}