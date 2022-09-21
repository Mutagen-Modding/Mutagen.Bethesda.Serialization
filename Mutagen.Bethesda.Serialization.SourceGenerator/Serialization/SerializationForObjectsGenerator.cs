using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

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
    
    public void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<BootstrapInvocation> bootstrapInvocations)
    {
        var distinctBootstraps = bootstrapInvocations.Collect()
            .Select((allSymbols, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return allSymbols
                    .Select(x => x.ObjectRegistration)
                    .Where(x => x != null)!
                    .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            });
        var mappings = context.CompilationProvider
            .Select((x, c) => _loquiMapper.GetMappings(x, c));
        var allClassesToGenerate = distinctBootstraps
            .SelectMany((items, _) => items)
            .Combine(mappings)
            .SelectMany((item, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return _accumulator.GetRelatedObjects(item.Right, item.Left!, cancel);
            })
            .Collect()
            .SelectMany((objs, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return objs.ToImmutableHashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
            });
        context.RegisterSourceOutput(
            allClassesToGenerate
                .Combine(context.CompilationProvider)
                .Combine(mappings),
            (c, i) =>
            {
                _serializationForObjectGenerator.Generate(new CompilationUnit(i.Left.Right, i.Right), c, i.Left.Left);
            });
    }
}