using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.Tests;

[UsesVerify]
public abstract class ASerializationTests
{
    public abstract void Serialize(SkyrimMod mod, Stream stream);
    public abstract ISkyrimModGetter Deserialize(Stream stream);
    public ModKey ModKey => ModKey.FromFileName("InputMod.esp");
    public string OriginalModBinaryPath => Path.Combine("Input", ModKey.FileName);
    public string OutputModBinaryPath => Path.Combine("Output", ModKey.FileName);
    public string OutputModSerializedPath => Path.Combine("Serialized", ModKey.FileName);
    
    [Theory]
    [TestAutoData]
    public async Task EmptySkyrimModExport()
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        Serialize(mod, stream);
        stream.Position = 0;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEnd();
        await Verifier.Verify(str);
    }
    
    [Theory]
    [TestAutoData]
    public async Task SingleGroupSkyrimModExport()
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
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
    [TestAutoData]
    public void EmptySkyrimModPassthrough(
        IFileSystem fileSystem)
    {
        PassThrough(
            fileSystem,
            new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE));
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public void SingleGroupPassthrough(
        IFileSystem fileSystem,
        Npc npc1,
        Npc npc2)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var firstNpc = mod.Npcs.AddNew();
        firstNpc.DeepCopyIn(npc1);
        var secondNpc = mod.Npcs.AddNew();
        secondNpc.DeepCopyIn(npc2);
        
        PassThrough(
            fileSystem,
            mod);
    }

    private void PassThrough(
        IFileSystem fileSystem,
        SkyrimMod mod)
    {
        using var tmp = TempFolder.FactoryByAddedPath(fileSystem: fileSystem, addedFolderPath: "Mutagen.Bethesda.Serialization.Tests");
        var filePath = Path.Combine(tmp.Dir, OutputModSerializedPath);
        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        using (var stream = fileSystem.File.OpenWrite(filePath))
        {
            Serialize(mod, stream);
        }

        ISkyrimModGetter mod2;
        using (var stream = fileSystem.File.OpenRead(filePath))
        {
            mod2 = Deserialize(stream);
        }
        CheckEquality(fileSystem, mod, mod2);
    }

    private void CheckEquality(
        IFileSystem fileSystem,
        ISkyrimModGetter mod1,
        ISkyrimModGetter mod2)
    {
        using var tmp = TempFolder.FactoryByAddedPath(fileSystem: fileSystem, addedFolderPath: "Mutagen.Bethesda.Serialization.Tests");
        var mod1OutFile = Path.Combine(tmp.Dir, OriginalModBinaryPath);
        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(mod1OutFile));
        var mod2OutFile = Path.Combine(tmp.Dir, OutputModBinaryPath);
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