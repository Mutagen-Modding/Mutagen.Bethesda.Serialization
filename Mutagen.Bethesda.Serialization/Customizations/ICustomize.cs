namespace Mutagen.Bethesda.Serialization.Customizations;

public interface ICustomize
{
    void Customize(ICustomizationBuilder builder);
}

public interface ICustomize<TObject>
{
    void CustomizeFor(ICustomizationBuilder<TObject> builder);
}