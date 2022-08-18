using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationForObjectsGenerator
{
    private readonly RelatedObjectAccumulator _accumulator;
    private readonly MixinForModGenerator _mixinForModGenerator;
    private readonly SerializationForObjectGenerator _serializationForObjectGenerator;

    public SerializationForObjectsGenerator(
        RelatedObjectAccumulator accumulator,
        // MixinForModGenerator mixinForModGenerator,
        SerializationForObjectGenerator serializationForObjectGenerator)
    {
        _accumulator = accumulator;
        // _mixinForModGenerator = mixinForModGenerator;
        _serializationForObjectGenerator = serializationForObjectGenerator;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<BootstrapInvocation> modBootstrapInvocations)
    {
        // var distinctBootstrapPairings = modBootstrapInvocations.Collect()
        //     .Select((allSymbols, cancel) =>
        //     {
        //         cancel.ThrowIfCancellationRequested();
        //         return allSymbols
        //             .Where(x => x.ModRegistration != null)
        //             .ToImmutableHashSet();
        //     });
        // context.RegisterSourceOutput(
        //     distinctBootstrapPairings
        //         .SelectMany((mods, _) => mods),
        //     (c, i) => _mixinForModGenerator.Generate(c, i));
        
        var distinctMods = modBootstrapInvocations.Collect()
            .Select((allSymbols, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return allSymbols
                    .Select(x => x.ModRegistration)
                    .Where(x => x != null)!
                    .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            });
        var allClassesToGenerate = distinctMods
            .SelectMany((mods, _) => mods)
            .SelectMany((mod, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return _accumulator.GetRelatedObjects(mod!, cancel);
            });
        context.RegisterSourceOutput(
            allClassesToGenerate,
            (c, i) => _serializationForObjectGenerator.Generate(c, i));
    }
}