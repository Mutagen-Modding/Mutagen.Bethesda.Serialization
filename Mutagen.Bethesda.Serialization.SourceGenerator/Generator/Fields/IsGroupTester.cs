using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class IsGroupTester
{
    public bool IsGroup(ITypeSymbol typeSymbol)
    {
        return typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.IsGenericType
            && namedTypeSymbol.TypeArguments.Length == 1
            && typeSymbol.Interfaces.Any(x => x.Name == "IGroupGetter");
    }
}