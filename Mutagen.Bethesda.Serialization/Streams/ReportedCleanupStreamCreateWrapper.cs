using System.IO.Abstractions;
using Noggog;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Streams;

public class ReportedCleanupStreamCreateWrapper : ICreateStream, IDisposable
{
    private readonly IFileSystem _fileSystem;
    private readonly DirectoryPath _baseDir;
    private readonly ICreateStream _wrapped;
    private readonly HashSet<FilePath> _writtenPaths = new();

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
    
    public Stream GetStreamFor(IFileSystem fileSystem, FilePath path, bool write)
    {
        MarkPathWrittenTo(path);
        path.Directory?.Create(fileSystem);
        return _wrapped.GetStreamFor(fileSystem, path, write);
    }

    public void Dispose()
    {
        lock (_writtenPaths)
        {
            CleanDir(_baseDir);
        }
    }

    private bool CleanDir(DirectoryPath dir)
    {
        bool canDelete = true;
        foreach (var subDir in dir.EnumerateDirectories(includeSelf: false, recursive: false, fileSystem: _fileSystem))
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

        foreach (var file in dir.EnumerateFiles(recursive: false, fileSystem: _fileSystem))
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