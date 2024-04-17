using System.Drawing;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Testing;
using Mutagen.Bethesda.Fallout4;
using Noggog;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Tests;

public abstract class ASerializationTests
{
    public abstract Task Serialize(IFallout4ModGetter mod, Stream stream, ICreateStream createStream);
    public abstract Task<IFallout4ModGetter> Deserialize(Stream stream, ModKey modKey, GameRelease release, ICreateStream createStream);
    public ModKey ModKey => ModKey.FromFileName("InputMod.esp");

    [Fact]
    public async Task EmptyFallout4ModExport()
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4, forceUseLowerFormIDRanges: true);
        var stream = new MemoryStream();
        await Serialize(mod, stream, NormalFileStreamCreator.Instance);
        stream.Position = 0;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEnd();
        await Verifier.Verify(str);
    }
    
    [Fact]
    public async Task SingleGroupFallout4ModExport()
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4, forceUseLowerFormIDRanges: true);
        var npc = mod.Npcs.AddNew();
        npc.Name = "Goblin";
        npc.Level = new NpcLevel()
        {
            Level = 123,
        };
        var npc2 = mod.Npcs.AddNew();
        npc2.Name = "Hobgoblin";
        npc.Level = new PcLevelMult()
        {
            LevelMult = 1.3f
        };
        npc2.Attacks.Add(new Attack()
        {
            AttackEvent = "Event1"
        });
        npc2.Attacks.Add(new Attack()
        {
            AttackEvent = "Event2"
        });
        var stream = new MemoryStream();
        await Serialize(mod, stream, NormalFileStreamCreator.Instance);
        stream.Position = 0;
        StreamReader streamReader = new StreamReader(stream);
        var str = streamReader.ReadToEnd();
        await Verifier.Verify(str);
    }
    
    [Theory, TestAutoData]
    public async Task EmptyFallout4ModPassthrough(
        IFileSystem fileSystem)
    {
        await PassThrough(
            fileSystem,
            new Fallout4Mod(ModKey, Fallout4Release.Fallout4));
    }
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task GroupPassthrough(
        IFileSystem fileSystem,
        Ammunition ammo1,
        Ammunition ammo2,
        ArmorAddon armorAddon1,
        ArmorAddon armorAddon2)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var firstAmmo = mod.Ammunitions.AddNew();
        firstAmmo.DeepCopyIn(ammo1);
        var secondAmmo = mod.Ammunitions.AddNew();
        secondAmmo.DeepCopyIn(ammo2);
        var firstArmorAddon = mod.ArmorAddons.AddNew();
        firstArmorAddon.DeepCopyIn(armorAddon1);
        var secondArmorAddon = mod.ArmorAddons.AddNew();
        secondArmorAddon.DeepCopyIn(armorAddon2);

        await PassThrough( 
            fileSystem,
            mod);
    }
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task EnumDictionary(
        IFileSystem fileSystem,
        IEnumerable<KeyValuePair<BipedObject, BipedObjectData>> vals)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var newRace = mod.Races.AddNew();
        
        // Can remove after update
        for (int key = 0; key < 32; ++key)
        {
            newRace.BipedObjects[(BipedObject)key] = new BipedObjectData();
        }
        
        newRace.BipedObjects.Set(vals);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task FlagEnum(IFileSystem fileSystem)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var added = mod.Races.AddNew();
        
        // Can remove after update
        for (int key = 0; key < 32; ++key)
        {
            added.BipedObjects[(BipedObject)key] = new BipedObjectData();
        }
        
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
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task ColorWithAlpha(IFileSystem fileSystem)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var added = mod.Keywords.AddNew();
        added.Color = Color.FromArgb(55, 66, 77, 88);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task NullableButDefaultLoqui(IFileSystem fileSystem)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var added = mod.Factions.AddNew();
        added.VendorValues = new();
        
        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task NullableButDefaultEnum(IFileSystem fileSystem)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var added = mod.TextureSets.AddNew();
        added.Flags = default(TextureSet.Flag);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task GenderedItem(
        IFileSystem fileSystem,
        Faction f)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var newFaction = mod.Factions.AddNew();
        newFaction.DeepCopyIn(f);

        await PassThrough(
            fileSystem,
            mod);
    }
    
    [Theory, TestAutoData(ConfigureMembers: true)]
    public async Task Subclassed(
        IFileSystem fileSystem)
    {
        var mod = new Fallout4Mod(ModKey, Fallout4Release.Fallout4);
        var newFloat = mod.Globals.AddNewFloat();
        newFloat.Data = 1.3f;
        
        await PassThrough(
            fileSystem,
            mod);
    }

    private async Task PassThrough(IFileSystem fileSystem, Fallout4Mod mod)
    {
        using var tmp = TempFolder.FactoryByAddedPath(fileSystem: fileSystem, addedFolderPath: "Mutagen.Bethesda.Serialization.Tests");
        await PassthroughTest.PassThrough(
            fileSystem,
            tmp.Dir,
            mod,
            async (m, d, c) =>
            {
                using var s = c.GetStreamFor(fileSystem, Path.Combine(d, mod.ModKey.ToString()), write: true);
                await Serialize(m, s, c);
            },
            async (d, m, c) =>
            {
                using var s = c.GetStreamFor(fileSystem, Path.Combine(d, m.ToString()), write: false);
                return await Deserialize(s, mod.ModKey, mod.GameRelease, c);
            });
    }
}