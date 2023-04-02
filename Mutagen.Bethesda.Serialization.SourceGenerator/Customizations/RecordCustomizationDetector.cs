using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record CustomizeRecordMethodDeclaration(
    MethodDeclarationSyntax MethodSyntax,
    INamedTypeSymbol Target,
    INamedTypeSymbol ContainingClass)
{
    public virtual bool Equals(CustomizeRecordMethodDeclaration? other)
    {
        if (other == null) return false;
        return Target.Equals(other.Target, SymbolEqualityComparer.Default)
            && MethodSyntax.Equals(other.MethodSyntax);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (SymbolEqualityComparer.Default.GetHashCode(Target) * 397)
                   ^ MethodSyntax.GetHashCode() * 397;
        }
    }
}

public class RecordCustomizationDetector
{
    public IncrementalValuesProvider<CustomizeRecordMethodDeclaration> GetCustomizationMethods(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is MethodDeclarationSyntax,
                transform: GetMethodDeclaration)
            .NotNull();
    }

    public CustomizeRecordMethodDeclaration? GetMethodDeclaration(GeneratorSyntaxContext context, CancellationToken cancel)
    {
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;
        if (methodDeclarationSyntax.Identifier.ToString() != "CustomizeFor") return default;
        if (methodDeclarationSyntax.ParameterList.Parameters.Count != 1) return default;
        if (methodDeclarationSyntax.ParameterList.Parameters[0].Type is not GenericNameSyntax genericNameSyntax)
            return default;
        if (genericNameSyntax.TypeArgumentList.Arguments.Count != 1) return default;
        if (genericNameSyntax.Identifier.ToString() != "ICustomizationBuilder") return default;
        var p = genericNameSyntax.TypeArgumentList.Arguments[0];
        var targetSymbol = context.SemanticModel.GetSymbolInfo(p).Symbol;
        if (targetSymbol is not INamedTypeSymbol namedTargetSymbol) return default;
        var containingClassDeclarationSyntax = methodDeclarationSyntax.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClassDeclarationSyntax == null) return default;
        var containingClass = context.SemanticModel.GetDeclaredSymbol(containingClassDeclarationSyntax);
        if (containingClass is not INamedTypeSymbol namedClass) return default;
        return new CustomizeRecordMethodDeclaration(methodDeclarationSyntax, namedTargetSymbol, ContainingClass: namedClass);
    }
}