using Noggog;

namespace Mutagen.Bethesda.Serialization.Exceptions;

public class FilePathedException : Exception
{
    public FilePath Path { get; }

    public FilePathedException(Exception wrappedException, FilePath path)
        : base($"Exception occurred while processing related to a filepath: {path}", wrappedException)
    {
        Path = path;
    }
}