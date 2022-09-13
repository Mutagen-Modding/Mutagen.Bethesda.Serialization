using System.IO.Abstractions;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.Tests;

[UsesVerify]
public abstract class ASerializationTests<TReaderKernel, TReaderObject, TWriterKernel, TWriterObject>
    where TReaderKernel : ISerializationReaderKernel<TReaderObject>, new()
    where TWriterKernel : ISerializationWriterKernel<TWriterObject>, new()
{
    [Theory]
    [DefaultAutoData]
    public async Task EmptySkyrimModExport()
    {
        var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        MutagenJsonConverter.Instance.Serialize(mod, stream);
        stream.Position = 0;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEnd();
        await Verifier.Verify(str);
    }
    
    [Theory]
    [DefaultAutoData]
    public void EmptySkyrimModPassthrough(
        IFileSystem fileSystem)
    {
        var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        MutagenJsonConverter.Instance.Serialize(mod, stream);
        stream.Position = 0;
        var mod2 = MutagenJsonConverter.Instance.Deserialize(stream);
        CheckEquality(fileSystem, mod, mod2);
    }

    private void CheckEquality(
        IFileSystem fileSystem,
        ISkyrimModGetter mod1,
        ISkyrimModGetter mod2)
    {
        var mod1Path = $"C:/{mod1.ModKey.FileName}";
        var mod2Path = $"C:/{mod2.ModKey.FileName}";
        using (var fs = fileSystem.FileStream.Create(mod1Path, FileMode.Create))
        {
            mod1.WriteToBinaryParallel(fs);
        }
        using (var fs = fileSystem.FileStream.Create(mod2Path, FileMode.Create))
        {
            mod2.WriteToBinaryParallel(fs);
        }
        AssertFilesEqual(
            fileSystem.FileStream.Create(mod1Path, FileMode.Open),
            mod2Path);
    }

    public static void AssertFilesEqual(
        Stream stream,
        string path2,
        ushort amountToReport = 5)
    {
        using var reader2 = new BinaryReadStream(path2);
        Stream compareStream = new ComparisonStream(
            stream,
            reader2);

        var errs = GetDifferences(compareStream)
            .First(amountToReport)
            .ToArray();
        if (errs.Length > 0)
        {
            throw new DidNotMatchException(path2, errs, stream);
        }
        if (stream.Position != stream.Length)
        {
            throw new MoreDataException(path2, stream.Position);
        }
        if (reader2.Position != reader2.Length)
        {
            throw new UnexpectedlyMoreData(path2, reader2.Position);
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