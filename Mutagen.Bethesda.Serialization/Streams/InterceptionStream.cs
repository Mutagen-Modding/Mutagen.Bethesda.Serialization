using System.IO.Abstractions;
using Noggog;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Streams;

public class InterceptionStream : Stream
{
    private readonly IExporter _exporter;
    public IFileSystem FileSystem { get; }
    public string Path { get; }
    private readonly MemoryTributary _tributary = new();

    public InterceptionStream(
        IFileSystem fileSystem, 
        string path,
        IExporter exporter)
    {
        _exporter = exporter;
        FileSystem = fileSystem;
        Path = path;
    }
    
    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _tributary.Write(buffer, offset, count);
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => _tributary.Length;

    public override long Position
    {
        get => _tributary.Position;
        set => _tributary.Position = value;
    }

    protected override void Dispose(bool disposing)
    {
        _tributary.Position = 0;
        _exporter.WriteOut(_tributary, FileSystem, Path, CancellationToken.None);
        base.Dispose(disposing);
        _tributary.Dispose();
    }
}

public class InterceptionStreamCreator : ICreateStream
{
    private readonly IExporter _exporter;

    public InterceptionStreamCreator(IExporter exporter)
    {
        _exporter = exporter;
    }
    
    public Stream GetStreamFor(IFileSystem fileSystem, FilePath path, bool write)
    {
        if (write)
        {
            return new InterceptionStream(fileSystem, path, _exporter);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}