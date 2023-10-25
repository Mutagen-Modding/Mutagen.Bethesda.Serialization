using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization;

public class SerializationMetaData
{
    public GameRelease Release { get; }
    public GameConstants Constants { get; }
    public IWorkDropoff WorkDropoff { get; }
    public IFileSystem FileSystem { get; }
    public ICreateStream StreamCreator { get; }
    public CancellationToken Cancel { get; }

    public SerializationMetaData(
        GameRelease release, 
        IWorkDropoff? workDropoff,
        IFileSystem? fileSystem, 
        ICreateStream? streamCreator,
        CancellationToken cancel)
    {
        Release = release;
        Constants = GameConstants.Get(release);
        Cancel = cancel;
        WorkDropoff = workDropoff.GetOrFallback(() => InlineWorkDropoff.Instance);
        StreamCreator = streamCreator.GetOrFallback(() => NormalFileStreamCreator.Instance);
        FileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
    }

    // For testing
    public SerializationMetaData(GameRelease release)
    {
        Release = release;
        WorkDropoff = null!;
        Constants = null!;
        StreamCreator = null!;
        FileSystem = null!;
        Cancel = CancellationToken.None;
    }

    public SerializationMetaData(CancellationToken cancel)
    {
        Release = default;
        Constants = null!;
        WorkDropoff = null!;
        StreamCreator = null!;
        FileSystem = null!;
        Cancel = cancel;
    }
}