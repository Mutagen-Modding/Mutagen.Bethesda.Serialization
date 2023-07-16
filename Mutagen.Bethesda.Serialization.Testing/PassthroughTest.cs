using System.Diagnostics;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Testing.Exceptions;
using Noggog;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Testing;

public static class PassthroughTest
{
    public static async Task PassThrough<TModGetter>(
        IFileSystem fileSystem,
        DirectoryPath dir,
        TModGetter mod,
        Func<TModGetter, DirectoryPath, ICreateStream, Task> serialize,
        Func<DirectoryPath, ModKey, ICreateStream, Task<TModGetter>> deserialize)
        where TModGetter : IModGetter
    {
        var streamCreator = NormalFileStreamCreator.Instance;
        var serializedFolder = Path.Combine(dir, "Serialized");
        fileSystem.Directory.CreateDirectory(serializedFolder);
        Stopwatch sw = new();
        sw.Start();
        
        using (var streamCreateWrapper = new ReportedCleanupStreamCreateWrapper(fileSystem, dir, streamCreator))
        {
            await serialize(mod, serializedFolder, streamCreateWrapper);
        }
        sw.Stop();
        Console.WriteLine($"Serialization took {sw.ElapsedMilliseconds / 1000d}s");

        sw.Restart();
        TModGetter mod2 = await deserialize(serializedFolder, mod.ModKey, streamCreator);
        Console.WriteLine($"Deserialization took {sw.ElapsedMilliseconds / 1000d}s");
        sw.Stop();
        
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
        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(mod1OutFile)!);
        var mod2OutFile = Path.Combine(dir, "Output");
        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(mod2OutFile)!);
        var options = new Plugins.Binary.Parameters.BinaryWriteParameters();
        using (var fs = fileSystem.FileStream.New(mod1OutFile, FileMode.Create, FileAccess.ReadWrite))
        {
            mod1.WriteToBinary(fs, options);
        }
        using (var fs = fileSystem.FileStream.New(mod2OutFile, FileMode.Create, FileAccess.ReadWrite))
        {
            mod2.WriteToBinary(fs, options);
        }

        using var stream1 = fileSystem.FileStream.New(mod1OutFile, FileMode.Open);
        using var stream2 = fileSystem.FileStream.New(mod2OutFile, FileMode.Open);

        AssertFilesEqual(
            stream1,
            mod2OutFile,
            stream2);
        Console.WriteLine($"Files equal");
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