using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

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

    public IncrementalValueProvider<ImmutableDictionary<ITypeSymbol, CustomizationCatalog>> Get(
        IncrementalGeneratorInitializationContext context)
    {
        return _interpreter.Interpret(_customizationDetector.GetCustomizationMethods(context))
            .Collect()
            .Select((i, c) =>
            {
                return i.ToImmutableDictionary<CustomizationCatalog, ITypeSymbol, CustomizationCatalog>(
                    x => x.Target, x => x, SymbolEqualityComparer.Default);
            });
    }
}