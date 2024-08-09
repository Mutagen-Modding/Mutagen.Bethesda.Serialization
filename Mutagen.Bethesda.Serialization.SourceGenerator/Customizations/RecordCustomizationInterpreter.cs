using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public class RecordCustomizationInterpreter
{
    public IncrementalValuesProvider<RecordCustomizationSpecifications> Interpret(
        IncrementalValuesProvider<CustomizeRecordMethodDeclaration> customizationRecordDeclarations,
        IncrementalValueProvider<LoquiMapping> mappings)
    {
        return customizationRecordDeclarations
            .Combine(mappings)
            .Select((x, c) => InterpretDriver(x.Left, x.Right, c))
            .NotNull();
    }

    private RecordCustomizationSpecifications? InterpretDriver(
        CustomizeRecordMethodDeclaration decl,
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
        var driver = new RecordCustomizationSpecifications(decl.ContainingClass, typeSet);
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
        RecordCustomizationSpecifications specifications,
        InvocationExpressionSyntax invoke)
    {
        if (invoke.Expression is not MemberAccessExpressionSyntax member) return false;
        switch (member.Name.ToString())
        {
            case "Omit" when invoke.ArgumentList.Arguments.Count is 1 or 2:
                return HandleOmit(specifications, invoke, member);
            default:
                return false;
        }
    }

    private bool HandleOmit(
        RecordCustomizationSpecifications specifications,
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
        specifications.ToOmit ??= new();
        specifications.ToOmit[name] = new Omission(name, filter);
        return true;
    }
}