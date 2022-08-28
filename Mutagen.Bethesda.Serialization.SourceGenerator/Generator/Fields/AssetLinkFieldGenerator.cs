using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

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
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        if (namedTypeSymbol.Name == "Nullable" 
            && namedTypeSymbol.TypeArguments.Length == 1
            && namedTypeSymbol.TypeArguments[0] is INamedTypeSymbol nullableType)
        {
            namedTypeSymbol = nullableType;
        }
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _expectedStrings.Contains(namedTypeSymbol.Name);
    }

    public void GenerateForSerialize(ITypeSymbol obj, ITypeSymbol field, string? fieldName, string fieldAccessor,
        string writerAccessor, string kernelAccessor, StructuredStringBuilder sb)
    {
        sb.AppendLine($"{kernelAccessor}.WriteString({writerAccessor}, {(fieldName == null ? "null" : $"\"{fieldName}\"")}, {fieldAccessor}.RawPath);");
    }

    public void GenerateForDeserialize(ITypeSymbol obj, IPropertySymbol propertySymbol, string itemAccessor, string writerAccessor,
        string kernelAccessor, StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }
}