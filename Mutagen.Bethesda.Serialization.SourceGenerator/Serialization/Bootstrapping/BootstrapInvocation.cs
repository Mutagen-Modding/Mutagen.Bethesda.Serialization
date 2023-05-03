using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Bootstrapping;

public record BootstrapInvocation(INamedTypeSymbol Bootstrap, INamedTypeSymbol? ObjectRegistration)
{
    public virtual bool Equals(BootstrapInvocation? other)
    {
        return SymbolEqualityComparer.IncludeNullability.Equals(ObjectRegistration, other?.ObjectRegistration)
               && Bootstrap.Equals(other?.Bootstrap, SymbolEqualityComparer.Default);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (SymbolEqualityComparer.Default.GetHashCode(Bootstrap) * 397)
                   ^ (ObjectRegistration != null ? SymbolEqualityComparer.IncludeNullability.GetHashCode(ObjectRegistration) : 0) * 397;
        }
    }
}