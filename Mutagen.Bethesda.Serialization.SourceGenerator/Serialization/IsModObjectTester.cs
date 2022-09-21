using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class IsModObjectTester
{
    public bool IsModObject(ITypeSymbol typeSymbol)
    {
        return typeSymbol.Name.Contains("Mod") 
               && typeSymbol.AllInterfaces
                   .Any(x => x.ToString().Contains("IModGetter"));
    }
}