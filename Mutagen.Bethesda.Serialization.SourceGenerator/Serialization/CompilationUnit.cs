using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public record CompilationUnit(
    Compilation Compilation, 
    LoquiMapping Mapping,
    CustomizationCatalog Customization,
    SourceProductionContext Context);