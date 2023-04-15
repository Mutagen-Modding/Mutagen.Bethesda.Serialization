using Mutagen.Bethesda.Serialization.Customizations;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

public class Customization : ICustomize
{
    public void Customize(ICustomizationBuilder builder)
    {
        builder.FilePerRecord()
            .EnforceRecordOrder();
    }
}