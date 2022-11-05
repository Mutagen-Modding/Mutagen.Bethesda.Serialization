using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public class CustomizationInterpreter
{
    public IncrementalValuesProvider<CustomizationCatalog> Interpret(
        IncrementalValuesProvider<CustomizeMethodDeclaration> customizationDeclarations,
        IncrementalValueProvider<LoquiMapping> mappings)
    {
        return customizationDeclarations
            .Combine(mappings)
            .Select((x, c) => InterpretDriver(x.Left, x.Right, c))
            .NotNull();
    }

    private CustomizationCatalog? InterpretDriver(
        CustomizeMethodDeclaration decl,
        LoquiMapping mapping,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (!mapping.TryGetTypeSet(decl.Target, out var typeSet))
        {
            // ToDo
            // Add error
            return default;
        }
        var driver = new CustomizationCatalog(decl.ContainingClass, typeSet);
        foreach (var invoke in decl.MethodSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            cancel.ThrowIfCancellationRequested();
            if (!AddCustomization(driver, invoke))
            {
                // ToDo
                // Add error
            }
        }

        return driver;
    }

    private bool AddCustomization(
        CustomizationCatalog catalog,
        InvocationExpressionSyntax invoke)
    {
        if (invoke.Expression is not MemberAccessExpressionSyntax member) return false;
        switch (member.Name.ToString())
        {
            case "Omit" when invoke.ArgumentList.Arguments.Count is 1 or 2:
                return HandleOmit(catalog, invoke, member);
            default:
                return false;
        }
    }

    private bool HandleOmit(
        CustomizationCatalog catalog,
        InvocationExpressionSyntax invoke,
        MemberAccessExpressionSyntax memberAccess)
    {
        if (invoke.ArgumentList.Arguments.Count is not 1 
            // ToDo
            // Implement filters
            // and not 2
            )
        {
            return false;
        }
        var arg = invoke.ArgumentList.Arguments[0];
        if (arg.Expression is not SimpleLambdaExpressionSyntax simpleLambda) return false;
        if (simpleLambda.ExpressionBody is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return false;
        ExpressionSyntax? filter = null;
        if (invoke.ArgumentList.Arguments.Count == 2
            && invoke.ArgumentList.Arguments[1].Expression is { } arg2Syntax)
        {
            filter = arg2Syntax;
        }
        var name = memberAccessExpressionSyntax.Name.ToString();
        catalog.ToOmit[name] = new Omission(name, filter);
        return true;
    }
}