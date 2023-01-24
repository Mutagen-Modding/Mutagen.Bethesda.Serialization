using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Testing.Exceptions;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Testing;

public static class PassthroughTest
{
    public static void PassThrough<TModGetter>(
        IFileSystem fileSystem,
        DirectoryPath dir,
        TModGetter mod,
        Action<TModGetter, Stream> serialize,
        Func<Stream, TModGetter> deserialize)
        where TModGetter : IModGetter
    {
        var filePath = Path.Combine(dir, "Serialized");
        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        using (var stream = fileSystem.File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            serialize(mod, stream);
        }

        TModGetter mod2;
        using (var stream = fileSystem.File.OpenRead(filePath))
        {
            mod2 = deserialize(stream);
        }
        CheckEquality(fileSystem, dir, mod, mod2);
    }

    public static void CheckEquality<TMod>(
        IFileSystem fileSystem,
        DirectoryPath dir,
        TMod mod1,
        TMod mod2)
        where TMod : IModGetter
    {
        var mod1OutFile = Path.Combine(dir, "Input");
        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(mod1OutFile));
        var mod2OutFile = Path.Combine(dir, "Output");
        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(mod2OutFile));
        using (var fs = fileSystem.FileStream.Create(mod1OutFile, FileMode.Create))
        {
            mod1.WriteToBinaryParallel(fs);
        }
        using (var fs = fileSystem.FileStream.Create(mod2OutFile, FileMode.Create))
        {
            mod2.WriteToBinaryParallel(fs);
        }

        using var stream1 = fileSystem.FileStream.Create(mod1OutFile, FileMode.Open);
        using var stream2 = fileSystem.FileStream.Create(mod2OutFile, FileMode.Open);

        AssertFilesEqual(
            stream1,
            mod2OutFile,
            stream2);
    }

    public static void AssertFilesEqual(
        Stream stream,
        FilePath stream2Path,
        Stream stream2,
        ushort amountToReport = 5)
    {
        using var reader2 = new BinaryReadStream(stream2);
        using Stream compareStream = new ComparisonStream(
            stream,
            reader2);

        var errs = GetDifferences(compareStream)
            .First(amountToReport)
            .ToArray();
        if (errs.Length > 0)
        {
            throw new DidNotMatchException(stream2Path, errs, stream);
        }
        if (stream.Position != stream.Length)
        {
            throw new MoreDataException(stream2Path, stream.Position);
        }
        if (reader2.Position != reader2.Length)
        {
            throw new UnexpectedlyMoreData(stream2Path, reader2.Position);
        }
    }

    public static IEnumerable<RangeInt64> GetDifferences(Stream reader)
    {
        byte[] buf = new byte[4096];
        bool inRange = false;
        long startRange = 0;
        var len = reader.Length;
        long pos = 0;
        while (pos < len)
        {
            var read = reader.Read(buf, 0, buf.Length);
            for (int i = 0; i < read; i++)
            {
                if (buf[i] != 0)
                {
                    if (!inRange)
                    {
                        startRange = pos + i;
                        inRange = true;
                    }
                }
                else
                {
                    if (inRange)
                    {
                        var sourceRange = new RangeInt64(startRange, pos + i);
                        yield return sourceRange;
                        inRange = false;
                    }
                }
            }
            pos += read;
        }
    }
}