using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record CustomizationCatalog(
    CustomizationSpecifications Overall,
    RecordCustomizationSpecifications? RecordSpecs);

public record CustomizationSpecifications(ITypeSymbol? CustomizationClass)
{
    public bool FilePerRecord { get; set; }
    public bool EnforceRecordOrder { get; set; }
}