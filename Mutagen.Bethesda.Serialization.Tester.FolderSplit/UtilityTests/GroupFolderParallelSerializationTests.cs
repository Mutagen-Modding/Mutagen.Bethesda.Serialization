using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Testing;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Fallout4;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;
using Shouldly;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit.UtilityTests;

[Collection("Verify")]
public class GroupFolderParallelSerializationTests
{
    [Theory, TestAutoData]
    public async Task WriteFolderPerRecord(
        IFileSystem fileSystem,
        FilePath someFile,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir);

        var npc1 = new Npc(FormKey.Factory("123456:Fallout4.esm"), Fallout4Release.Fallout4);
        var npc2 = new Npc(FormKey.Factory("123457:Fallout4.esm"), Fallout4Release.Fallout4);
        npc1.EditorID = "TestEdid";
        npc1.Aggression = Npc.AggressionType.Frenzied;
        npc2.EditorID = null;
        npc2.Aggression = Npc.AggressionType.Unaggressive;
        var group = new Fallout4Group<Npc>(null!)
        {
            LastModified = 123,
        };
        group.Add(npc1);
        group.Add(npc2);

        await SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Fallout4Group<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.Fallout4, new InlineWorkDropoff(), fileSystem, NormalFileStreamCreator.Instance, CancellationToken.None),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Fallout4Group_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            groupHasSerializationItems: static (i, m) => Mutagen.Bethesda.Fallout4.Fallout4Group_Serialization.HasSerializationItems<INpcGetter>(i, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).ShouldBeTrue();
        var headerDataPath = Path.Combine(existingDir, "Npcs", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).ShouldBeTrue();
        var npcPath1 = Path.Combine(existingDir, "Npcs", $"TestEdid - 123456_Fallout4.esm", "RecordData.json");
        fileSystem.File.Exists(npcPath1).ShouldBeTrue();
        var npcPath2 = Path.Combine(existingDir, "Npcs", "123457_Fallout4.esm", "RecordData.json");
        fileSystem.File.Exists(npcPath2).ShouldBeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(WriteFolderPerRecord)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);
        settings.UseFileName($"{nameof(WriteFolderPerRecord)}_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath1), settings);
        settings.UseFileName($"{nameof(WriteFolderPerRecord)}_123457_Fallout4.esm.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath2), settings);
    }
}