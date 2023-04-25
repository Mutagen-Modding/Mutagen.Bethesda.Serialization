using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

[UsesVerify, Collection("Verify")]
public class MajorRecordListSerializationTests
{
    [Theory, TestAutoData]
    public async Task TypicalWrite(
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
        var list = new List<Npc>()
        {
            npc1,
            npc2
        };

        var metaData = new SerializationMetaData(GameRelease.SkyrimSE, new InlineWorkDropoff());

        await SerializationHelper.WriteMajorRecordList<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Npc>(
            streamPackage,
            "Npcs",
            list,
            metaData,
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        var npcPath1 = Path.Combine(existingDir, "Npcs", "TestEdid.json");
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        var npcPath2 = Path.Combine(existingDir, "Npcs", "123457_Skyrim.esm.json");
        fileSystem.File.Exists(npcPath2).Should().BeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(TypicalWrite)}_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath1), settings);
        settings.UseFileName($"{nameof(TypicalWrite)}_123457_Skyrim.esm.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath2), settings);
    }
    
}