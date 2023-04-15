using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Noggog;

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
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir, fileSystem);

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

        var toDo = new List<Action>();
        
        SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
            streamPackage,
            mod.Cells,
            "Cells",
            static x => x.Records,
            static x => x.SubBlocks,
            static x => x.Cells,
            static x => x.BlockNumber,
            static x => x.BlockNumber,
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Cells")).Should().BeTrue();
        var headerDataPath = Path.Combine(existingDir, "Cells", "GroupRecordData.json");
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);

        var oneFolder = Path.Combine(existingDir, "Cells", "1");
        fileSystem.Directory.Exists(oneFolder).Should().BeTrue();
        var oneHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "GroupRecordData.json");
        fileSystem.File.Exists(oneHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneHeaderDataPath), settings);

        var fourFolder = Path.Combine(existingDir, "Cells", "4");
        fileSystem.Directory.Exists(fourFolder).Should().BeTrue();
        var fourHeaderDataPath = Path.Combine(existingDir, "Cells", "4", "GroupRecordData.json");
        fileSystem.File.Exists(fourHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_4_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourHeaderDataPath), settings);

        var subOneThreeFolder = Path.Combine(existingDir, "Cells", "1", "3");
        fileSystem.Directory.Exists(subOneThreeFolder).Should().BeTrue();
        var oneThreeHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "3", "GroupRecordData.json");
        fileSystem.File.Exists(oneThreeHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_3_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneThreeHeaderDataPath), settings);

        var subOneSixFolder = Path.Combine(existingDir, "Cells", "1", "6");
        fileSystem.Directory.Exists(subOneSixFolder).Should().BeTrue();
        var oneSixHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "6", "GroupRecordData.json");
        fileSystem.File.Exists(oneSixHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(oneSixHeaderDataPath), settings);
        
        var cellPath = Path.Combine(existingDir, "Cells", "1", "6", "TestCell.json");
        fileSystem.File.Exists(cellPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_1_6_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(cellPath), settings);
    }
    
    [Theory, TestAutoData]
    public async Task AddBlocksToWorkRemovesDeletedRecords(
        IFileSystem fileSystem,
        FilePath someFile,
        SkyrimMod mod,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir, fileSystem);

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

        var toDo = new List<Action>();
        
        SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
            streamPackage,
            mod.Cells,
            "Cells",
            static x => x.Records,
            static x => x.SubBlocks,
            static x => x.Cells,
            static x => x.BlockNumber,
            static x => x.BlockNumber,
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
        var headerDataPath = Path.Combine(existingDir, "Cells", "GroupRecordData.json");
        var oneFolder = Path.Combine(existingDir, "Cells", "1");
        var oneHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "GroupRecordData.json");
        var fourFolder = Path.Combine(existingDir, "Cells", "4");
        var fourHeaderDataPath = Path.Combine(existingDir, "Cells", "4", "GroupRecordData.json");
        var subOneThreeFolder = Path.Combine(existingDir, "Cells", "1", "3");
        var oneThreeHeaderDataPath = Path.Combine(existingDir, "Cells", "1", "3", "GroupRecordData.json");
        var subOneSixFolder = Path.Combine(existingDir, "Cells", "1", "6");
        var cellPath = Path.Combine(existingDir, "Cells", "1", "6", "TestCell.json");
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
        
        toDo.Clear();
        SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
            streamPackage,
            mod.Cells,
            "Cells",
            static x => x.Records,
            static x => x.SubBlocks,
            static x => x.Cells,
            static x => x.BlockNumber,
            static x => x.BlockNumber,
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
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
        
        toDo.Clear();
        SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
            streamPackage,
            mod.Cells,
            "Cells",
            static x => x.Records,
            static x => x.SubBlocks,
            static x => x.Cells,
            static x => x.BlockNumber,
            static x => x.BlockNumber,
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
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
        
        toDo.Clear();
        SerializationHelper.AddBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimListGroup<CellBlock>, CellBlock, CellSubBlock, Cell>(
            streamPackage,
            mod.Cells,
            "Cells",
            static x => x.Records,
            static x => x.SubBlocks,
            static x => x.Cells,
            static x => x.BlockNumber,
            static x => x.BlockNumber,
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimListGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.CellSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
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