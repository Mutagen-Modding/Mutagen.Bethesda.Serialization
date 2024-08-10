using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record CustomizationCatalog(
    CustomizationSpecifications Overall,
    RecordCustomizationSpecifications? RecordSpecs)
{
    public bool EmbedRecordForProperty(IPropertySymbol prop)
    {
        return EmbedRecordForProperty(prop.Name);
    }
    
    public bool EmbedRecordForProperty(string? name)
    {
        return name != null
               && RecordSpecs?.ToEmbedRecordsInSameFile != null
               && RecordSpecs.ToEmbedRecordsInSameFile.Contains(name);
    }
}

public record CustomizationSpecifications(ITypeSymbol? CustomizationClass)
{
    public bool FilePerRecord { get; set; }
    public bool EnforceRecordOrder { get; set; }
    public bool OmitLastModifiedData { get; set; }
    public bool OmitTimestampData { get; set; }
}