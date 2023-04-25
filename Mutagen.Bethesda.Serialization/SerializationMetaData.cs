using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization;

public class SerializationMetaData
{
    public GameRelease Release { get; }
    public IWorkDropoff WorkDropoff { get; }

    public SerializationMetaData(GameRelease release, IWorkDropoff workDropoff)
    {
        Release = release;
        WorkDropoff = workDropoff;
    }
}