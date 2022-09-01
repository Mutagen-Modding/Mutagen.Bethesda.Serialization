using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class IsGroupTester
{
    public bool IsGroup(ITypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith("Group")
               || typeSymbol.Name.EndsWith("GroupGetter");
    }
}