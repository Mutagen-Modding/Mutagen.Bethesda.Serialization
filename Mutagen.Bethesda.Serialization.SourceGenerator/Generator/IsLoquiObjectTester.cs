using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class IsLoquiObjectTester
{
    public bool IsLoqui(ITypeSymbol typeSymbol)
    {
        return typeSymbol.Interfaces.Any(x => x.Name == "ILoquiObject");
    }
}