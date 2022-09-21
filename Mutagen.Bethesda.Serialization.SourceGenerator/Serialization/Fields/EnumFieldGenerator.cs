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