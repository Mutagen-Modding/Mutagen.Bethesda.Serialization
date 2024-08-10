using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
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

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "Mutagen.Bethesda.Plugins.Assets";
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;
    
    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationCatalog customization,
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        namedTypeSymbol = namedTypeSymbol.PeelNullable();
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _expectedStrings.Contains(namedTypeSymbol.Name);
    }

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
        using (var c = sb.Call($"{kernelAccessor}.WriteString", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add($"{fieldAccessor}?.GivenPath");
            if (defaultValueAccessor != null)
            {
                c.Add($"{defaultValueAccessor}.GivenPath");
            }
            else
            {
                c.Add($"default(string{field.NullChar()})");
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
        throw new NotImplementedException($"No GetDefault defined for {typeof(AssetLinkFieldGenerator)}");
    }

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
        var named = (INamedTypeSymbol)field;
        var sub = named.TypeArguments[0];
        var linkStr = $"IAssetLinkGetter<{sub}>";
        sb.AppendLine($"if (!EqualityComparer<{linkStr}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({linkStr})"})) return true;");
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
        var named = (INamedTypeSymbol)field;
        if (insideCollection)
        {
            if (field.IsNullable())
            {
                sb.AppendLine($"{fieldAccessor} (AssetLink<{named.TypeArguments[0]}>?){kernelAccessor}.ReadString({readerAccessor});");
            }
            else
            {
                sb.AppendLine($"{fieldAccessor} (AssetLink<{named.TypeArguments[0]}>){kernelAccessor}.ReadString({readerAccessor}).StripNull(\"{fieldName}\");");
            }
        }
        else
        {
            using (var c = sb.Call($"{fieldAccessor} = {kernelAccessor}.ReadString", linePerArgument: false,
                       suffixLine: $".StripNull(name: \"{fieldName}\")"))
            {
                c.Add(readerAccessor);
            }
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}