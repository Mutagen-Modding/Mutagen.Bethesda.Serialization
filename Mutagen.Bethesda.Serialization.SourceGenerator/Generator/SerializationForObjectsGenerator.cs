using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationForObjectsGenerator
{
    private readonly RelatedObjectAccumulator _accumulator;
    private readonly LoquiMapper _loquiMapper;
    private readonly SerializationForObjectGenerator _serializationForObjectGenerator;

    public SerializationForObjectsGenerator(
        RelatedObjectAccumulator accumulator,
        LoquiMapper loquiMapper,
        SerializationForObjectGenerator serializationForObjectGenerator)
    {
        _accumulator = accumulator;
        _loquiMapper = loquiMapper;
        _serializationForObjectGenerator = serializationForObjectGenerator;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<BootstrapInvocation> modBootstrapInvocations)
    {
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
            .Combine(context.CompilationProvider)
            .SelectMany((mod, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                var mapping = _loquiMapper.GetMappings(mod.Right, cancel);
                return _accumulator.GetRelatedObjects(mapping, mod.Left!, cancel)
                    .Select(x => (Type: x, Compilation: new CompilationUnit(mod.Right, mapping)));
            });
        context.RegisterSourceOutput(
            allClassesToGenerate,
            (c, i) => _serializationForObjectGenerator.Generate(i.Compilation, c, i.Type));
    }
}