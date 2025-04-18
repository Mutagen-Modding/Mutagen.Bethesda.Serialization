﻿using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class EnumFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        return Enumerable.Empty<string>();
    }

    public static bool IsApplicable(ITypeSymbol typeSymbol)
    {
        typeSymbol = typeSymbol.PeelNullable();
        return typeSymbol.TypeKind == TypeKind.Enum
                  || (typeSymbol.BaseType is { Name: "Enum" } 
                      && typeSymbol.BaseType.ContainingNamespace.ToString() == "System");
    }

    public bool Applicable(
        LoquiTypeSet obj, 
        CompilationUnit compilation,
        ITypeSymbol typeSymbol, 
        string? fieldName,
        bool isInsideCollection)
    {
        if (IsApplicable(typeSymbol)) return true;

        if (obj.Getter is INamedTypeSymbol namedTypeSymbol)
        {
            var match = namedTypeSymbol.TypeArguments.FirstOrDefault(t => t.Name == typeSymbol.Name);
            if (match is ITypeParameterSymbol typeParameterSymbol && typeParameterSymbol.ConstraintTypes.Any(x => x.Name == "Enum"))
            {
                return true;
            }
        }

        return false;
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
        bool isNullable = field.IsNullable();
        field = field.PeelNullable();
        using (var c = sb.Call($"{kernelAccessor}.WriteEnum<{field}>", linePerArgument: false))
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
                c.Add($"default({field}{(isNullable ? "?" : null)})");
            }

            if (isInsideCollection)
            {
                c.Add("checkDefaults: false");
            }
        }
    }

    public bool HasVariableHasSerialize => true;

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException($"No GetDefault defined for {typeof(EnumFieldGenerator)}");
    }

    public void GenerateForHasSerialize(CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string metaAccessor,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        sb.AppendLine($"if (!EqualityComparer<{field}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({field})"})) return true;");
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
        string setAccessor)
    {
        using (var c = sb.Call($"{setAccessor}{kernelAccessor}.ReadEnum<{field}>", linePerArgument: false))
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