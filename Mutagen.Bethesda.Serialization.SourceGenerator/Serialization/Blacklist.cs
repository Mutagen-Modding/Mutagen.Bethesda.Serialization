namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class Blacklist
{
    public bool ShouldSkip(LoquiTypeSet typeSet)
    {
        return typeSet.Getter?.Name.Contains("HaveVirtual") ?? false;
    }
}