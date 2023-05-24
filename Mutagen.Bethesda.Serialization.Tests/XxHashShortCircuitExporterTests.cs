using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class XxHashShortCircuitExporterTests
{
    private byte[] GetBytes()
    {
        byte[] data = new byte[10000];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)i;
        }

        return data;
    }
    
    [Theory, DefaultAutoData]
    public async Task BlocksDuplicateExport(
        IFileSystem fileSystem,
        FilePath filePath,
        XxHashShortCircuitExporter sut)
    {
        var memStream = new MemoryStream(GetBytes());
        fileSystem.File.Exists(filePath).Should().BeFalse();
        var result = await sut.WriteOut(memStream, fileSystem, filePath, CancellationToken.None);
        result.Should().BeTrue();
        fileSystem.File.Exists(filePath).Should().BeTrue();
        var writeTime = fileSystem.File.GetLastWriteTime(filePath);
        memStream.Position = 0;
        result = await sut.WriteOut(memStream, fileSystem, filePath, CancellationToken.None);
        result.Should().BeFalse();
        fileSystem.File.GetLastWriteTime(filePath).Should().Be(writeTime);
    }
    
    [Theory, DefaultAutoData]
    public async Task OverwritesNonMatchingHash(
        IFileSystem fileSystem,
        FilePath existingFilePath,
        XxHashShortCircuitExporter sut)
    {
        var hashPath = XxHashShortCircuitExporter.GetHashPath(existingFilePath);
        await fileSystem.File.WriteAllTextAsync(hashPath, "Gibberish");
        fileSystem.File.Exists(existingFilePath).Should().BeTrue();
        var writeTime = fileSystem.File.GetLastWriteTime(existingFilePath);
        
        var memStream = new MemoryStream(GetBytes());
        var result = await sut.WriteOut(memStream, fileSystem, existingFilePath, CancellationToken.None);
        result.Should().BeTrue();
        fileSystem.File.GetLastWriteTime(existingFilePath).Should().BeAfter(writeTime);
    }
    
    [Theory, DefaultAutoData]
    public async Task OverwritesMissingHash(
        IFileSystem fileSystem,
        FilePath existingFilePath,
        XxHashShortCircuitExporter sut)
    {
        fileSystem.File.Exists(existingFilePath).Should().BeTrue();
        var writeTime = fileSystem.File.GetLastWriteTime(existingFilePath);
        
        var memStream = new MemoryStream(GetBytes());
        var result = await sut.WriteOut(memStream, fileSystem, existingFilePath, CancellationToken.None);
        result.Should().BeTrue();
        fileSystem.File.GetLastWriteTime(existingFilePath).Should().BeAfter(writeTime);
    }
}