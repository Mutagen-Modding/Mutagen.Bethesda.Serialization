using System.Drawing;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Testing;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Tests;

[UsesVerify]
public abstract class ASerializationTests
{
    public abstract void Serialize(ISkyrimModGetter mod, Stream stream);
    public abstract ISkyrimModGetter Deserialize(Stream stream);
    public ModKey ModKey => ModKey.FromFileName("InputMod.esp");
    
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
    
    [Theory]
    [TestAutoData(ConfigureMembers: true)]
    public async Task EnumDictionary(
        IFileSystem fileSystem,
        IEnumerable<KeyValuePair<BasicStat, Byte>> vals)
    {
        var mod = new SkyrimMod(ModKey, SkyrimRelease.SkyrimSE);
        var newClass = mod.Classes.AddNew();
        newClass.StatWeights.SetTo(vals);
        
        PassThrough(
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
        
        PassThrough(
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
        
        PassThrough(
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
        
        PassThrough(
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
        
        PassThrough(
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
        
        PassThrough(
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
        
        PassThrough(
            fileSystem,
            mod);
    }

    private void PassThrough(IFileSystem fileSystem, SkyrimMod skyrimMod)
    {
        using var tmp = TempFolder.FactoryByAddedPath(fileSystem: fileSystem, addedFolderPath: "Mutagen.Bethesda.Serialization.Tests");
        PassthroughTest.PassThrough(
            fileSystem,
            tmp.Dir,
            skyrimMod,
            Serialize,
            Deserialize);
    }
}