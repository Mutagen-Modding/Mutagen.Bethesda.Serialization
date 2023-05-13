using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

public static class Extensions
{
    public static ITypeSymbol? TryGetTypeSymbol(this ISymbol? symbol)
    {
        if (symbol is ILocalSymbol local) return local.Type;
        if (symbol is IParameterSymbol param) return param.Type;
        if (symbol is IMethodSymbol methodSymbol) return methodSymbol.ContainingType;
        return default;
    }

    public static IncrementalValuesProvider<T> NotNull<T>(this IncrementalValuesProvider<T?> vals)
    {
        return vals.Where(x => x != null)!;
    }

    public static bool IsGettable(this IPropertySymbol p)
    {
        if (p.GetMethod is null) return false;
        return p.DeclaredAccessibility is Accessibility.Public;
    }

    public static bool IsSettable(this IPropertySymbol p)
    {
        if (p.SetMethod is null) return false;
        return p.DeclaredAccessibility is Accessibility.Public;
    }
}