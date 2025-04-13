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
public class BlockSerializationTests
{
    [Theory, TestAutoData]
    public async Task AddBlocksToWork(
        IFileSystem fileSystem,
        FilePath someFile,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir);

        var mod = new Fallout4Mod(ModKey.FromNameAndExtension("Mod0.esp"), Fallout4Release.Fallout4, forceUseLowerFormIDRanges: true);

        mod.Cells.LastModified = 789;
        mod.Cells.Records.SetTo(
            new []
            {
                new CellBlock()
                {
                    LastModified = 123,
                    BlockNumber = 1,
                    SubBlocks = new ExtendedList<CellSubBlock>()
                    {
                        new CellSubBlock()
                        {
                            LastModified = 147,
                            BlockNumber = 3
                        },
                        new CellSubBlock()
                        {
                            LastModified = 852,
                            BlockNumber = 6,
                            Cells = new ExtendedList<Cell>()
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
                new CellBlock()
                {
                    LastModified = 456,
                    BlockNumber = 4,
                }
            });

        var metaData = new SerializationMetaData(GameRelease.Fallout4, new InlineWorkDropoff(), fileSystem, NormalFileStreamCreator.Instance, CancellationToken.None);

        await SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Fallout4ListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
            streamPackage,
            mod.Cells,
            "Cells",
            static x => x.Records,
            static x => x.SubBlocks,
            static x => x.Cells,
            static x => x.BlockNumber,
            static x => x.BlockNumber,
            metaData,
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Fallout4ListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            metaHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.Fallout4ListGroup_Serialization.HasSerializationItems<ICellBlockGetter>(i, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.CellBlock_Serialization.HasSerializationItems(i, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockHasSerialization: static (i, m) => Mutagen.Bethesda.Fallout4.CellSubBlock_Serialization.HasSerializationItems(i, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Fallout4.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: true);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Cells")).ShouldBeTrue();
        var headerDataPath = Path.Combine(existingDir, "Cells", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).ShouldBeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);

        var oneFolder = Path.Combine(existingDir, "Cells", "[0] 1");
        fileSystem.Directory.Exists(oneFolder).ShouldBeTrue();
        var oneHeaderDataPath = Path.Combine(existingDir, "Cells", "[0] 1", "GroupRecordData.json");
        fileSystem.File.Exists(oneHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneHeaderDataPath), settings);

        var fourFolder = Path.Combine(existingDir, "Cells", "[1] 4");
        fileSystem.Directory.Exists(fourFolder).ShouldBeTrue();
        var fourHeaderDataPath = Path.Combine(existingDir, "Cells", "[1] 4", "GroupRecordData.json");
        fileSystem.File.Exists(fourHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_4_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourHeaderDataPath), settings);

        var subOneThreeFolder = Path.Combine(existingDir, "Cells", "[0] 1", "[0] 3");
        fileSystem.Directory.Exists(subOneThreeFolder).ShouldBeTrue();
        var oneThreeHeaderDataPath = Path.Combine(existingDir, "Cells", "[0] 1", "[0] 3", "GroupRecordData.json");
        fileSystem.File.Exists(oneThreeHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_3_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneThreeHeaderDataPath), settings);

        var subOneSixFolder = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6");
        fileSystem.Directory.Exists(subOneSixFolder).ShouldBeTrue();
        var oneSixHeaderDataPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "GroupRecordData.json");
        fileSystem.File.Exists(oneSixHeaderDataPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneSixHeaderDataPath), settings);
        
        var cellDirPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell - 000000_Mod0.esp");
        fileSystem.Directory.Exists(cellDirPath).ShouldBeTrue();
        var cellPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell - 000000_Mod0.esp", "RecordData.json");
        fileSystem.File.Exists(cellPath).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(cellPath), settings);
        
        var tempPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell - 000000_Mod0.esp", "Temporary");
        fileSystem.Directory.Exists(tempPath).ShouldBeTrue();
        var placed1Path = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell - 000000_Mod0.esp", "Temporary", "[0] Placed1 - 000001_Mod0.esp.json");
        fileSystem.File.Exists(placed1Path).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_TestEdid_Temporary_Placed1.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed1Path), settings);
        
        var perstPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell - 000000_Mod0.esp", "Persistent");
        fileSystem.Directory.Exists(perstPath).ShouldBeTrue();
        var placed2Path = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell - 000000_Mod0.esp", "Persistent", "[0] Placed2 - 000002_Mod0.esp.json");
        fileSystem.File.Exists(placed2Path).ShouldBeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_TestEdid_Persistent_Placed2.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed2Path), settings);
    }
}