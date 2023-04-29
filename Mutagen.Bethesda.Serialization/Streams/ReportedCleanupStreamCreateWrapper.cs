using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Streams;

public class ReportedCleanupStreamCreateWrapper : ICreateStream, IDisposable
{
    private readonly IFileSystem _fileSystem;
    private readonly DirectoryPath _baseDir;
    private readonly ICreateStream _wrapped;
    private readonly HashSet<string> _writtenPaths = new();

    public ReportedCleanupStreamCreateWrapper(
        IFileSystem fileSystem, 
        DirectoryPath baseDir,
        ICreateStream wrapped)
    {
        _fileSystem = fileSystem;
        _baseDir = baseDir;
        _wrapped = wrapped;
    }

    public void MarkPathWrittenTo(FilePath path)
    {
        lock (_writtenPaths)
        {
            _writtenPaths.Add(path.Path);
        }
    }
    
    public Stream GetStreamFor(IFileSystem fileSystem, FilePath path)
    {
        MarkPathWrittenTo(path);
        return _wrapped.GetStreamFor(fileSystem, path);
    }

    public void Dispose()
    {
        CleanDir(_baseDir);
    }

    private bool CleanDir(string dir)
    {
        bool canDelete = true;
        foreach (var subDir in _fileSystem.Directory.GetDirectories(dir))
        {
            if (CleanDir(subDir))
            {
                _fileSystem.Directory.Delete(subDir);
            }
            else
            {
                canDelete = false;
            }
        }

        foreach (var file in _fileSystem.Directory.GetFiles(dir))
        {
            if (_writtenPaths.Contains(file))
            {
                canDelete = false;
            }
            else
            {
                _fileSystem.File.Delete(file);
            }
        }

        return canDelete;
    }
}