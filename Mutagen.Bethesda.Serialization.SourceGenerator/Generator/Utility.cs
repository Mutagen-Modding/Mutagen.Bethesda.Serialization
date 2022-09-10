using System.Collections;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public static class Utility
{
    public static T PeelNullable<T>(T typeSymbol)
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
    
    public static bool IsNullable(ITypeSymbol typeSymbol)
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
}