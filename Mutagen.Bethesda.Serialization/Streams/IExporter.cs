using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Streams;

public interface IExporter
{
    Task<bool> WriteOut(
        Stream stream,
        IFileSystem fileSystem, 
        FilePath path,
        CancellationToken cancel);
}