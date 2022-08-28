using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class PropertyFilter
{
    public bool Skip(IPropertySymbol propertySymbol)
    {
        if (propertySymbol.Type.Name != "ILoquiRegistration") return false;
        switch (propertySymbol.Name)
        {
            case "StaticRegistration":
            case "Registration":
                return true;
            default:
                return false;
        }
    }
}