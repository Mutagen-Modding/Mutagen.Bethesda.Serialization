using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class IsListTester
{
    private static readonly HashSet<string> _listStrings = new()
    {
        "List",
        "IReadOnlyList",
        "IList",
        "ExtendedList",
        "IExtendedList",
    };

    public IReadOnlyCollection<string> ListNameStrings => _listStrings;
    
    public bool Applicable(
        ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            if (arr.ElementType.Name == "Byte") return false;
            return true;
        }
        else
        {
            typeSymbol = typeSymbol.PeelNullable();
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
            var typeMembers = namedTypeSymbol.TypeArguments;
            if (typeMembers.Length != 1) return false;
            return _listStrings.Contains(typeSymbol.Name);
        }
    }
}