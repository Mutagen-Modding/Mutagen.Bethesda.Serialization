using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public class RecordCustomizationProvider
{
    private readonly RecordCustomizationDetector _recordCustomizationDetector;
    private readonly RecordCustomizationInterpreter _interpreter;

    public RecordCustomizationProvider(
        RecordCustomizationDetector recordCustomizationDetector,
        RecordCustomizationInterpreter interpreter)
    {
        _recordCustomizationDetector = recordCustomizationDetector;
        _interpreter = interpreter;
    }

    public IncrementalValueProvider<ImmutableDictionary<LoquiTypeSet, RecordCustomizationSpecifications>> Get(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<LoquiMapping> mappings)
    {
        return _interpreter.Interpret(
                _recordCustomizationDetector.GetCustomizationMethods(context),
                mappings)
            .Collect()
            .Select((i, c) =>
            {
                return i.ToImmutableDictionary(x => x.Target, x => x);
            });
    }
}