namespace Mutagen.Bethesda.Serialization.Customizations;

public interface ICustomize<TObject>
{
    void CustomizeFor(ICustomizationBuilder<TObject> builder);
}