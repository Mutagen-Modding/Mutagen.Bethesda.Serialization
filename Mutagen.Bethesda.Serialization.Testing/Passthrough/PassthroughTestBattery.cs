using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public abstract class PassthroughTestBattery
{
    protected abstract IPassthroughTest GetTest();

    private async Task RunTestFor(
        IFileSystem fileSystem,
        IFallout4ModGetter mod,
        DirectoryPath testFolder)
    {
        var path = Path.Combine(testFolder, mod.ModKey.ToString());

        // fileSystem = new FileSystem();
        fileSystem.Directory.CreateDirectory(testFolder);
        
        await mod.BeginWrite
            .WithNoLoadOrder()
            .ToPath(path)
            .WithFileSystem(fileSystem: fileSystem)
            .WriteAsync();

        await RunPassthrough.RunTest(
            GetTest(),
            new RunPassthroughCommand()
            {
                GameRelease = GameRelease.Fallout4,
                Parallel = false,
                Path = path,
                TestFolder = Path.Combine(testFolder)
            },
            fileSystem);

        await RunPassthrough.RunTest(
            GetTest(),
            new RunPassthroughCommand()
            {
                GameRelease = GameRelease.Fallout4,
                Parallel = true,
                Path = path,
                TestFolder = Path.Combine(testFolder)
            },
            fileSystem);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public async Task EmptyMod(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        Fallout4Mod skyrimMod)
    {
        await RunTestFor(
            fileSystem,
            skyrimMod,
            testDir);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Fallout4, ConfigureMembers: true)]
    public async Task TypicalRecords(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        Fallout4Mod mod,
        Ammunition ammo1,
        Ammunition ammo2,
        ArmorAddon armor1)
    {
        var path = $"C:/TestDirectory/{mod.ModKey}";

        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await mod.BeginWrite
            .WithNoLoadOrder()
            .ToPath(path)
            .WithFileSystem(fileSystem: fileSystem)
            .WriteAsync();

        await RunTestFor(
            fileSystem,
            mod,
            testDir);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Fallout4, ConfigureMembers: true)]
    public async Task GenericRecords(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        Fallout4Mod mod)
    {
        var armor = mod.Armors.AddNew();
        armor.ObjectTemplates ??= new();
        armor.ObjectTemplates.SetTo(new ObjectTemplate<Armor.Property>()
        {
            Properties = new ExtendedList<AObjectModProperty<Armor.Property>>()
            {
                new ObjectModBoolProperty<Armor.Property>()
                {
                    Property = Armor.Property.Rating,
                    Value = true,
                    Value2 = false,
                    Step = 1.23f
                }
            }
        });
        
        var path = $"C:/TestDirectory/{mod.ModKey}";

        fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await mod.BeginWrite
            .WithNoLoadOrder()
            .ToPath(path)
            .WithFileSystem(fileSystem: fileSystem)
            .WriteAsync();

        await RunTestFor(
            fileSystem,
            mod,
            testDir);
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4, ConfigureMembers: true)]
    public async Task Cell(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        Fallout4Mod mod,
        CellBlock cellBlock)
    {
        mod.Cells.Add(cellBlock);
        await RunTestFor(
            fileSystem,
            mod,
            testDir);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Fallout4, ConfigureMembers: true)]
    public async Task Worldspace(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        Fallout4Mod skyrimMod,
        Worldspace worldspace)
    {
        await RunTestFor(
            fileSystem,
            skyrimMod,
            testDir);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Fallout4, ConfigureMembers: true)]
    public async Task DialogTopic(
        IFileSystem fileSystem,
        DirectoryPath testDir,
        Fallout4Mod skyrimMod,
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