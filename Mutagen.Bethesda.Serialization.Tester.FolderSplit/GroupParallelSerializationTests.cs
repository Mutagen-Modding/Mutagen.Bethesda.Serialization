using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

[UsesVerify, Collection("Verify")]
public class GroupParallelSerializationTests
{
    [Theory, TestAutoData]
    public async Task WriteFilePerRecord(
        IFileSystem fileSystem,
        FilePath someFile,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir, fileSystem);

        var npc1 = new Npc(FormKey.Factory("123456:Skyrim.esm"), SkyrimRelease.SkyrimSE);
        var npc2 = new Npc(FormKey.Factory("123457:Skyrim.esm"), SkyrimRelease.SkyrimSE);
        npc1.EditorID = "TestEdid";
        npc1.Height = 5;
        npc2.EditorID = null;
        npc2.Height = 15;
        var group = new SkyrimGroup<Npc>(null!)
        {
            LastModified = 123,
        };
        group.Add(npc1);
        group.Add(npc2);

        var toDo = new List<Action>();
        
        SerializationHelper.WriteFilePerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        var headerDataPath = Path.Combine(existingDir, "Npcs", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        var npcPath1 = Path.Combine(existingDir, "Npcs", "TestEdid.json");
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        var npcPath2 = Path.Combine(existingDir, "Npcs", "123457_Skyrim.esm.json");
        fileSystem.File.Exists(npcPath2).Should().BeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(WriteFilePerRecord)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);
        settings.UseFileName($"{nameof(WriteFilePerRecord)}_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath1), settings);
        settings.UseFileName($"{nameof(WriteFilePerRecord)}_123457_Skyrim.esm.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath2), settings);
    }
    
    [Theory, TestAutoData]
    public async Task WriteFilePerRecordRemovesDeletedRecords(
        IFileSystem fileSystem,
        FilePath someFile,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir, fileSystem);

        var npc1 = new Npc(FormKey.Factory("123456:Skyrim.esm"), SkyrimRelease.SkyrimSE);
        var npc2 = new Npc(FormKey.Factory("123457:Skyrim.esm"), SkyrimRelease.SkyrimSE);
        npc1.EditorID = "TestEdid";
        npc2.EditorID = null;
        var group = new SkyrimGroup<Npc>(null!)
        {
            LastModified = 123,
        };
        group.Add(npc1);
        group.Add(npc2);

        var toDo = new List<Action>();
        
        SerializationHelper.WriteFilePerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);
        
        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        var headerDataPath = Path.Combine(existingDir, "Npcs", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        var npcPath1 = Path.Combine(existingDir, "Npcs", "TestEdid.json");
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        var npcPath2 = Path.Combine(existingDir, "Npcs", "123457_Skyrim.esm.json");
        fileSystem.File.Exists(npcPath2).Should().BeTrue();
        
        // Remove one
        group.Remove(npc2);
        toDo.Clear();
        
        SerializationHelper.WriteFilePerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        fileSystem.File.Exists(npcPath2).Should().BeFalse();
        
        // Remove all
        group.Clear();
        toDo.Clear();
        
        SerializationHelper.WriteFilePerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.File.Exists(npcPath1).Should().BeFalse();
        fileSystem.File.Exists(npcPath2).Should().BeFalse();
    }
    
}