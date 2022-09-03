using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class ModInvocationDetector
{
    public IncrementalValuesProvider<BootstrapInvocation> Detect(IncrementalValuesProvider<BootstrapInvocation> bootstrapSymbols)
    {
        return bootstrapSymbols
            .Where(x => x.ObjectRegistration != null)
            .Collect()
            .SelectMany((x, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                return x.Distinct().ToImmutableHashSet();
            });
    }
}