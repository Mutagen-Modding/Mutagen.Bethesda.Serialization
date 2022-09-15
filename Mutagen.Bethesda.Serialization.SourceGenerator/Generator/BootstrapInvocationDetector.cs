using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

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
                   ^ (ObjectRegistration != null ? SymbolEqualityComparer.IncludeNullability.GetHashCode(ObjectRegistration) : 0);
        }
    }
}

public class BootstrapInvocationDetector
{
    private readonly IsLoquiObjectTester _loquiObjectTester;

    public BootstrapInvocationDetector(
        IsLoquiObjectTester loquiObjectTester)
    {
        _loquiObjectTester = loquiObjectTester;
    }
    
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
        if (invocationExpressionSyntax.ArgumentList.Arguments.Count is not (2 or 1)) return ret;
        
        var symb = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression).Symbol;
        if (symb == null) return ret;
        
        var type = symb.TryGetTypeSymbol();
        if (type == null) return ret;

        if (!_loquiObjectTester.IsLoqui(type)) return ret;
        
        var getterInterface = type.AllInterfaces
            .And(type)
            .WhereCastable<ITypeSymbol, INamedTypeSymbol>()
            .FirstOrDefault(x => x.Name.EndsWith("Getter") &&
                        SymbolEqualityComparer.Default.Equals(x.ContainingNamespace, type.ContainingNamespace));
        if (getterInterface == null) return ret;

        return ret with { ObjectRegistration = getterInterface };
    }
}