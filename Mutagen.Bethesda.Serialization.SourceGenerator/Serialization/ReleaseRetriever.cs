using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class ReleaseRetriever
{
    public string GetReleaseName(ITypeSymbol typeSymbol)
    {
        var str = typeSymbol.ContainingNamespace.ToString();
        if (str.StartsWith("Mutagen.Bethesda."))
        {
            return str.Substring("Mutagen.Bethesda.".Length);
        }

        throw new NotImplementedException();
    }
}