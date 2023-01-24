using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public static class Utility
{
    public static T PeelNullable<T>(this T typeSymbol)
        where T : class, ITypeSymbol
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeArguments.Length == 1
            && namedTypeSymbol.Name == "Nullable"
            && namedTypeSymbol.TypeArguments[0] is T tItem)
        {
            return tItem;
        }

        return typeSymbol;
    }
    
    public static bool IsNullable(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeArguments.Length == 1
            && namedTypeSymbol.Name == "Nullable")
        {
            return true;
        }

        if (typeSymbol.ToString().EndsWith("?"))
        {
            return true;
        }

        return false;
    }
    
    public static string NullChar(this ITypeSymbol typeSymbol)
    {
        return NullChar(typeSymbol.IsNullable());
    }
    
    public static string NullChar(bool isNullable)
    {
        return isNullable ? "?" : string.Empty;
    }

    public delegate void WriteReadCall(
        StructuredStringBuilder sb,
        ITypeSymbol? field,
        string kernelAccessor,
        string readerAccessor,
        string setAccessor);
    
    public static void WrapStripNull(
        ITypeSymbol? field,
        string? fieldName,
        string fieldAccessor,
        string readerAccessor,
        string kernelAccessor,
        bool insideCollection,
        StructuredStringBuilder sb,
        WriteReadCall call)
    {
        if (field == null || fieldName == null)
        {
            call(sb, null, kernelAccessor, readerAccessor, fieldAccessor);
            return;
        }
        var isNullable = field.IsNullable();
        field = field.PeelNullable();
        fieldAccessor = $"{fieldAccessor}{(insideCollection ? null : " = ")}";
        
        if (isNullable)
        {
            call(sb, field, kernelAccessor, readerAccessor, fieldAccessor);
        }
        else
        {
            using (var strip = sb.Call($"{fieldAccessor}SerializationHelper.StripNull", linePerArgument: false))
            {
                strip.Add((subSb) =>
                {
                    call(subSb, field, kernelAccessor, readerAccessor, string.Empty);
                });
                strip.Add($"name: \"{fieldName}\"");
            }
        }
    }
}