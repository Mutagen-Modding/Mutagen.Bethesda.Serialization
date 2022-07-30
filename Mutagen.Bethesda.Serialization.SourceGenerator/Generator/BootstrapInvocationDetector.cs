using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public record BootstrapInvocation(
    ClassDetails NamedTypeSymbol,
    ClassDetails? ModRegistration);

public record ClassDetails(string ClassName, string Namespace)
{
    public override string ToString()
    {
        return $"{Namespace}.{ClassName}";
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
        
        var ret = new BootstrapInvocation(new ClassDetails(namedTypeSymbol.Name, namedTypeSymbol.ContainingNamespace.ToString()), default);
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

        return ret with { ModRegistration = new ClassDetails(getterInterface.Name, getterInterface.ContainingNamespace.Name) };
    }
}