﻿using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

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

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Mutagen.Bethesda.Strings";
    }

    public void GenerateForSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj, 
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor, 
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
        }
    }

    public bool HasVariableHasSerialize => true;

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if (!EqualityComparer<{field}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({field})"})) return true;");
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