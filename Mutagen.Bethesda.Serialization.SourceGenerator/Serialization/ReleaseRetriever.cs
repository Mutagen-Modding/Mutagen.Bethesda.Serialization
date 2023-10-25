using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class ReleaseRetriever
{
    private readonly NamespaceSuffixRetriever _namespaceSuffixRetriever;

    public ReleaseRetriever(NamespaceSuffixRetriever namespaceSuffixRetriever)
    {
        _namespaceSuffixRetriever = namespaceSuffixRetriever;
    }
    
    public string GetReleaseName(ITypeSymbol typeSymbol)
    {
        return _namespaceSuffixRetriever.Get(typeSymbol);
    }

    public bool HasRelease(ITypeSymbol typeSymbol)
    {
        // ToDo
        // Upgrade to something with reflection perhaps
        return _namespaceSuffixRetriever.TryGet(typeSymbol) switch
        {
            "Oblivion" => false,
            _ => true
        };
    }
}