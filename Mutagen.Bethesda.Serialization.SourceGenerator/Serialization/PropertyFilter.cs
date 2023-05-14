using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class PropertyFilter
{
    private readonly EnumFieldGenerator _enumFieldGenerator;

    public PropertyFilter(EnumFieldGenerator enumFieldGenerator)
    {
        _enumFieldGenerator = enumFieldGenerator;
    }

    public bool Skip(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        IPropertySymbol propertySymbol, 
        ISerializationForFieldGenerator? generator)
    {
        if (propertySymbol.IsIndexer) return true;
        switch (propertySymbol.Name)
        {
            case "StaticRegistration":
            case "Registration":
                return propertySymbol.Type.Name == "ILoquiRegistration";
            case "BinaryWriteTranslator":
                return true;
            case "FormVersion":
                return obj.Getter?.Name == "IMajorRecordGetter";
            default:
                break;
        }

        if (propertySymbol.Name.EndsWith("Release")
            && _enumFieldGenerator.Applicable(
                obj,
                compilation.Customization.Overall,
                propertySymbol.Type,
                propertySymbol.Name))
        {
            return true;
        }
        
        if (generator != null && !generator.ShouldGenerate(propertySymbol)) return true;

        return false;
    }
}