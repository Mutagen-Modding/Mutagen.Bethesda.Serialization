using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class IsGroupTester
{
    public bool IsGroup(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null) return false;
        return typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.IsGenericType
            && namedTypeSymbol.TypeArguments.Length == 1
            && (typeSymbol.AllInterfaces.Any(x => x.Name == "IGroupGetter") || typeSymbol.Name.Contains("ListGroup"));
    }
}