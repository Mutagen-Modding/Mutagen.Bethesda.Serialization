using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record CustomizationCatalog(
    CustomizationSpecifications Overall,
    RecordCustomizationSpecifications? TargetRecordSpecs,
    ImmutableDictionary<LoquiTypeSet, RecordCustomizationSpecifications> RecordSpecs)
{
}

public record CustomizationSpecifications(ITypeSymbol? CustomizationClass)
{
    public bool FilePerRecord { get; set; }
    public bool EnforceRecordOrder { get; set; }
    public bool OmitLastModifiedData { get; set; }
    public bool OmitTimestampData { get; set; }
}