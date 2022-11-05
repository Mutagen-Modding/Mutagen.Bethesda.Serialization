using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class SerializationForObjectsGenerator
{
    private readonly RelatedObjectAccumulator _accumulator;
    private readonly SerializationForObjectGenerator _serializationForObjectGenerator;

    public SerializationForObjectsGenerator(
        RelatedObjectAccumulator accumulator,
        SerializationForObjectGenerator serializationForObjectGenerator)
    {
        _accumulator = accumulator;
        _serializationForObjectGenerator = serializationForObjectGenerator;
    }
    
    public void Initialize(
        IncrementalGeneratorInitializationContext context, 
        IncrementalValuesProvider<BootstrapInvocation> bootstrapInvocations,
        IncrementalValueProvider<LoquiMapping> mappings,
        IncrementalValueProvider<ImmutableDictionary<LoquiTypeSet, CustomizationCatalog>> customizationDriver)
    {
        var distinctBootstraps = bootstrapInvocations.Collect()
            .Select((allSymbols, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return allSymbols
                    .Select(x => x.ObjectRegistration)
                    .NotNull()
                    .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            });
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
                return objs.ToImmutableHashSet();
            });
        context.RegisterSourceOutput(
            allClassesToGenerate
                .Combine(context.CompilationProvider)
                .Combine(mappings)
                .Combine(customizationDriver),
            (compilation, i) =>
            {
                var compUnit = new CompilationUnit(i.Left.Left.Right, i.Left.Right);
                var target = i.Left.Left.Left;
                CustomizationCatalog? customization = null;
                if (i.Right.TryGetValue(target, out var customDriver))
                {
                    customization = customDriver;
                }
                _serializationForObjectGenerator.Generate(
                    compUnit, 
                    customization,
                    compilation,
                    target);
            });
    }
}