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

    public IncrementalValueProvider<CustomizationSpecifications> Get(
        IncrementalGeneratorInitializationContext context)
    {
        return _interpreter.Interpret(_customizationDetector.GetCustomizationMethod(context));
    }
}