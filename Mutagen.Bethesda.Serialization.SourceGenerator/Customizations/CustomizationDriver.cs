using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public class CustomizationDriver
{
    public void WrapOmission(
        CustomizationCatalog? catalog,
        StructuredStringBuilder sb, 
        PropertyMetadata property,
        Action toDo)
    {
        if (catalog == null)
        {
            toDo();
            return;
        }

        var name = property.Property.Name;

        if (!catalog.ToOmit.TryGetValue(name, out var omission))
        {
            toDo();
            return;
        }
        
        // ToDo
        // Add filter lambdas
    }
}