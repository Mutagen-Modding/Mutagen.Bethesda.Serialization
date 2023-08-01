using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class NamespaceSuffixRetriever
{
    public string? TryGet(ITypeSymbol typeSymbol)
    {
        var str = typeSymbol.ContainingNamespace.ToString();
        if (str.StartsWith("Mutagen.Bethesda."))
        {
            return str.Substring("Mutagen.Bethesda.".Length);
        }

        return null;
    }
    
    public string Get(ITypeSymbol typeSymbol)
    {
        return TryGet(typeSymbol) ?? throw new ArgumentException($"TypeSymbol had no Mutagen namespace: {typeSymbol}");
    }
}