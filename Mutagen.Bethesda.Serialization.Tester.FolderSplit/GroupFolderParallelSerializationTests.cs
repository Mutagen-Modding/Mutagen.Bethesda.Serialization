using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WorkEngine;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

[UsesVerify, Collection("Verify")]
public class GroupFolderParallelSerializationTests
{
    [Theory, TestAutoData]
    public async Task WriteFolderPerRecord(
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

        await SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE, new InlineWorkDropoff()),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        var headerDataPath = Path.Combine(existingDir, "Npcs", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        var npcPath1 = Path.Combine(existingDir, "Npcs", "TestEdid", "RecordData.json");
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        var npcPath2 = Path.Combine(existingDir, "Npcs", "123457_Skyrim.esm", "RecordData.json");
        fileSystem.File.Exists(npcPath2).Should().BeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(WriteFolderPerRecord)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);
        settings.UseFileName($"{nameof(WriteFolderPerRecord)}_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath1), settings);
        settings.UseFileName($"{nameof(WriteFolderPerRecord)}_123457_Skyrim.esm.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath2), settings);
    }
    
    [Theory, TestAutoData]
    public async Task WriteFolderPerRecordRemovesDeletedRecords(
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

        var metadata = new SerializationMetaData(GameRelease.SkyrimSE, new InlineWorkDropoff());

        await SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            metadata,
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        var headerDataPath = Path.Combine(existingDir, "Npcs", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        var npcPath1 = Path.Combine(existingDir, "Npcs", "TestEdid", "RecordData.json");
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        var npcPath2 = Path.Combine(existingDir, "Npcs", "123457_Skyrim.esm", "RecordData.json");
        fileSystem.File.Exists(npcPath2).Should().BeTrue();
        
        // Remove one
        group.Remove(npc2);
        
        await SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            metadata,
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        fileSystem.File.Exists(npcPath2).Should().BeFalse();
        
        // Remove all
        group.Clear();

        await SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            metadata,
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.File.Exists(npcPath1).Should().BeFalse();
        fileSystem.File.Exists(npcPath2).Should().BeFalse();
    }
}