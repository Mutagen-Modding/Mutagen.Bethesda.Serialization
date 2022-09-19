namespace Mutagen.Bethesda.Serialization;

public class SerializationMetaData
{
    public GameRelease Release { get; }
    
    public SerializationMetaData(GameRelease release)
    {
        Release = release;
    }
}