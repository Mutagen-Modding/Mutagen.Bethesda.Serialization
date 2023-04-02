using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record CustomizeMethodDeclaration(
    MethodDeclarationSyntax MethodSyntax,
    INamedTypeSymbol ContainingClass)
{
    public virtual bool Equals(CustomizeMethodDeclaration? other)
    {
        if (other == null) return false;
        return MethodSyntax.Equals(other.MethodSyntax);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return MethodSyntax.GetHashCode() * 397;
        }
    }
}

public class CustomizationDetector
{
    public IncrementalValueProvider<CustomizeMethodDeclaration?> GetCustomizationMethod(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is MethodDeclarationSyntax,
                transform: GetMethodDeclaration)
            .NotNull()
            .Collect()
            .Select((x, cancel) =>
            {
                // ToDo
                // Show error if more than 1 defined
                
                if (x.Length != 1) return default(CustomizeMethodDeclaration?);

                return x[0];
            });
    }

    public CustomizeMethodDeclaration? GetMethodDeclaration(GeneratorSyntaxContext context, CancellationToken cancel)
    {
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;
        if (methodDeclarationSyntax.Identifier.ToString() != "Customize") return default;
        if (methodDeclarationSyntax.ParameterList.Parameters.Count != 1) return default;
        if (methodDeclarationSyntax.ParameterList.Parameters[0].Type?.ToString() != "ICustomizationBuilder") return default;
        var containingClassDeclarationSyntax = methodDeclarationSyntax.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClassDeclarationSyntax == null) return default;
        var containingClass = context.SemanticModel.GetDeclaredSymbol(containingClassDeclarationSyntax);
        if (containingClass is not INamedTypeSymbol namedClass) return default;
        return new CustomizeMethodDeclaration(methodDeclarationSyntax, ContainingClass: namedClass);
    }
}