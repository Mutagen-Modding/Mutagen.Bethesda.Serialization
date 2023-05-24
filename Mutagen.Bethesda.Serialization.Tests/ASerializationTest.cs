using System.IO.Abstractions;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public abstract class ASerializationTest<TWriterKernal, TWriterObj, TReaderKernel, TReaderObj>
    where TWriterKernal : ISerializationWriterKernel<TWriterObj>, new()
    where TReaderKernel : ISerializationReaderKernel<TReaderObj>, new()
{
    protected StreamPackage GetStreamPackage(Stream stream) => new StreamPackage(stream, null);

    protected TReaderKernel ReaderKernel { get; } = new TReaderKernel();
    protected MutagenSerializationWriterKernel<TWriterKernal, TWriterObj> WriterKernel { get; } = MutagenSerializationWriterKernel<TWriterKernal, TWriterObj>.Instance;

    public IDisposable GetReaderObj(
        IFileSystem fileSystem,
        FilePath path,
        out TReaderObj readerObj)
    {
        var fs = fileSystem.File.OpenRead(path);
        readerObj = ReaderKernel.GetNewObject(new StreamPackage(fs, path.Directory));
        return fs;
    }
    
    public IDisposable GetWriterObj(
        IFileSystem fileSystem,
        FilePath path,
        out TWriterObj writerObj)
    {
        var fs = fileSystem.File.OpenWrite(path);
        var sp = new StreamPackage(fs, path.Directory);
        var wo = WriterKernel.GetNewObject(sp);
        writerObj = wo;
        return Disposable.Create(() =>
        {
            WriterKernel.Finalize(sp, wo);
            fs.Dispose();
        });
    }
}