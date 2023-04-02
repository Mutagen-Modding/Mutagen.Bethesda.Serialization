using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public class CustomizationInterpreter
{
    public IncrementalValueProvider<CustomizationSpecifications> Interpret(
        IncrementalValueProvider<CustomizeMethodDeclaration?> customizationDeclarations)
    {
        return customizationDeclarations
            .Select(InterpretDriver);
    }
    
    private CustomizationSpecifications InterpretDriver(
        CustomizeMethodDeclaration? decl,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (decl == null) return new CustomizationSpecifications(null);
        var driver = new CustomizationSpecifications(decl.ContainingClass);
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
        CustomizationSpecifications specifications,
        InvocationExpressionSyntax invoke)
    {
        if (invoke.Expression is not MemberAccessExpressionSyntax member) return false;
        switch (member.Name.ToString())
        {
            case "FolderPerRecord" when invoke.ArgumentList.Arguments.Count is 0:
                return HandleFolderPerRecord(specifications, invoke, member);
            default:
                return false;
        }
    }

    private bool HandleFolderPerRecord(
        CustomizationSpecifications specifications,
        InvocationExpressionSyntax invoke,
        MemberAccessExpressionSyntax memberAccess)
    {
        specifications.FolderPerRecord = true;
        return true;
    }
}