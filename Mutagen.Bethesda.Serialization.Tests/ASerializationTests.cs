using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.Tests;

[UsesVerify]
public abstract class ASerializationTests
{
    public abstract void Serialize(SkyrimMod mod, Stream stream);
    public abstract ISkyrimModGetter Deserialize(Stream stream);
    public ModKey OriginalModKey => ModKey.FromFileName("InputMod.esp");
    public ModKey OutputModKey => ModKey.FromFileName("TestMod.esp");
    public FilePath OriginalModBinaryPath => $"C:/Binary/{OriginalModKey.FileName}";
    public FilePath OutputModBinaryPath => $"C:/Binary/{OutputModKey.FileName}";
    public FilePath OutputModSerializedPath => $"C:/Serialized/{OriginalModKey.FileName}";
    
    [Theory]
    [DefaultAutoData]
    public async Task EmptySkyrimModExport()
    {
        var mod = new SkyrimMod(OriginalModKey, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        Serialize(mod, stream);
        stream.Position = 0;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEnd();
        await Verifier.Verify(str);
    }
    
    [Theory]
    [DefaultAutoData]
    public async Task SingleGroupSkyrimModExport()
    {
        var mod = new SkyrimMod(OriginalModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        npc.Name = "Goblin";
        npc.Configuration.Level = new NpcLevel();
        npc.Configuration.HealthOffset = 100;
        var npc2 = mod.Npcs.AddNew();
        npc2.Name = "Hobgoblin";
        npc2.Configuration.HealthOffset = 200;
        npc2.Attacks.Add(new Attack()
        {
            AttackEvent = "Event1"
        });
        npc2.Attacks.Add(new Attack()
        {
            AttackEvent = "Event2"
        });
        var stream = new MemoryStream();
        Serialize(mod, stream);
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
        PassThrough(
            fileSystem,
            new SkyrimMod(OriginalModKey, SkyrimRelease.SkyrimSE));
    }

    private void PassThrough(
        IFileSystem fileSystem,
        SkyrimMod mod)
    {
        fileSystem.Directory.CreateDirectory(OutputModSerializedPath.Directory);
        Serialize(mod, fileSystem.File.OpenWrite(OutputModSerializedPath));
        var mod2 = Deserialize(fileSystem.File.OpenRead(OutputModSerializedPath));
        CheckEquality(fileSystem, mod, mod2);
    }

    private void CheckEquality(
        IFileSystem fileSystem,
        ISkyrimModGetter mod1,
        ISkyrimModGetter mod2)
    {
        fileSystem.Directory.CreateDirectory(OriginalModBinaryPath.Directory);
        fileSystem.Directory.CreateDirectory(OutputModBinaryPath.Directory);
        using (var fs = fileSystem.FileStream.Create(OriginalModBinaryPath, FileMode.Create))
        {
            mod1.WriteToBinaryParallel(fs);
        }
        using (var fs = fileSystem.FileStream.Create(OutputModBinaryPath, FileMode.Create))
        {
            mod2.WriteToBinaryParallel(fs);
        }
        AssertFilesEqual(
            fileSystem.FileStream.Create(OriginalModBinaryPath, FileMode.Open),
            OutputModBinaryPath,
            fileSystem.FileStream.Create(OutputModBinaryPath, FileMode.Open));
    }

    public static void AssertFilesEqual(
        Stream stream,
        FilePath stream2Path,
        Stream stream2,
        ushort amountToReport = 5)
    {
        using var reader2 = new BinaryReadStream(stream2);
        Stream compareStream = new ComparisonStream(
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