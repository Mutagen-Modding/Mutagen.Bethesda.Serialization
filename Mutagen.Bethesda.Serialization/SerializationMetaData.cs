using System.IO.Abstractions;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization;

public class SerializationMetaData
{
    public GameRelease Release { get; }
    public IWorkDropoff WorkDropoff { get; }
    public IFileSystem FileSystem { get; }

    public SerializationMetaData(GameRelease release, IWorkDropoff workDropoff, IFileSystem? fileSystem)
    {
        Release = release;
        WorkDropoff = workDropoff;
        FileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
    }
}