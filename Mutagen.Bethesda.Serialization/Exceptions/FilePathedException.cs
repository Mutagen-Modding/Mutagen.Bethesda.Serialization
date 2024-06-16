using Noggog;

namespace Mutagen.Bethesda.Serialization.Exceptions;

public class FilePathedException : Exception
{
    public FilePath Path { get; }

    private FilePathedException(Exception wrappedException, FilePath path)
        : base($"Exception occurred while processing related to a filepath: {path}", wrappedException)
    {
        Path = path;
    }

    public static FilePathedException Enrich(Exception ex, FilePath path)
    {
        if (ex is FilePathedException rhs) return rhs;
        return new FilePathedException(ex, path);
    }
}