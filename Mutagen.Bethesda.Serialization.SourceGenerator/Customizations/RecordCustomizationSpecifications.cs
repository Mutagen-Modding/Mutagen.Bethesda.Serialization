using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record Omission(string Name, ExpressionSyntax? Filter);

public record ContainerSortField(string ListFieldName, string ItemFieldName, int Priority);

public record RecordCustomizationSpecifications(ITypeSymbol CustomizationClass, LoquiTypeSet Target)
{
    public Dictionary<string, Omission>? ToOmit;
    public HashSet<string>? ToEmbedRecordsInSameFile;
    public List<ContainerSortField>? ContainerSortFields;
    public string? CurrentListField;

    public bool EmbedRecordForProperty(IPropertySymbol prop)
    {
        return EmbedRecordForProperty(prop.Name);
    }

    public bool EmbedRecordForProperty(string? name)
    {
        return name != null
               && ToEmbedRecordsInSameFile != null
               && ToEmbedRecordsInSameFile.Contains(name);
    }

    public bool HasContainerSortFields => ContainerSortFields is { Count: > 0 };
}