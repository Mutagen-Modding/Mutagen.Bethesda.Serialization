using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Bootstrapping;
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
        IncrementalValueProvider<CustomizationSpecifications> customization,
        IncrementalValueProvider<ImmutableDictionary<LoquiTypeSet, RecordCustomizationSpecifications>> recordCustomizationDriver)
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
            .Combine(customization)
            .SelectMany((item, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return _accumulator.GetRelatedObjects(item.Left.Right, item.Left.Left!, item.Right, cancel);
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
                .Combine(recordCustomizationDriver)
                .Combine(customization),
            (compilation, i) =>
            {
                var target = i.Left.Left.Left.Left;
                RecordCustomizationSpecifications? recordCustomization = null;
                if (i.Left.Right.TryGetValue(target, out var customDriver))
                {
                    recordCustomization = customDriver;
                }
                var compUnit = new CompilationUnit(
                    i.Left.Left.Left.Right,
                    i.Left.Left.Right,
                    new CustomizationCatalog(
                        i.Right,
                        recordCustomization),
                    compilation);
                _serializationForObjectGenerator.Generate(
                    compUnit,
                    target);
            });
    }
}