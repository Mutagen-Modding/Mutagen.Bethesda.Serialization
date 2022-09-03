using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class IsLoquiObjectTester
{
    public bool IsLoqui(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.Name.StartsWith("ILoquiObject")) return false;
        return typeSymbol.AllInterfaces.Any(x => x.Name.StartsWith("ILoquiObject")) 
               || (typeSymbol.BaseType?.Name?.StartsWith("ILoquiObject") ?? false);
    }
    
    public bool IsLoqui(Type type)
    {
        return type.Name != "Object";
    }
}