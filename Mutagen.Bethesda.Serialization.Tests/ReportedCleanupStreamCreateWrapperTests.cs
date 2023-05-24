using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class ReportedCleanupStreamCreateWrapperTests
{
    [Theory, DefaultAutoData]
    public void TypicalCleanup(
        IFileSystem fileSystem,
        DirectoryPath existingDir,
        ICreateStream createStream)
    {
        var file1 = Path.Combine(existingDir, "File1.esp");
        var subDir = Path.Combine(existingDir, "SubDir");
        var file2 = Path.Combine(existingDir, "SubDir", "File2.esp");
        var subDir2 = Path.Combine(existingDir, "SubDir2");
        var file3 = Path.Combine(existingDir, "SubDir2", "File3.esp");
        var file4 = Path.Combine(existingDir, "SubDir2", "File4.esp");
        var subDir3 = Path.Combine(existingDir, "SubDir3");
        fileSystem.File.WriteAllText(file1, string.Empty);
        fileSystem.Directory.CreateDirectory(subDir);
        fileSystem.File.WriteAllText(file2, string.Empty);
        fileSystem.Directory.CreateDirectory(subDir2);
        fileSystem.File.WriteAllText(file3, string.Empty);
        fileSystem.File.WriteAllText(file4, string.Empty);
        fileSystem.Directory.CreateDirectory(subDir3);

        var sut = new ReportedCleanupStreamCreateWrapper(fileSystem, existingDir, createStream);
        sut.GetStreamFor(fileSystem, file2, write: true);
        sut.GetStreamFor(fileSystem, file3, write: true);
        sut.Dispose();

        fileSystem.File.Exists(file1).Should().BeFalse();
        fileSystem.File.Exists(file2).Should().BeTrue();
        fileSystem.File.Exists(file3).Should().BeTrue();
        fileSystem.File.Exists(file4).Should().BeFalse();
        fileSystem.Directory.Exists(subDir3).Should().BeFalse();
    }
}