using System.Drawing;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Testing;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;
using Xunit;

namespace Mutagen.Bethesda.Serialization.Tests;

[UsesVerify]
public abstract class ASerializationTests
{
    public abstract Task Serialize(ISkyrimModGetter mod, Stream stream);
    public abstract Task<ISkyrimModGetter> Deserialize(Stream stream);
    public ModKey ModKey => ModKey.FromFileName("InputMod.esp");
    
    [Theory]
    [TestAutoData]
    public async Task EmptySkyrimModExport()
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        await Serialize(mod, stream);
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
        await Serialize(mod, stream);
        stream.Position = 0;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEnd();
        await Verifier.Verify(str);
    }
    
    [Theory]
    [TestAutoData]
    public async Task EmptySkyrimModPassthrough(
        IFileSystem fileSystem)
    {
        await PassThrough(
            fileSystem,
            new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE));
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task GroupPassthrough(
        IFileSystem fileSystem,
        Npc npc1,
        Npc npc2,
        Weapon weapon1,
        Weapon weapon2)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var firstNpc = mod.Npcs.AddNew();
        firstNpc.DeepCopyIn(npc1);
        var secondNpc = mod.Npcs.AddNew();
        secondNpc.DeepCopyIn(npc2);
        var firstWeapon = mod.Weapons.AddNew();
        firstWeapon.DeepCopyIn(weapon1);
        var secondWeapon = mod.Weapons.AddNew();
        secondWeapon.DeepCopyIn(weapon2);

        await PassThrough( 
            fileSystem,
            mod);
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task EnumDictionary(
        IFileSystem fileSystem,
        IEnumerable<KeyValuePair<BasicStat, Byte>> vals)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var newClass = mod.Classes.AddNew();
        newClass.StatWeights.SetTo(vals);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task FlagEnum(IFileSystem fileSystem)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var added = mod.Races.AddNew();
        added.Flags = Race.Flag.FaceGenHead 
                      | Race.Flag.Child
                      | Race.Flag.Swims
                      | Race.Flag.Walks
                      | Race.Flag.NoCombatInWater
                      | Race.Flag.UsesHeadTrackAnims
                      | Race.Flag.AllowPcDialog
                      | Race.Flag.CanPickupItems
                      | Race.Flag.CanDualWield;

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task ColorWithAlpha(IFileSystem fileSystem)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var added = mod.Keywords.AddNew();
        added.Color = Color.FromArgb(55, 66, 77, 88);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task NullableButDefaultLoqui(IFileSystem fileSystem)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var added = mod.Factions.AddNew();
        added.VendorValues = new();
        
        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task NullableButDefaultEnum(IFileSystem fileSystem)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var added = mod.TextureSets.AddNew();
        added.Flags = default(TextureSet.Flag);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task GenderedItem(
        IFileSystem fileSystem,
        Faction f)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var newFaction = mod.Factions.AddNew();
        newFaction.DeepCopyIn(f);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task Subclassed(
        IFileSystem fileSystem)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var newFloat = mod.Globals.AddNewFloat();
        newFloat.Data = 1.3f;
        
        await PassThrough(
            fileSystem,
            mod);
    }

    private async Task PassThrough(IFileSystem fileSystem, SkyrimMod skyrimMod)
    {
        using var tmp = TempFolder.FactoryByAddedPath(fileSystem: fileSystem, addedFolderPath: "Mutagen.Bethesda.Serialization.Tests");
        await PassthroughTest.PassThrough(
            fileSystem,
            tmp.Dir,
            skyrimMod,
            (m, s) => Serialize(m, s.Stream),
            (s) => Deserialize(s.Stream));
    }
}