using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Bootstrapping;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

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