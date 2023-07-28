# Overview
This is a library dedicated to providing the serialization code for converting Bethesda files to other formats.
Right now Json and Yaml are supporeted, but it can be expanded to target other formats.

It's driven by using C# Source Generators to inspect the interfaces of [Mutagen Objects](https://github.com/Mutagen-Modding/Mutagen) and provide the necessary code to convert those objects to and from the desired formats.

Typical entry point API:
```cs
// Mutagen code to read in a SkyrimMod from disk.
using var modGetter = SkyrimMod.CreateFromBinaryOverlay(modPath, SkyrimRelease.SkyrimSE);

// Call into the Mutagen.Bethesda.Serialization system to Yaml
await MutagenYamlConverter.Instance.Serialize(mod, "SomePath/Output.yaml");
```

# Customization
Source generators create code on-demand which lives in the end-user's project, rather than this library.   Because of this, we can offer customization that are driven by your needs that actually changes the generated code to fulfill the customization, rather than checking/adjusting at runtime.
- Exporting a file per record, rather than a file per mod
- Name swaps
- Skipping fields
- Comments
- More to be added as desired

These customizations are driven by defining a Customization object, which contains all the instructions desired:

Here is an example of a customization file with some instructions that you could add:
```cs
public class CustomizeOverall : ICustomize
{
    public void Customize(ICustomizationBuilder builder)
    {
        builder
            .FilePerRecord()
            .EnforceRecordOrder();
    }
}
public class CustomizeNpc : ICustomize<INpcGetter>
{
    public void CustomizeFor(ICustomizationBuilder<INpcGetter> builder)
    {
        builder.Omit(n => n.Name);
    }
}
```
This would adjust your generated code to:
- Export a mod as a folder of record files
- Enforce those files to sort to be the same order as they appeared in the original mod (changes the file name to include index number)
- And for Npcs, specifically, omit the `Name`.  (just for proof of concept)

This pattern can be used to provide many functionalities users might want, while generating the customized, optimized, and compiled code to fulfill it.

# Spriggit
[Spriggit](https://github.com/Mutagen-Modding/Spriggit) is a downstream project that was developed alongside this library, with the goal of providing a more end-user experience, including a UI, for converting `.esp` files to `.json` or `.yaml`, mainly for the purpose of storing them in Git Repositories.
