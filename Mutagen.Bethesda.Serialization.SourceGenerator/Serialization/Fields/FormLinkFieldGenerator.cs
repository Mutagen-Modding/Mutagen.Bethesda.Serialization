using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FormLinkFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    private static readonly HashSet<string> _expectedStrings = new()
    {
        "FormLink",
        "FormLinkNullable",
        "IFormLink",
        "IFormLinkNullable",
        "IFormLinkGetter",
        "IFormLinkNullableGetter",
    };

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Mutagen.Bethesda.Plugins";
    }

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _expectedStrings.Contains(typeSymbol.Name);
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
        var nullable = field.Name.Contains("Nullable");
        using (var c = sb.Call($"{kernelAccessor}.WriteFormKey", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add($"{fieldAccessor}.FormKeyNullable");
            if (defaultValueAccessor != null)
            {
                c.Add($"{defaultValueAccessor}.FormKeyNullable");
            }
            else
            {
                c.Add($"default(FormKey{Utility.NullChar(nullable)})");
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
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"{fieldAccessor}.SetTo({kernelAccessor}.ReadFormKey({readerAccessor}));");
    }
}