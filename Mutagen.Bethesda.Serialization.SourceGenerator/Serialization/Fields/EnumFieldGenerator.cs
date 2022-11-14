using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class EnumFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        return Enumerable.Empty<string>();
    }

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        typeSymbol = typeSymbol.PeelNullable();
        return typeSymbol.TypeKind == TypeKind.Enum
            || (typeSymbol.BaseType is { Name: "Enum" } 
                && typeSymbol.BaseType.ContainingNamespace.ToString() == "System");
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public void GenerateForSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj, 
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor, 
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
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
                c.Add($"default({field})");
            }
        }
    }

    public bool HasVariableHasSerialize => true;

    public void GenerateForHasSerialize(CompilationUnit compilation,
        ITypeSymbol obj,
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

    public void GenerateForDeserialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
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
        field = field.PeelNullable();
        fieldAccessor = $"{fieldAccessor}{(insideCollection ? null : " = ")}";
        
        if (field.IsNullable())
        {
            AddReadCall(sb, field, kernelAccessor, readerAccessor, fieldAccessor);
        }
        
        using (var strip = sb.Call($"{fieldAccessor}SerializationHelper.StripNull", linePerArgument: false))
        {
            strip.Add((subSb) =>
            {
                AddReadCall(subSb, field, kernelAccessor, readerAccessor, string.Empty);
            });
            if (fieldName == null) throw new NullReferenceException();
            strip.Add($"name: \"{fieldName}\"");
        }
    }

    private void AddReadCall(
        StructuredStringBuilder sb,
        ITypeSymbol field,
        string kernelAccessor,
        string readerAccessor,
        string setAccessor)
    {
        using (var c = sb.Call($"{setAccessor}{kernelAccessor}.ReadEnum<{field}>", linePerArgument: false))
        {
            c.Add(readerAccessor);
        }
    }
}