using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class HasFormVersionRetriever
{
    private readonly NamespaceSuffixRetriever _namespaceSuffixRetriever;

    public HasFormVersionRetriever(NamespaceSuffixRetriever namespaceSuffixRetriever)
    {
        _namespaceSuffixRetriever = namespaceSuffixRetriever;
    }

    public bool HasFormVersion(ITypeSymbol typeSymbol)
    {
        return _namespaceSuffixRetriever.TryGet(typeSymbol) != "Oblivion";
    }
}