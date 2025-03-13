using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.Extensions;
using Shouldly;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.UtilityTests;

public class MajorRecordListParallelHelperTests
{
    [Theory, DefaultAutoData]
    public async Task WriteWithNoFieldName(
        MutagenSerializationWriterKernel<YamlSerializationWriterKernel, YamlWritingUnit> kernel,
        SerializationMetaData meta,
        IReadOnlyList<INpcGetter> list)
    {
        var memStream = new MemoryStream();
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await SerializationHelper.WriteMajorRecordList<YamlSerializationWriterKernel, YamlWritingUnit, INpcGetter>(
                new StreamPackage(memStream, null),
                null,
                list,
                meta,
                kernel,
                async (w, o, k, m) =>
                {
                },
                withNumbering: true);
        });
    }
    
    [Theory, DefaultAutoData]
    public async Task WriteNothingShouldNotMakeFolder(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath dir,
        MutagenSerializationWriterKernel<YamlSerializationWriterKernel, YamlWritingUnit> kernel,
        SerializationMetaData meta)
    {
        var memStream = new MemoryStream();
        await SerializationHelper.WriteMajorRecordList<YamlSerializationWriterKernel, YamlWritingUnit, INpcGetter>(
            new StreamPackage(memStream, dir.Path),
            fieldName,
            Array.Empty<INpcGetter>(),
            meta,
            kernel,
            async (w, o, k, m) =>
            {
                throw new NotImplementedException();
            },
            withNumbering: true);
        fileSystem.Directory.Exists(dir).ShouldBeFalse();
    }
    
    [Theory, MutagenAutoData]
    public async Task WriteTypical(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath dir,
        FormKey f1,
        FormKey f2,
        FormKey f3,
        string e1,
        string e2,
        string e3,
        MutagenSerializationWriterKernel<YamlSerializationWriterKernel, YamlWritingUnit> kernel)
    {
        var memStream = new MemoryStream();
        var npcs = new[]
        {
            new Npc(f1, SkyrimRelease.SkyrimSE)
            {
                EditorID = e1,
                Height = 1,
            },
            new Npc(f2, SkyrimRelease.SkyrimSE)
            {
                EditorID = e2,
                Height = 2,
            },
            new Npc(f3, SkyrimRelease.SkyrimSE)
            {
                EditorID = e3,
                Height = 3,
            },
        };
        await SerializationHelper.WriteMajorRecordList<YamlSerializationWriterKernel, YamlWritingUnit, INpcGetter>(
            new StreamPackage(memStream, dir.Path),
            fieldName,
            npcs,
            new SerializationMetaData(GameRelease.SkyrimSE, null, fileSystem, null, CancellationToken.None),
            kernel,
            async (w, o, k, m) =>
            {
                k.WriteFloat(w, "Height", o.Height, 0);
            },
            withNumbering: false);
        fileSystem.Directory.Exists(dir).ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"{e1} - {f1.ToFilesafeString()}.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"{e2} - {f2.ToFilesafeString()}.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"{e3} - {f3.ToFilesafeString()}.yaml"))
            .ShouldBeTrue();
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"{e1} - {f1.ToFilesafeString()}.yaml"))
            .ShouldBe($"Height: 1{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"{e2} - {f2.ToFilesafeString()}.yaml"))
            .ShouldBe($"Height: 2{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"{e3} - {f3.ToFilesafeString()}.yaml"))
            .ShouldBe($"Height: 3{Environment.NewLine}");
    }
    
    [Theory, MutagenAutoData]
    public async Task WriteWithNumbers(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath dir,
        FormKey f1,
        FormKey f2,
        FormKey f3,
        string e1,
        string e2,
        string e3,
        MutagenSerializationWriterKernel<YamlSerializationWriterKernel, YamlWritingUnit> kernel)
    {
        var memStream = new MemoryStream();
        var npcs = new[]
        {
            new Npc(f1, SkyrimRelease.SkyrimSE)
            {
                EditorID = e1,
                Height = 1,
            },
            new Npc(f2, SkyrimRelease.SkyrimSE)
            {
                EditorID = e2,
                Height = 2,
            },
            new Npc(f3, SkyrimRelease.SkyrimSE)
            {
                EditorID = e3,
                Height = 3,
            },
        };
        await SerializationHelper.WriteMajorRecordList<YamlSerializationWriterKernel, YamlWritingUnit, INpcGetter>(
            new StreamPackage(memStream, dir.Path),
            fieldName,
            npcs,
            new SerializationMetaData(GameRelease.SkyrimSE, null, fileSystem, null, CancellationToken.None),
            kernel,
            async (w, o, k, m) =>
            {
                k.WriteFloat(w, "Height", o.Height, 0);
            },
            withNumbering: true);
        fileSystem.Directory.Exists(dir).ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"[0] {e1} - {f1.ToFilesafeString()}.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"[1] {e2} - {f2.ToFilesafeString()}.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"[2] {e3} - {f3.ToFilesafeString()}.yaml"))
            .ShouldBeTrue();
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"[0] {e1} - {f1.ToFilesafeString()}.yaml"))
            .ShouldBe($"Height: 1{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"[1] {e2} - {f2.ToFilesafeString()}.yaml"))
            .ShouldBe($"Height: 2{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"[2] {e3} - {f3.ToFilesafeString()}.yaml"))
            .ShouldBe($"Height: 3{Environment.NewLine}");
    }
    
    [Theory, MutagenAutoData]
    public async Task WriteEachInFolder(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath dir,
        FormKey f1,
        FormKey f2,
        FormKey f3,
        string e1,
        string e2,
        string e3,
        MutagenSerializationWriterKernel<YamlSerializationWriterKernel, YamlWritingUnit> kernel)
    {
        var memStream = new MemoryStream();
        var npcs = new[]
        {
            new Npc(f1, SkyrimRelease.SkyrimSE)
            {
                EditorID = e1,
                Height = 1,
            },
            new Npc(f2, SkyrimRelease.SkyrimSE)
            {
                EditorID = e2,
                Height = 2,
            },
            new Npc(f3, SkyrimRelease.SkyrimSE)
            {
                EditorID = e3,
                Height = 3,
            },
        };
        await SerializationHelper.WriteMajorRecordList<YamlSerializationWriterKernel, YamlWritingUnit, INpcGetter>(
            new StreamPackage(memStream, dir.Path),
            fieldName,
            npcs,
            new SerializationMetaData(GameRelease.SkyrimSE, null, fileSystem, null, CancellationToken.None),
            kernel,
            async (w, o, k, m) =>
            {
                k.WriteFloat(w, "Height", o.Height, 0);
            },
            withNumbering: false,
            eachRecordInFolder: true);
        fileSystem.Directory.Exists(dir).ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"{e1} - {f1.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"{e2} - {f2.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"{e3} - {f3.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBeTrue();
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"{e1} - {f1.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBe($"Height: 1{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"{e2} - {f2.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBe($"Height: 2{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"{e3} - {f3.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBe($"Height: 3{Environment.NewLine}");
    }
    
    [Theory, MutagenAutoData]
    public async Task WriteEachInFolderWithNumbering(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath dir,
        FormKey f1,
        FormKey f2,
        FormKey f3,
        string e1,
        string e2,
        string e3,
        MutagenSerializationWriterKernel<YamlSerializationWriterKernel, YamlWritingUnit> kernel)
    {
        var memStream = new MemoryStream();
        var npcs = new[]
        {
            new Npc(f1, SkyrimRelease.SkyrimSE)
            {
                EditorID = e1,
                Height = 1,
            },
            new Npc(f2, SkyrimRelease.SkyrimSE)
            {
                EditorID = e2,
                Height = 2,
            },
            new Npc(f3, SkyrimRelease.SkyrimSE)
            {
                EditorID = e3,
                Height = 3,
            },
        };
        await SerializationHelper.WriteMajorRecordList<YamlSerializationWriterKernel, YamlWritingUnit, INpcGetter>(
            new StreamPackage(memStream, dir.Path),
            fieldName,
            npcs,
            new SerializationMetaData(GameRelease.SkyrimSE, null, fileSystem, null, CancellationToken.None),
            kernel,
            async (w, o, k, m) =>
            {
                k.WriteFloat(w, "Height", o.Height, 0);
            },
            withNumbering: true,
            eachRecordInFolder: true);
        fileSystem.Directory.Exists(dir).ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"[0] {e1} - {f1.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"[1] {e2} - {f2.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(dir, fieldName, $"[2] {e3} - {f3.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBeTrue();
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"[0] {e1} - {f1.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBe($"Height: 1{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"[1] {e2} - {f2.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBe($"Height: 2{Environment.NewLine}");
        fileSystem.File.ReadAllText(Path.Combine(dir, fieldName, $"[2] {e3} - {f3.ToFilesafeString()}", $"RecordData.yaml"))
            .ShouldBe($"Height: 3{Environment.NewLine}");
    }

    [Theory, MutagenAutoData]
    public async Task ReadTypical(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath existingDir,
        FormKey f1,
        FormKey f2,
        string e1,
        string e2,
        YamlSerializationReaderKernel readerKernel)
    {
        List<Npc> npcs = new();

        fileSystem.Directory.CreateDirectory(Path.Combine(existingDir, fieldName));
        fileSystem.File.WriteAllText(Path.Combine(existingDir, fieldName, $"{e1}.yaml"), 
            $"""
             FormKey: {f1}
             Height: 1
             """);
        fileSystem.File.WriteAllText(Path.Combine(existingDir, fieldName, $"{e2}.yaml"), 
            $"""
             FormKey: {f2}
             Height: 2
             """);

        var expectedStreamPackagePath = Path.Combine(existingDir, fieldName);
        
        await SerializationHelper.ReadMajorRecordList<YamlSerializationReaderKernel, YamlReadingUnit, Npc>(
            new StreamPackage(null!, existingDir),
            fieldName,
            npcs,
            new SerializationMetaData(GameRelease.SkyrimSE, null, fileSystem, null, CancellationToken.None),
            readerKernel,
            async (r, k, m) =>
            {
                r.StreamPackage.Path!.Value.Path.ShouldBe(expectedStreamPackagePath);
                k.ReadString(r).ShouldBe("FormKey");
                var formKey = k.ReadString(r);
                k.ReadString(r).ShouldBe("Height");
                var height = k.ReadFloat(r);
                return new Npc(FormKey.Factory(formKey), SkyrimRelease.SkyrimSE)
                {
                    Height = height ?? throw new ArgumentException()
                };
            },
            eachRecordInFolder: false);

        npcs.Select(x => x.FormKey)
            .ShouldEqual(f1, f2);
        npcs.Select(x => x.Height)
            .ShouldEqual(1, 2);
    }

    [Theory, MutagenAutoData]
    public async Task ReadTypicalNumbered(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath existingDir,
        FormKey f1,
        FormKey f2,
        string e1,
        string e2,
        YamlSerializationReaderKernel readerKernel)
    {
        List<Npc> npcs = new();

        fileSystem.Directory.CreateDirectory(Path.Combine(existingDir, fieldName));
        fileSystem.File.WriteAllText(Path.Combine(existingDir, fieldName, $"[0] {e1}.yaml"), 
            $"""
             FormKey: {f1}
             Height: 1
             """);
        fileSystem.File.WriteAllText(Path.Combine(existingDir, fieldName, $"[1] {e2}.yaml"), 
            $"""
             FormKey: {f2}
             Height: 2
             """);

        var expectedStreamPackagePath = Path.Combine(existingDir, fieldName);

        await SerializationHelper.ReadMajorRecordList<YamlSerializationReaderKernel, YamlReadingUnit, Npc>(
            new StreamPackage(null!, existingDir),
            fieldName,
            npcs,
            new SerializationMetaData(GameRelease.SkyrimSE, null, fileSystem, null, CancellationToken.None),
            readerKernel,
            async (r, k, m) =>
            {
                r.StreamPackage.Path!.Value.Path.ShouldBe(expectedStreamPackagePath);
                k.ReadString(r).ShouldBe("FormKey");
                var formKey = k.ReadString(r);
                k.ReadString(r).ShouldBe("Height");
                var height = k.ReadFloat(r);
                return new Npc(FormKey.Factory(formKey), SkyrimRelease.SkyrimSE)
                {
                    Height = height ?? throw new ArgumentException()
                };
            },
            eachRecordInFolder: false);

        npcs.Select(x => x.FormKey)
            .ShouldEqual(f1, f2);
        npcs.Select(x => x.Height)
            .ShouldEqual(1, 2);
    }

    [Theory, MutagenAutoData]
    public async Task ReadTypicalFolderPerRecord(
        IFileSystem fileSystem,
        string fieldName,
        DirectoryPath existingDir,
        FormKey f1,
        FormKey f2,
        string e1,
        string e2,
        YamlSerializationReaderKernel readerKernel)
    {
        List<Npc> npcs = new();

        fileSystem.Directory.CreateDirectory(Path.Combine(existingDir, fieldName, e1));
        fileSystem.Directory.CreateDirectory(Path.Combine(existingDir, fieldName, e2));
        fileSystem.File.WriteAllText(Path.Combine(existingDir, fieldName, e1, $"RecordData.yaml"), 
            $"""
             FormKey: {f1}
             Height: 1
             """);
        fileSystem.File.WriteAllText(Path.Combine(existingDir, fieldName, e2, $"RecordData.yaml"), 
            $"""
             FormKey: {f2}
             Height: 2
             """);

        await SerializationHelper.ReadMajorRecordList<YamlSerializationReaderKernel, YamlReadingUnit, Npc>(
            new StreamPackage(null!, existingDir),
            fieldName,
            npcs,
            new SerializationMetaData(GameRelease.SkyrimSE, null, fileSystem, null, CancellationToken.None),
            readerKernel,
            async (r, k, m) =>
            {
                k.ReadString(r).ShouldBe("FormKey");
                var formKey = k.ReadString(r);
                k.ReadString(r).ShouldBe("Height");
                var height = k.ReadFloat(r);
                if (formKey == f1.ToString())
                {
                    r.StreamPackage.Path!.Value.Path.ShouldBe(Path.Combine(existingDir.Path, fieldName, e1));
                }
                else
                {
                    r.StreamPackage.Path!.Value.Path.ShouldBe(Path.Combine(existingDir.Path, fieldName, e2));
                }
                return new Npc(FormKey.Factory(formKey), SkyrimRelease.SkyrimSE)
                {
                    Height = height ?? throw new ArgumentException()
                };
            },
            eachRecordInFolder: true);

        npcs.Select(x => x.FormKey)
            .ShouldEqual(f1, f2);
        npcs.Select(x => x.Height)
            .ShouldEqual(1, 2);
    }
}