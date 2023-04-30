using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Streams;

public interface ICreateStream
{
    Stream GetStreamFor(IFileSystem fileSystem, FilePath path, bool write);
}

public class NormalFileStreamCreator : ICreateStream
{
    public static readonly NormalFileStreamCreator Instance = new();
    
    public Stream GetStreamFor(IFileSystem fileSystem, FilePath path, bool write)
    {
        return fileSystem.File.Open(path, write ? FileMode.Create : FileMode.Open, write ? FileAccess.Write : FileAccess.Read, FileShare.Read);
    }
}