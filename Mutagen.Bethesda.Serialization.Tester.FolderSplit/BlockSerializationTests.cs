using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Testing;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

[UsesVerify, Collection("Verify")]
public class BlockSerializationTests
{
    [Theory, TestAutoData]
    public async Task AddBlocksToWork(
        IFileSystem fileSystem,
        FilePath someFile,
        SkyrimMod mod,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir);

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
                                            Health = 23
                                        }
                                    },
                                    Persistent = new ExtendedList<IPlaced>()
                                    {
                                        new PlacedNpc(mod)
                                        {
                                            EditorID = "Placed2",
                                            Health = 45
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

        var metaData = new SerializationMetaData(GameRelease.SkyrimSE, new InlineWorkDropoff(), fileSystem, NormalFileStreamCreator.Instance);

        await SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
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
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: true);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Cells")).Should().BeTrue();
        var headerDataPath = Path.Combine(existingDir, "Cells", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);

        var oneFolder = Path.Combine(existingDir, "Cells", "[0] 1");
        fileSystem.Directory.Exists(oneFolder).Should().BeTrue();
        var oneHeaderDataPath = Path.Combine(existingDir, "Cells", "[0] 1", "GroupRecordData.json");
        fileSystem.File.Exists(oneHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneHeaderDataPath), settings);

        var fourFolder = Path.Combine(existingDir, "Cells", "[1] 4");
        fileSystem.Directory.Exists(fourFolder).Should().BeTrue();
        var fourHeaderDataPath = Path.Combine(existingDir, "Cells", "[1] 4", "GroupRecordData.json");
        fileSystem.File.Exists(fourHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_4_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourHeaderDataPath), settings);

        var subOneThreeFolder = Path.Combine(existingDir, "Cells", "[0] 1", "[0] 3");
        fileSystem.Directory.Exists(subOneThreeFolder).Should().BeTrue();
        var oneThreeHeaderDataPath = Path.Combine(existingDir, "Cells", "[0] 1", "[0] 3", "GroupRecordData.json");
        fileSystem.File.Exists(oneThreeHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_3_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneThreeHeaderDataPath), settings);

        var subOneSixFolder = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6");
        fileSystem.Directory.Exists(subOneSixFolder).Should().BeTrue();
        var oneSixHeaderDataPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "GroupRecordData.json");
        fileSystem.File.Exists(oneSixHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneSixHeaderDataPath), settings);
        
        var cellDirPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell");
        fileSystem.Directory.Exists(cellDirPath).Should().BeTrue();
        var cellPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell", "RecordData.json");
        fileSystem.File.Exists(cellPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(cellPath), settings);
        
        var tempPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell", "Temporary");
        fileSystem.Directory.Exists(tempPath).Should().BeTrue();
        var placed1Path = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell", "Temporary", "[0] Placed1.json");
        fileSystem.File.Exists(placed1Path).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_TestEdid_Temporary_Placed1.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed1Path), settings);
        
        var perstPath = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell", "Persistent");
        fileSystem.Directory.Exists(perstPath).Should().BeTrue();
        var placed2Path = Path.Combine(existingDir, "Cells", "[0] 1", "[1] 6", "[0] TestCell", "Persistent", "[0] Placed2.json");
        fileSystem.File.Exists(placed2Path).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_TestEdid_Persistent_Placed2.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(placed2Path), settings);
    }
    
    [Theory, TestAutoData]
    public async Task AddBlocksToWorkRemovesDeletedRecords(
        IFileSystem fileSystem,
        FilePath someFile,
        SkyrimMod mod,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir);

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
                                    EditorID = "TestCell"
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

        var metaData = new SerializationMetaData(GameRelease.SkyrimSE, new InlineWorkDropoff(), fileSystem, NormalFileStreamCreator.Instance);

        await SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
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
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        var headerDataPath = Path.Combine(existingDir, "Cells", "GroupRecordData.json");
        var oneFolder = Path.Combine(existingDir, "Cells", "1");
        var oneHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "GroupRecordData.json");
        var fourFolder = Path.Combine(existingDir, "Cells", "4");
        var fourHeaderDataPath = Path.Combine(existingDir, "Cells", "4", "GroupRecordData.json");
        var subOneThreeFolder = Path.Combine(existingDir, "Cells", "1", "3");
        var oneThreeHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "3", "GroupRecordData.json");
        var subOneSixFolder = Path.Combine(existingDir, "Cells", "1", "6");
        var cellDirPath = Path.Combine(existingDir, "Cells", "1", "6", "TestCell");
        var cellPath = Path.Combine(existingDir, "Cells", "1", "6", "TestCell", "RecordData.json");
        var oneSixHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "6", "GroupRecordData.json");
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Cells")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(oneFolder).Should().BeTrue();
        fileSystem.File.Exists(oneHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(fourFolder).Should().BeTrue();
        fileSystem.File.Exists(fourHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(subOneThreeFolder).Should().BeTrue();
        fileSystem.File.Exists(oneThreeHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(subOneSixFolder).Should().BeTrue();
        fileSystem.File.Exists(oneSixHeaderDataPath).Should().BeTrue();
        
        fileSystem.Directory.Exists(cellDirPath).Should().BeTrue();
        fileSystem.File.Exists(cellPath).Should().BeTrue();
        
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
                            LastModified = 852,
                            BlockNumber = 6,
                            Cells = new ExtendedList<Cell>()
                            {
                                new Cell(mod)
                                {
                                    EditorID = "TestCell"
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
        
        await SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
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
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Cells")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(oneFolder).Should().BeTrue();
        fileSystem.File.Exists(oneHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(fourFolder).Should().BeTrue();
        fileSystem.File.Exists(fourHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(subOneThreeFolder).Should().BeFalse();
        fileSystem.File.Exists(oneThreeHeaderDataPath).Should().BeFalse();

        fileSystem.Directory.Exists(subOneSixFolder).Should().BeTrue();
        fileSystem.File.Exists(oneSixHeaderDataPath).Should().BeTrue();
        
        fileSystem.Directory.Exists(cellDirPath).Should().BeTrue();
        fileSystem.File.Exists(cellPath).Should().BeTrue();
        
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
                            LastModified = 852,
                            BlockNumber = 6,
                        }
                    }
                },
                new CellBlock()
                {
                    LastModified = 456,
                    BlockNumber = 4,
                }
            });

        await SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
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
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Cells")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(oneFolder).Should().BeTrue();
        fileSystem.File.Exists(oneHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(fourFolder).Should().BeTrue();
        fileSystem.File.Exists(fourHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(subOneThreeFolder).Should().BeFalse();
        fileSystem.File.Exists(oneThreeHeaderDataPath).Should().BeFalse();

        fileSystem.Directory.Exists(subOneSixFolder).Should().BeTrue();
        fileSystem.File.Exists(oneSixHeaderDataPath).Should().BeTrue();
        
        fileSystem.Directory.Exists(cellDirPath).Should().BeFalse();
        fileSystem.File.Exists(cellPath).Should().BeFalse();
        
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
                            LastModified = 852,
                            BlockNumber = 6,
                        }
                    }
                }
            });

        await SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
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
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false);
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Cells")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(oneFolder).Should().BeTrue();
        fileSystem.File.Exists(oneHeaderDataPath).Should().BeTrue();

        fileSystem.Directory.Exists(fourFolder).Should().BeFalse();
        fileSystem.File.Exists(fourHeaderDataPath).Should().BeFalse();

        fileSystem.Directory.Exists(subOneThreeFolder).Should().BeFalse();
        fileSystem.File.Exists(oneThreeHeaderDataPath).Should().BeFalse();

        fileSystem.Directory.Exists(subOneSixFolder).Should().BeTrue();
        fileSystem.File.Exists(oneSixHeaderDataPath).Should().BeTrue();
        
        fileSystem.File.Exists(cellPath).Should().BeFalse();
    }
}