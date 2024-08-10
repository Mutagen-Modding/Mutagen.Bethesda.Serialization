using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record CustomizationCatalog(
    CustomizationSpecifications Overall,
    RecordCustomizationSpecifications? RecordSpecs)
{
    public bool EmbedRecordForProperty(IPropertySymbol prop)
    {
        return RecordSpecs?.ToEmbedRecordsInSameFile != null
               && RecordSpecs.ToEmbedRecordsInSameFile.Contains(prop.Name);
    }
    
    public bool EmbedRecordForProperty(ITypeSymbol prop)
    {
        return RecordSpecs?.ToEmbedRecordsInSameFile != null
               && RecordSpecs.ToEmbedRecordsInSameFile.Contains(prop.Name);
    }
}

public record CustomizationSpecifications(ITypeSymbol? CustomizationClass)
{
    public bool FilePerRecord { get; set; }
    public bool EnforceRecordOrder { get; set; }
    public bool OmitLastModifiedData { get; set; }
    public bool OmitTimestampData { get; set; }
}