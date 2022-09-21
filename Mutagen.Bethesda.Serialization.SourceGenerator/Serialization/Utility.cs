using Microsoft.CodeAnalysis;

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
}