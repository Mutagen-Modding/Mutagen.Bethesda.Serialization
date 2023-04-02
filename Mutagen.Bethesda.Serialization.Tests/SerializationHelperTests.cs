using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Tests;

[UsesVerify]
public class SerializationHelperTests
{
    [Theory, TestAutoData]
    public async Task AddGroupToWork(
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
        
        SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
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
        settings.UseFileName($"{nameof(AddGroupToWork)}_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);
        settings.UseFileName($"{nameof(AddGroupToWork)}_TestEdid.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath1), settings);
        settings.UseFileName($"{nameof(AddGroupToWork)}_123457_Skyrim.esm.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(npcPath2), settings);
    }
    
    [Theory, TestAutoData]
    public async Task AddGroupToWorkRemovesDeletedRecords(
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
        
        SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
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
        
        SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            toDo);

        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.File.Exists(npcPath1).Should().BeTrue();
        fileSystem.File.Exists(npcPath2).Should().BeFalse();
        
        // Remove all
        group.Clear();
        toDo.Clear();
        
        SerializationHelper.WriteFolderPerRecord<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Npc>, Npc>(
            streamPackage,
            group,
            "Npcs",
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, INpcGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Npc_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            toDo);

        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Npcs")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.File.Exists(npcPath1).Should().BeFalse();
        fileSystem.File.Exists(npcPath2).Should().BeFalse();
    }
    
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
    
    [Theory, TestAutoData]
    public async Task AddXYBlocksToWork(
        IFileSystem fileSystem,
        FilePath someFile,
        SkyrimMod mod,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir, fileSystem);

        var ws = new Worldspace(mod)
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
                                    EditorID = "TestCell"
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
        };

        var toDo = new List<Action>();
        
        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            ws,
            "Worldspaces",
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            toDo);

        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces")).Should().BeTrue();
        var headerDataPath = Path.Combine(existingDir, "Worldspaces", "RecordData.json");
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        var settings = new VerifySettings();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_RecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(headerDataPath), settings);
        
        var fourFiveFolder = Path.Combine(existingDir, "Worldspaces", "4, 5");
        fileSystem.Directory.Exists(fourFiveFolder).Should().BeTrue();
        var fourFiveHeaderDataPath = Path.Combine(fourFiveFolder, "GroupRecordData.json");
        fileSystem.File.Exists(fourFiveHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourFiveHeaderDataPath), settings);

        var fourSixFolder = Path.Combine(existingDir, "Worldspaces", "4, 6");
        fileSystem.Directory.Exists(fourSixFolder).Should().BeTrue();
        var fourSixHeaderDataPath = Path.Combine(fourSixFolder, "GroupRecordData.json");
        fileSystem.File.Exists(fourSixHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_6_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourSixHeaderDataPath), settings);

        var fourFiveThreeNineFolder = Path.Combine(existingDir, "Worldspaces", "4, 5", "3, 9");
        fileSystem.Directory.Exists(fourFiveThreeNineFolder).Should().BeTrue();
        var fourFiveThreeNineHeaderDataPath = Path.Combine(fourFiveThreeNineFolder, "GroupRecordData.json");
        fileSystem.File.Exists(fourFiveThreeNineHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_3_9_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourFiveThreeNineHeaderDataPath), settings);

        var fourFiveSixFourFolder = Path.Combine(existingDir, "Worldspaces", "4, 5", "6, 4");
        fileSystem.Directory.Exists(fourFiveSixFourFolder).Should().BeTrue();
        var fourFiveSixFourHeaderDataPath = Path.Combine(fourFiveSixFourFolder, "GroupRecordData.json");
        fileSystem.File.Exists(fourFiveSixFourHeaderDataPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddXYBlocksToWork)}_4_5_6_4_GroupRecordData.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(fourFiveSixFourHeaderDataPath), settings);

        var cellPath = Path.Combine(existingDir, "Worldspaces", "4, 5", "6, 4", "TestCell.json");
        fileSystem.File.Exists(cellPath).Should().BeTrue();
        settings.UseFileName($"{nameof(AddBlocksToWork)}_4_5_6_4_TestCell.json");
        await Verifier.Verify(fileSystem.File.ReadAllText(cellPath), settings);
    }
    
    [Theory, TestAutoData]
    public async Task AddXYBlocksToWorkRemovesDeletedFiles(
        IFileSystem fileSystem,
        FilePath someFile,
        SkyrimMod mod,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(fileSystem.File.Create(someFile), existingDir, fileSystem);

        var ws = new Worldspace(mod)
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
                                    EditorID = "TestCell"
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
        };

        var toDo = new List<Action>();
        
        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            ws,
            "Worldspaces",
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            toDo);

        toDo.ForEach(x => x());
        
        var headerDataPath = Path.Combine(existingDir, "Worldspaces", "RecordData.json");
        var fourFiveFolder = Path.Combine(existingDir, "Worldspaces", "4, 5");
        var fourSixFolder = Path.Combine(existingDir, "Worldspaces", "4, 6");
        var fourFiveThreeNineFolder = Path.Combine(existingDir, "Worldspaces", "4, 5", "3, 9");
        var fourFiveSixFourFolder = Path.Combine(existingDir, "Worldspaces", "4, 5", "6, 4");
        var cellPath = Path.Combine(existingDir, "Worldspaces", "4, 5", "6, 4", "TestCell.json");
        var fourFiveHeaderDataPath = Path.Combine(fourFiveFolder, "GroupRecordData.json");
        var fourSixHeaderDataPath = Path.Combine(fourSixFolder, "GroupRecordData.json");
        var fourFiveSixFourHeaderDataPath = Path.Combine(fourFiveSixFourFolder, "GroupRecordData.json");
        var fourFiveThreeNineHeaderDataPath = Path.Combine(fourFiveThreeNineFolder, "GroupRecordData.json");
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourSixFolder).Should().BeTrue();
        fileSystem.File.Exists(fourSixHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveThreeNineFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveThreeNineHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveSixFourFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveSixFourHeaderDataPath).Should().BeTrue();
        fileSystem.File.Exists(cellPath).Should().BeTrue();
        
        ws.SubCells.SetTo(
            new ExtendedList<WorldspaceBlock>()
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
                        }
                    }
                },
                new WorldspaceBlock()
                {
                    LastModified = 456,
                    BlockNumberX = 4,
                    BlockNumberY = 6,
                }
            });

        toDo.Clear();
        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            ws,
            "Worldspaces",
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            toDo);
        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourSixFolder).Should().BeTrue();
        fileSystem.File.Exists(fourSixHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveThreeNineFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveThreeNineHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveSixFourFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveSixFourHeaderDataPath).Should().BeTrue();
        fileSystem.File.Exists(cellPath).Should().BeFalse();
        
        ws.SubCells.SetTo(
            new ExtendedList<WorldspaceBlock>()
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
                    }
                },
                new WorldspaceBlock()
                {
                    LastModified = 456,
                    BlockNumberX = 4,
                    BlockNumberY = 6,
                }
            });

        toDo.Clear();
        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            ws,
            "Worldspaces",
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            toDo);
        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourSixFolder).Should().BeTrue();
        fileSystem.File.Exists(fourSixHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveThreeNineFolder).Should().BeTrue();
        fileSystem.File.Exists(fourFiveThreeNineHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveSixFourFolder).Should().BeFalse();
        fileSystem.File.Exists(fourFiveSixFourHeaderDataPath).Should().BeFalse();
        fileSystem.File.Exists(cellPath).Should().BeFalse();;
        
        ws.SubCells.SetTo(
            new ExtendedList<WorldspaceBlock>()
            {
                new WorldspaceBlock()
                {
                    LastModified = 456,
                    BlockNumberX = 4,
                    BlockNumberY = 6,
                }
            });

        toDo.Clear();
        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            ws,
            "Worldspaces",
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            toDo);
        toDo.ForEach(x => x());
        
        fileSystem.Directory.Exists(Path.Combine(existingDir, "Worldspaces")).Should().BeTrue();
        fileSystem.File.Exists(headerDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveFolder).Should().BeFalse();
        fileSystem.File.Exists(fourFiveHeaderDataPath).Should().BeFalse();
        fileSystem.Directory.Exists(fourSixFolder).Should().BeTrue();
        fileSystem.File.Exists(fourSixHeaderDataPath).Should().BeTrue();
        fileSystem.Directory.Exists(fourFiveThreeNineFolder).Should().BeFalse();
        fileSystem.File.Exists(fourFiveThreeNineHeaderDataPath).Should().BeFalse();
        fileSystem.Directory.Exists(fourFiveSixFourFolder).Should().BeFalse();
        fileSystem.File.Exists(fourFiveSixFourHeaderDataPath).Should().BeFalse();
        fileSystem.File.Exists(cellPath).Should().BeFalse();
    }
}