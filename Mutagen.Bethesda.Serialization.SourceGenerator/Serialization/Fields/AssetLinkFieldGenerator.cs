using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class AssetLinkFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    private static readonly HashSet<string> _expectedStrings = new()
    {
        "AssetLink",
        "AssetLinkGetter",
        "IAssetLink",
        "IAssetLinkGetter",
        "Mutagen.Bethesda.Plugins.Assets.AssetLink",
        "Mutagen.Bethesda.Plugins.Assets.IAssetLink",
        "Mutagen.Bethesda.Plugins.Assets.IAssetLinkGetter",
    };

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Mutagen.Bethesda.Plugins.Assets";
    }
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        namedTypeSymbol = namedTypeSymbol.PeelNullable();
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _expectedStrings.Contains(namedTypeSymbol.Name);
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
        using (var c = sb.Call($"{kernelAccessor}.WriteString", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add($"{fieldAccessor}?.RawPath");
            if (defaultValueAccessor != null)
            {
                c.Add($"{defaultValueAccessor}.RawPath");
            }
            else
            {
                c.Add($"default(string{field.NullChar()})");
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
        using (var c = sb.Call($"{fieldAccessor} = {kernelAccessor}.ReadString", linePerArgument: false))
        {
            c.Add(readerAccessor);
        }
    }
}