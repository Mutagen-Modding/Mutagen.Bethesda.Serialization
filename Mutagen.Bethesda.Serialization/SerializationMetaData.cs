using System.IO.Abstractions;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization;

public class SerializationMetaData
{
    public GameRelease Release { get; }
    public IWorkDropoff WorkDropoff { get; }
    public IFileSystem FileSystem { get; }
    public ICreateStream StreamCreator { get; }

    public SerializationMetaData(
        GameRelease release, 
        IWorkDropoff? workDropoff,
        IFileSystem? fileSystem, 
        ICreateStream? streamCreator)
    {
        Release = release;
        WorkDropoff = workDropoff.GetOrFallback(() => InlineWorkDropoff.Instance);
        StreamCreator = streamCreator.GetOrFallback(() => NormalFileStreamCreator.Instance);
        FileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
    }

    // For testing
    public SerializationMetaData(GameRelease release)
    {
        Release = release;
        WorkDropoff = null!;
        StreamCreator = null!;
        FileSystem = null!;
    }
}