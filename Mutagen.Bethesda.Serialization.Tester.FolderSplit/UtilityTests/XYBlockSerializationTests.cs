using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;
using Shouldly;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit.UtilityTests;

[Collection("Verify")]
public class XYBlockSerializationTests
{
    [Theory, MutagenAutoData]
    public async Task AddXYBlocksToWork(
        IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(null!, existingDir);

        var group = new Fallout4Group<Worldspace>(null!)
        {
            LastModified = 793
        };

        var mod = new Fallout4Mod(modKey, Fallout4Release.Fallout4, forceUseLowerFormIDRanges: true);
        
        group.RecordCache.Set(new Worldspace(mod)
        {
            EditorID = "MyWorldspace",
            SubCellsTimestamp = 123,
            SubCells = new ExtendedList<WorldspaceBlock>()
            {
                new WorldspaceBlock()
                {
                    LastModified = 123,
                    BlockNumberX = 4,
                    BlockNumberY = 5,
                    Items = new ExtendedList<WorldspaceSubBlock>()
                    {
                        new WorldspaceSubBlock()
                        {
                            LastModified = 147,
                            BlockNumberX = 3,
                            BlockNumberY = 9,
                        },
                        new WorldspaceSubBlock()
                        {
                            LastModified = 852,
                            BlockNumberX = 6,
                            BlockNumberY = 4,
                            Items = new ExtendedList<Cell>()
                            {
                                new Cell(mod)
                                {
                                    EditorID = "TestCell",
                                    Temporary = new ExtendedList<IPlaced>()
                                    {
                                        new PlacedNpc(mod)
                                        {
                                            EditorID = "Placed1",
                                            Health = new Percent(0.23d)
                                        }
                                    },
                                    Persistent = new ExtendedList<IPlaced>()
                                    {
                                        new PlacedNpc(mod)
                                        {
                                            EditorID = "Placed2",
                                            Health = new Percent(0.45d)
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new WorldspaceBlock()
                {
                    LastModified = 456,
                    BlockNumberX = 4,
                    BlockNumberY = 6,
                }
            }
        });

        var metaData = new SerializationMetaData(GameRelease.Fallout4, new InlineWorkDropoff(), fileSystem, NormalFileStreamCreator.Instance, CancellationToken.None);

        await SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Fallout4Group<Worldspace>, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            group,
            "Worldspaces",
            static x => x.Records,
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            metaData,
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Fallout4Group_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace>(w, i, k, m),
            groupHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.Fallout4Group_Serialization.HasSerializationItems<Worldspace>(i, m),
            topRecordWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Worldspace_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            topRecordHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.Worldspace_Serialization.HasSerializationItems(i, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.WorldspaceBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.WorldspaceBlock_Serialization.HasSerializationItems(i, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.WorldspaceSubBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.WorldspaceSubBlock_Serialization.HasSerializationItems(i, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: true);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces")).ShouldBeTrue();
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp")).ShouldBeTrue();
        
        var headerDataPath = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "RecordData.json");
        fileSystem.File.Exists(headerDataPath).ShouldBeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);
        
        var fourFiveFolder = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5");
        fileSystem.Directory.Exists(fourFiveFolder).ShouldBeTrue();
        var fourFiveHeaderDataPath = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5", "GroupRecordData.json");
        fileSystem.File.Exists(fourFiveHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourFiveHeaderDataPath), settings);

        var fourSixFolder = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[1] 4, 6");
        fileSystem.Directory.Exists(fourSixFolder).ShouldBeTrue();
        var fourSixHeaderDataPath = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[1] 4, 6", "GroupRecordData.json");
        fileSystem.File.Exists(fourSixHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_6_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourSixHeaderDataPath), settings);
        
        var fourFiveThreeNineFolder = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5", "[0] 3, 9");
        fileSystem.Directory.Exists(fourFiveThreeNineFolder).ShouldBeTrue();
        var fourFiveThreeNineHeaderDataPath = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5", "[0] 3, 9", "GroupRecordData.json");
        fileSystem.File.Exists(fourFiveThreeNineHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_3_9_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourFiveThreeNineHeaderDataPath), settings);
        
        var fourFiveSixFourFolder = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5", "[1] 6, 4");
        fileSystem.Directory.Exists(fourFiveSixFourFolder).ShouldBeTrue();
        var fourFiveSixFourHeaderDataPath = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5", "[1] 6, 4", "GroupRecordData.json");
        fileSystem.File.Exists(fourFiveSixFourHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_6_4_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourFiveSixFourHeaderDataPath), settings);
        
        var cellDirPath = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5", "[1] 6, 4", "[0] TestCell - 000001_modKey.esp");
        fileSystem.Directory.Exists(cellDirPath).ShouldBeTrue();
        var cellDataPath = Path.Combine(existingDir, "Worldspaces", "[0] MyWorldspace - 000000_modKey.esp", "[0] 4, 5", "[1] 6, 4", "[0] TestCell - 000001_modKey.esp", "RecordData.json");
        fileSystem.File.Exists(cellDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_6_4_TestCellRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(cellDataPath), settings);

        var tempPath = Path.Combine(cellDirPath, "Temporary");
        fileSystem.Directory.Exists(tempPath).ShouldBeTrue();
        var placed1Path = Path.Combine(tempPath, "[0] Placed1 - 000002_modKey.esp.json");
        fileSystem.File.Exists(placed1Path).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_6_4_TestCell_Temporary_Placed1.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed1Path), settings);
        
        var persistentPath = Path.Combine(cellDirPath, "Persistent");
        fileSystem.Directory.Exists(persistentPath).ShouldBeTrue();
        var placed2Path = Path.Combine(persistentPath, "[0] Placed2 - 000003_modKey.esp.json");
        fileSystem.File.Exists(placed2Path).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_6_4_TestCell_Persistent_Placed2.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed2Path), settings);
    }
    
    [Theory, MutagenAutoData]
    public async Task TopLevelRecord(
        IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(null!, existingDir);

        var group = new Fallout4Group<Worldspace>(null!)
        {
            LastModified = 793
        };

        var mod = new Fallout4Mod(modKey, Fallout4Release.Fallout4, forceUseLowerFormIDRanges: true);

        var ws = group.RecordCache.SetReturn(new Worldspace(mod)
        {
            EditorID = "MyWorldspace",
            SubCellsTimestamp = 123,
            TopCell = new Cell(mod)
            {
                EditorID = "TestCell",
                Temporary = new ExtendedList<IPlaced>()
                {
                    new PlacedNpc(mod)
                    {
                        EditorID = "Placed1",
                        Health = new Percent(0.23d)
                    }
                },
                Persistent = new ExtendedList<IPlaced>()
                {
                    new PlacedNpc(mod)
                    {
                        EditorID = "Placed2",
                        Health = new Percent(0.45d)
                    }
                }
            }
        });

        var metaData = new SerializationMetaData(GameRelease.Fallout4, new InlineWorkDropoff(), fileSystem, NormalFileStreamCreator.Instance, CancellationToken.None);

        await SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Fallout4Group<Worldspace>, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            group,
            "Worldspaces",
            static x => x.Records,
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            metaData,
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Fallout4Group_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace>(w, i, k, m),
            groupHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.Fallout4Group_Serialization.HasSerializationItems<Worldspace>(i, m),
            topRecordWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Worldspace_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            topRecordHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.Worldspace_Serialization.HasSerializationItems(i, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.WorldspaceBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.WorldspaceBlock_Serialization.HasSerializationItems(i, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.WorldspaceSubBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.WorldspaceSubBlock_Serialization.HasSerializationItems(i, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: true);

        var wsFolder = $"[0] MyWorldspace - {ws.FormKey.ToFilesafeString()}";
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces")).ShouldBeTrue();
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces", wsFolder)).ShouldBeTrue();
        
        var headerDataPath = Path.Combine(existingDir, "Worldspaces", wsFolder, "RecordData.json");
        fileSystem.File.Exists(headerDataPath).ShouldBeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(TopLevelRecord)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);
        
        var cellDirPath = Path.Combine(existingDir, "Worldspaces", wsFolder, "TopCell");
        fileSystem.Directory.Exists(cellDirPath).ShouldBeTrue();
        var cellDataPath = Path.Combine(existingDir, "Worldspaces", wsFolder, "TopCell", "RecordData.json");
        fileSystem.File.Exists(cellDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(TopLevelRecord)}_4_5_6_4_TestCellRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(cellDataPath), settings);

        var tempPath = Path.Combine(cellDirPath, "Temporary");
        fileSystem.Directory.Exists(tempPath).ShouldBeTrue();
        var placed1Path = Path.Combine(tempPath, "[0] Placed1 - 000002_modKey.esp.json");
        fileSystem.File.Exists(placed1Path).ShouldBeTrue();
        settings.UseFileName($"{nameof(TopLevelRecord)}_4_5_6_4_TestCell_Temporary_Placed1.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed1Path), settings);
        
        var persistentPath = Path.Combine(cellDirPath, "Persistent");
        fileSystem.Directory.Exists(persistentPath).ShouldBeTrue();
        var placed2Path = Path.Combine(persistentPath, "[0] Placed2 - 000003_modKey.esp.json");
        fileSystem.File.Exists(placed2Path).ShouldBeTrue();
        settings.UseFileName($"{nameof(TopLevelRecord)}_4_5_6_4_TestCell_Persistent_Placed2.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed2Path), settings);
    }
}