using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

[UsesVerify, Collection("Verify")]
public class XYBlockSerializationTests
{
    [Theory, DefaultAutoData]
    public async Task AddXYBlocksToWork(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(null!, existingDir, fileSystem);

        var group = new SkyrimGroup<Worldspace>(null!)
        {
            LastModified = 793
        };

        var mod = new SkyrimMod(ModKey.Null, SkyrimRelease.SkyrimSE);
        
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
        });

        var toDo = new List<Action>();
        
        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Worldspace>, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            group,
            "Worldspaces",
            static x => x.Records,
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel,JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace>(w, i, k, m),
            topRecordWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
        await TestHelper.VerifyFileSystem(fileSystem);
    }
    
    [Theory, DefaultAutoData]
    public async Task AddXYBlocksToWorkRemovesDeletedFiles(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var streamPackage = new StreamPackage(null!, existingDir, fileSystem);
        
        var mod = new SkyrimMod(ModKey.Null, SkyrimRelease.SkyrimSE);
        
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

        var group = new SkyrimGroup<Worldspace>(null!)
        {
            LastModified = 793
        };
        group.RecordCache.Set(ws);

        var toDo = new List<Action>();

        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Worldspace>, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            group,
            "Worldspaces",
            static x => x.Records,
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace>(w, i, k, m),
            topRecordWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
            toDo);

        toDo.ForEach(x => x());
        
        var headerDataPath = Path.Combine(existingDir, "Worldspaces", "MyWorldspace", "RecordData.json");
        var fourFiveFolder = Path.Combine(existingDir, "Worldspaces", "MyWorldspace", "4, 5");
        var fourSixFolder = Path.Combine(existingDir, "Worldspaces", "MyWorldspace", "4, 6");
        var fourFiveThreeNineFolder = Path.Combine(existingDir, "Worldspaces", "MyWorldspace", "4, 5", "3, 9");
        var fourFiveSixFourFolder = Path.Combine(existingDir, "Worldspaces", "MyWorldspace", "4, 5", "6, 4");
        var cellPath = Path.Combine(existingDir, "Worldspaces", "MyWorldspace", "4, 5", "6, 4", "TestCell.json");
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

        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Worldspace>, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            group,
            "Worldspaces",
            static x => x.Records,
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace>(w, i, k, m),
            topRecordWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
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

        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Worldspace>, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            group,
            "Worldspaces",
            static x => x.Records,
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace>(w, i, k, m),
            topRecordWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
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

        SerializationHelper.AddXYBlocksToWork<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, SkyrimGroup<Worldspace>, Worldspace, WorldspaceBlock, WorldspaceSubBlock, Cell>(
            streamPackage,
            group,
            "Worldspaces",
            static x => x.Records,
            static x => x.SubCells,
            static x => x.Items,
            static x => x.Items,
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            static x => new P2Int16(x.BlockNumberX, x.BlockNumberY),
            new SerializationMetaData(GameRelease.SkyrimSE),
            new MutagenSerializationWriterKernel<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(),
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.SkyrimGroup_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, Worldspace>(w, i, k, m),
            topRecordWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Worldspace_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.WorldspaceSubBlock_Serialization.SerializeFields<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Skyrim.Cell_Serialization.Serialize<NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit>(w, i, k, m),
            withNumbering: false,
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