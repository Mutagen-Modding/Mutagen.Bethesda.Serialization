using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class PropertyFilter
{
    public bool Skip(IPropertySymbol propertySymbol)
    {
        switch (propertySymbol.Name)
        {
            case "StaticRegistration":
                return true;
            default:
                return false;
        }
    }
}