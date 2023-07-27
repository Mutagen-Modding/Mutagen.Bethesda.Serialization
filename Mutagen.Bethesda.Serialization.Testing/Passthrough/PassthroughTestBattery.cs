using System.IO.Abstractions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public abstract class PassthroughTestBattery
{
    protected abstract IPassthroughTest GetTest();

    private async Task RunTestFor(
        IFileSystem fileSystem,
        ISkyrimModGetter mod,
        DirectoryPath testFolder)
    {
        var path = Path.Combine(testFolder, mod.ModKey.ToString());

        // fileSystem = new FileSystem();
        fileSystem.Directory.CreateDirectory(testFolder);
        
        mod.WriteToBinary(path, fileSystem: fileSystem);

        await RunPassthrough.RunTest(
            GetTest(),
            new RunPassthroughCommand()
            {
                GameRelease = GameRelease.SkyrimSE,
                Parallel = false,
                Path = path,
                TestFolder = Path.Combine(testFolder)
            },
            fileSystem);

        await RunPassthrough.RunTest(
            GetTest(),
            new RunPassthroughCommand()
            {
                GameRelease = GameRelease.SkyrimSE,
                Parallel = true,
                Path = path,
                TestFolder = Path.Combine(testFolder)
            },
            fileSystem);
    }
    
    [Theory, MutagenModAutoData]
    public async Task EmptyMod(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        SkyrimMod skyrimMod)
    {
        await RunTestFor(
            fileSystem,
            skyrimMod,
            testDir);
    }
    
    [Theory, MutagenModAutoData(ConfigureMembers: true)]
    public async Task TypicalRecords(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        SkyrimMod mod,
        Npc npc1,
        Npc npc2,
        Weapon weapon1)
    {
        var path = $"C:/TestDirectory/{mod.ModKey}";

        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        mod.WriteToBinary(path, fileSystem: fileSystem);

        await RunTestFor(
            fileSystem,
            mod,
            testDir);
    }

    [Theory, MutagenModAutoData(ConfigureMembers: true)]
    public async Task Cell(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        SkyrimMod mod,
        CellBlock cellBlock)
    {
        mod.Cells.Add(cellBlock);
        await RunTestFor(
            fileSystem,
            mod,
            testDir);
    }
    
    [Theory, MutagenModAutoData(ConfigureMembers: true)]
    public async Task Worldspace(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        SkyrimMod skyrimMod,
        Worldspace worldspace)
    {
        await RunTestFor(
            fileSystem,
            skyrimMod,
            testDir);
    }
    
    [Theory, MutagenModAutoData(ConfigureMembers: true)]
    public async Task DialogTopic(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        SkyrimMod skyrimMod,
        DialogTopic topic)
    {
        // Ideally not required, but builder not yet smart enough
        // to set in bounds
        topic.Responses.ForEach(r =>
        {
            if (r.Flags != null)
            {
                r.Flags.ResetHours = 5f;
            }
        });
        await RunTestFor(
            fileSystem,
            skyrimMod,
            testDir);
    }
}