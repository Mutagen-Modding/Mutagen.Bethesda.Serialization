using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class ObjectTypeTester
{
    public bool IsModObject(ITypeSymbol typeSymbol)
    {
        return typeSymbol.Name.Contains("Mod") 
               && typeSymbol.AllInterfaces
                   .Any(x => x.ToString().Contains("IModGetter"));
    }
    
    public bool IsMajorRecordObject(ITypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces
            .Any(x => x.ToString().Contains("IMajorRecordGetter"));
    }
}