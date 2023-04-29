using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Streams;

public interface ICreateStream
{
    Stream GetStreamFor(IFileSystem fileSystem, FilePath path);
}

public class NormalFileStreamCreator : ICreateStream
{
    public static readonly NormalFileStreamCreator Instance = new();
    
    public Stream GetStreamFor(IFileSystem fileSystem, FilePath path)
    {
        return fileSystem.File.Create(path);
    }
}