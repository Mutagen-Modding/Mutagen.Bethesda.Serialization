using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Serialization.Yaml;
using Noggog;
using Noggog.IO;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public abstract class AExtractMetaTests<TWriterKernal, TWriterObj, TReaderKernel, TReaderObj>
    : ASerializationTest<TWriterKernal, TWriterObj, TReaderKernel, TReaderObj>
    where TWriterKernal : ISerializationWriterKernel<TWriterObj>, new()
    where TReaderKernel : ISerializationReaderKernel<TReaderObj>, new()
{
    public record MetaObj(int Integer, bool Boolean, string String);

    private static readonly GameRelease Release = GameRelease.SkyrimSE;

    private void WriteSpriggitMeta(TWriterObj writerObj, ModKey? modKey)
    {
        WriterKernel.WriteEnum<GameRelease>(writerObj, "GameRelease", Release, null, checkDefaults: false);
        WriterKernel.WriteString(writerObj, "ModKey", modKey?.ToString(), null, checkDefaults: true);
    }
    
    [Theory, DefaultAutoData]
    public void WithModKey(
        IFileSystem fileSystem,
        FilePath filePath,
        ModKey modKey)
    {
        using (GetWriterObj(fileSystem, filePath, out var writer))
        {
            WriteSpriggitMeta(writer, modKey);
        }
        
        using var disp = GetReaderObj(fileSystem, filePath, out var reader);
        SerializationHelper.ExtractMeta<TReaderObj, object>(
            fileSystem,
            filePath.Path,
            filePath.Path,
            new NormalFileStreamCreator(),
            ReaderKernel, null, null, 
            CancellationToken.None,
            out var readModKey, out var readGameRelease);
        readModKey.Should().Be(modKey);
        readGameRelease.Should().Be(Release);
    }
    
    [Theory, DefaultAutoData]
    public void NoModKey(
        IFileSystem fileSystem,
        FilePath filePath,
        ModKey modKey)
    {
        using (GetWriterObj(fileSystem, filePath, out var writer))
        {
            WriteSpriggitMeta(writer, null);
        }
        
        using var disp = GetReaderObj(fileSystem, filePath, out var reader);
        Assert.Throws<MalformedDataException>(() =>
        {
            SerializationHelper.ExtractMeta<TReaderObj, object>(
                fileSystem,
                filePath.Path,
                filePath.Path,
                new NormalFileStreamCreator(),
                ReaderKernel, null, null, 
                CancellationToken.None,
                out var readModKey, out var readGameRelease);
        });
    }
    
    [Theory, DefaultAutoData]
    public void ModKeyFromFilename(
        IFileSystem fileSystem,
        DirectoryPath existingDir,
        ModKey modKey)
    {
        var filePath = Path.Combine(existingDir, modKey.ToString());
        using (GetWriterObj(fileSystem, filePath, out var writer))
        {
            WriteSpriggitMeta(writer, null);
        }
        
        using var disp = GetReaderObj(fileSystem, filePath, out var reader);
        
        SerializationHelper.ExtractMeta<TReaderObj, object>(
            fileSystem,
            filePath,
            filePath,
            new NormalFileStreamCreator(),
            ReaderKernel, null, null, 
            CancellationToken.None,
            out var readModKey, out var readGameRelease);
        readModKey.Should().Be(modKey);
        readGameRelease.Should().Be(Release);
    }
    
    [Theory, DefaultAutoData]
    public void NoMeta(
        IFileSystem fileSystem,
        DirectoryPath existingDir,
        ModKey modKey)
    {
        var filePath = Path.Combine(existingDir, modKey.ToString());
        using (GetWriterObj(fileSystem, filePath, out var writer))
        {
        }
        
        using var disp = GetReaderObj(fileSystem, filePath, out var reader);
        Assert.Throws<MalformedDataException>(() =>
        {
            SerializationHelper.ExtractMeta<TReaderObj, object>(
                fileSystem,
                filePath,
                filePath,
                new NormalFileStreamCreator(),
                ReaderKernel, null, null, 
                CancellationToken.None,
                out var readModKey, 
                out var readGameRelease);
        });
    }
    
    [Theory, DefaultAutoData]
    public void DataBeforeFileName(
        IFileSystem fileSystem,
        FilePath filePath,
        ModKey modKey)
    {
        using (GetWriterObj(fileSystem, filePath, out var writer))
        {
            WriterKernel.WriteString(writer, "SomeField", "SomeData", null);
            WriteSpriggitMeta(writer, modKey);
        }
        
        using var disp = GetReaderObj(fileSystem, filePath, out var reader);
        SerializationHelper.ExtractMeta<TReaderObj, object>(
            fileSystem,
            filePath.Path,
            filePath.Path,
            new NormalFileStreamCreator(),
            ReaderKernel, null, null, 
            CancellationToken.None,
            out var readModKey, out var readGameRelease);
        readModKey.Should().Be(modKey);
        readGameRelease.Should().Be(Release);
    }
    
    [Theory, DefaultAutoData]
    public void LoquiDataBeforeMeta(
        IFileSystem fileSystem,
        FilePath filePath,
        ModKey modKey)
    {
        using (GetWriterObj(fileSystem, filePath, out var writer))
        {
            WriterKernel.WriteLoqui(writer, "SomeNestedObj",
                new MetaObj(1, true, "Hello"),
                new SerializationMetaData(Release),
                static (obj, metaObj, kernel, data) =>
                {
                    kernel.WriteInt32(obj, nameof(MetaObj.Integer), metaObj.Integer, default);
                    kernel.WriteBool(obj, nameof(MetaObj.Boolean), metaObj.Boolean, default);
                    kernel.WriteString(obj, nameof(MetaObj.String), metaObj.String, default);
                });
            WriterKernel.WriteString(writer, "SomeField", "SomeData", null);
            WriteSpriggitMeta(writer, modKey);
        }
        
        using var disp = GetReaderObj(fileSystem, filePath, out var reader);
        SerializationHelper.ExtractMeta<TReaderObj, object>(
            fileSystem,
            filePath.Path,
            filePath.Path,
            new NormalFileStreamCreator(),
            ReaderKernel, null, null, 
            CancellationToken.None,
            out var readModKey, out var readGameRelease);
        readModKey.Should().Be(modKey);
        readGameRelease.Should().Be(Release);
    }
}

public class JsonExtractMetaTests : AExtractMetaTests<
    NewtonsoftJsonSerializationWriterKernel, JsonWritingUnit, 
    NewtonsoftJsonSerializationReaderKernel, JsonReadingUnit>
{
}

public class YamlExtractMetaTests : AExtractMetaTests<
    YamlSerializationWriterKernel, YamlWritingUnit, 
    YamlSerializationReaderKernel, YamlReadingUnit>
{
}