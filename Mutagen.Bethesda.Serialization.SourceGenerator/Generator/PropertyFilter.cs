using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class PropertyFilter
{
    private readonly EnumFieldGenerator _enumFieldGenerator;

    public PropertyFilter(EnumFieldGenerator enumFieldGenerator)
    {
        _enumFieldGenerator = enumFieldGenerator;
    }
    
    public bool Skip(IPropertySymbol propertySymbol)
    {
        if (propertySymbol.IsIndexer) return true;
        if (propertySymbol.Type.Name == "ILoquiRegistration")
        {
            switch (propertySymbol.Name)
            {
                case "StaticRegistration":
                case "Registration":
                    return true;
                default:
                    return false;
            }
        }

        if (propertySymbol.Name.EndsWith("Release")
            && _enumFieldGenerator.Applicable(propertySymbol.Type))
        {
            return true;
        }
        return false;
    }
}