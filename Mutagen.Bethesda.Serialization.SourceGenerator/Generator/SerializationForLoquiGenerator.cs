using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationForLoquiGenerator
{
    private readonly RelatedObjectAccumulator _accumulator;
    private readonly SerializationForObjectGenerator _serializationForObjectGenerator;

    public SerializationForLoquiGenerator(
        RelatedObjectAccumulator accumulator,
        SerializationForObjectGenerator serializationForObjectGenerator)
    {
        _accumulator = accumulator;
        _serializationForObjectGenerator = serializationForObjectGenerator;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<BootstrapInvocation> modBootstrapInvocations)
    {
        var distinctModInvocations = modBootstrapInvocations.Collect()
            .Select((allSymbols, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return allSymbols
                    .Where(x => x.ModRegistration != null)
                    .ToImmutableHashSet();
            });
        var allClassesToGenerate = distinctModInvocations
            .SelectMany((mods, _) => mods)
            .SelectMany((mod, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return _accumulator.GetRelatedObjects(mod.ModRegistration!, cancel)
                    .Select(x => (x, mod.Bootstrap));
            });
        context.RegisterSourceOutput(
            allClassesToGenerate,
            (c, i) => _serializationForObjectGenerator.Generate(c, i.x, i.Bootstrap));
    }
}