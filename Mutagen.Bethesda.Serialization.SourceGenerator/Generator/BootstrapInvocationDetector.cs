using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public record BootstrapInvocation(INamedTypeSymbol Bootstrap, INamedTypeSymbol? ModRegistration)
{
    public virtual bool Equals(BootstrapInvocation? other)
    {
        return SymbolEqualityComparer.IncludeNullability.Equals(ModRegistration, other?.ModRegistration)
               && Bootstrap.Equals(other?.Bootstrap, SymbolEqualityComparer.Default);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (SymbolEqualityComparer.Default.GetHashCode(Bootstrap) * 397)
                   ^ (ModRegistration != null ? SymbolEqualityComparer.IncludeNullability.GetHashCode(ModRegistration) : 0);
        }
    }
}

public class BootstrapInvocationDetector
{
    public IncrementalValuesProvider<BootstrapInvocation> GetBootstrapInvocations(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is MemberAccessExpressionSyntax,
                transform: GetBootstrapInvocation)
            .Where(x => x != null)!;
    }
    
    public BootstrapInvocation? GetBootstrapInvocation(GeneratorSyntaxContext context, CancellationToken cancel)
    {
        var memberAccessSyntax = (MemberAccessExpressionSyntax)context.Node;
        var expressionSymbol = context.SemanticModel.GetSymbolInfo(memberAccessSyntax.Expression).Symbol;
        if (expressionSymbol is IFieldSymbol fieldSymbol)
        {
            expressionSymbol = fieldSymbol.Type;
        }
        if (expressionSymbol is not INamedTypeSymbol namedTypeSymbol) return default;
        if (!namedTypeSymbol.AllInterfaces.Any(x => x.Name == "IMutagenSerializationBootstrap")) return default;
        
        var ret = new BootstrapInvocation(namedTypeSymbol, default);
        if (memberAccessSyntax.Parent is not InvocationExpressionSyntax invocationExpressionSyntax) return ret;
        if (invocationExpressionSyntax.ArgumentList.Arguments.Count != 1) return ret;
        
        var symb = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression).Symbol;
        if (symb == null) return ret;
        
        var type = symb.TryGetTypeSymbol();
        if (type == null) return ret;
        if (type.BaseType?.Name != "AMod") return ret;

        var getterInterface = type.AllInterfaces
            .FirstOrDefault(x => x.Name.EndsWith("ModGetter") &&
                        SymbolEqualityComparer.Default.Equals(x.ContainingNamespace, type.ContainingNamespace));
        if (getterInterface == null) return ret;

        return ret with { ModRegistration = getterInterface };
    }
}