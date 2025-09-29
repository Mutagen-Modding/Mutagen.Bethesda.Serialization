using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public static class FieldSortingHelper
{
    public static bool ShouldApplyContainerSorting(CompilationUnit compilation, string? fieldName)
    {
        if (fieldName == null) return false;
        return compilation.Customization.TargetRecordSpecs?.ContainerSortFields?.Any(x => x.ListFieldName == fieldName) ?? false;
    }

    public static void GenerateContainerSortedListAccess(
        CompilationUnit compilation,
        string? fieldName,
        string sourceAccessor,
        string resultVariableName,
        StructuredStringBuilder sb)
    {
        var containerSortFields = compilation.Customization.TargetRecordSpecs?.ContainerSortFields?
            .Where(x => x.ListFieldName == fieldName)
            .OrderBy(x => x.Priority)
            .ToList();

        if (containerSortFields == null || !containerSortFields.Any())
        {
            sb.AppendLine($"var {resultVariableName} = {sourceAccessor};");
            return;
        }

        sb.AppendLine($"var {resultVariableName} = {sourceAccessor}");

        for (int i = 0; i < containerSortFields.Count; i++)
        {
            var sortField = containerSortFields[i];
            var method = i == 0 ? "OrderBy" : "ThenBy";
            sb.AppendLine($"    .{method}(x => x.{sortField.ItemFieldName})");
        }

        sb.AppendLine("    .ToList();");
    }
}