using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public class CustomizationProvider
{
    private readonly CustomizationDetector _customizationDetector;
    private readonly CustomizationInterpreter _interpreter;

    public CustomizationProvider(
        CustomizationDetector customizationDetector,
        CustomizationInterpreter interpreter)
    {
        _customizationDetector = customizationDetector;
        _interpreter = interpreter;
    }

    public IncrementalValueProvider<ImmutableDictionary<LoquiTypeSet, CustomizationCatalog>> Get(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<LoquiMapping> mappings)
    {
        return _interpreter.Interpret(_customizationDetector.GetCustomizationMethods(context), mappings)
            .Collect()
            .Select((i, c) =>
            {
                return i.ToImmutableDictionary(x => x.Target, x => x);
            });
    }
}