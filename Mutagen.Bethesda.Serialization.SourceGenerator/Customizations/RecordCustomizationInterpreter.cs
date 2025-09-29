using System.Collections.Generic;
using System.Linq;
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

        // Process invocations in source order to handle method chaining correctly
        var invocations = decl.MethodSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>()
            .OrderBy(x => x.SpanStart)
            .Reverse() // Process from innermost to outermost calls
            .ToList();

        foreach (var invoke in invocations)
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
            case "EmbedRecordsInSameFile" when invoke.ArgumentList.Arguments.Count is 1:
                return HandleEmbedRecordsInSameFile(specifications, invoke, member);
            case "SortList" when invoke.ArgumentList.Arguments.Count is 1:
                return HandleSortList(specifications, invoke, member);
            case "ByField" when invoke.ArgumentList.Arguments.Count is 1:
                return HandleByField(specifications, invoke, member);
            case "ThenByField" when invoke.ArgumentList.Arguments.Count is 1:
                return HandleContainerThenByField(specifications, invoke, member);
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
    
    private bool HandleEmbedRecordsInSameFile(
        RecordCustomizationSpecifications specifications,
        InvocationExpressionSyntax invoke,
        MemberAccessExpressionSyntax memberAccess)
    {
        var arg = invoke.ArgumentList.Arguments[0];
        if (arg.Expression is not SimpleLambdaExpressionSyntax simpleLambda) return false;
        if (simpleLambda.ExpressionBody is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return false;
        var name = memberAccessExpressionSyntax.Name.ToString();
        specifications.ToEmbedRecordsInSameFile ??= new();
        specifications.ToEmbedRecordsInSameFile.Add(name);
        return true;
    }


    private string GetFullMemberAccessPath(MemberAccessExpressionSyntax memberAccess)
    {
        var parts = new List<string>();
        var current = memberAccess;

        // Walk up the member access chain to collect all parts
        while (current != null)
        {
            parts.Add(current.Name.ToString());

            if (current.Expression is MemberAccessExpressionSyntax parentMember)
            {
                current = parentMember;
            }
            else
            {
                // We've reached the parameter (e.g., "x"), so we're done
                break;
            }
        }

        // Reverse the parts since we collected them in reverse order
        parts.Reverse();

        // Join with dots to create the full path (e.g., "Grid.X")
        return string.Join(".", parts);
    }

    private bool HandleSortList(
        RecordCustomizationSpecifications specifications,
        InvocationExpressionSyntax invoke,
        MemberAccessExpressionSyntax memberAccess)
    {
        var arg = invoke.ArgumentList.Arguments[0];
        if (arg.Expression is not SimpleLambdaExpressionSyntax simpleLambda) return false;
        if (simpleLambda.ExpressionBody is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return false;

        var listFieldName = GetFullMemberAccessPath(memberAccessExpressionSyntax);

        // Store the current list field name for use by subsequent ByField calls
        specifications.CurrentListField = listFieldName;
        return true;
    }

    private bool HandleByField(
        RecordCustomizationSpecifications specifications,
        InvocationExpressionSyntax invoke,
        MemberAccessExpressionSyntax memberAccess)
    {
        var arg = invoke.ArgumentList.Arguments[0];
        if (arg.Expression is not SimpleLambdaExpressionSyntax simpleLambda) return false;
        if (simpleLambda.ExpressionBody is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return false;

        var itemFieldName = GetFullMemberAccessPath(memberAccessExpressionSyntax);

        // Get the list field name from the preceding SortList call
        if (string.IsNullOrEmpty(specifications.CurrentListField)) return false;

        specifications.ContainerSortFields ??= new();

        var priority = 0; // First field always has priority 0
        specifications.ContainerSortFields.Add(new ContainerSortField(specifications.CurrentListField!, itemFieldName, priority));
        return true;
    }


    private bool HandleContainerThenByField(
        RecordCustomizationSpecifications specifications,
        InvocationExpressionSyntax invoke,
        MemberAccessExpressionSyntax memberAccess)
    {
        var arg = invoke.ArgumentList.Arguments[0];
        if (arg.Expression is not SimpleLambdaExpressionSyntax simpleLambda) return false;
        if (simpleLambda.ExpressionBody is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return false;

        var itemFieldName = GetFullMemberAccessPath(memberAccessExpressionSyntax);

        // Get the list field name from the current sorting context
        if (string.IsNullOrEmpty(specifications.CurrentListField)) return false;

        specifications.ContainerSortFields ??= new();

        var priority = specifications.ContainerSortFields.Count;
        specifications.ContainerSortFields.Add(new ContainerSortField(specifications.CurrentListField!, itemFieldName, priority));
        return true;
    }
}