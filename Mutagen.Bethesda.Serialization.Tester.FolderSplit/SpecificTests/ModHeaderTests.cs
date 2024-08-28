using FluentAssertions;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Fallout4;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit.SpecificTests;

public class ModHeaderTests
{
    [Fact]
    public async Task Test()
    {
        var groupRecStreamPackage = new StreamPackage(File.OpenRead("Files/ModHeader.yaml"), null!);
        var kernel = new YamlSerializationReaderKernel();
        var dataWriter = kernel.GetNewObject(groupRecStreamPackage);
        var metaData = new SerializationMetaData(GameRelease.Fallout4, new InlineWorkDropoff(), IFileSystemExt.DefaultFilesystem, NormalFileStreamCreator.Instance, CancellationToken.None);
        var a = await Fallout4ModHeader_Serialization.Deserialize(dataWriter, kernel, metaData);
        a.Stats.NumRecords.Should().Be(0);
        a.Stats.NextFormID.Should().Be(0x0);
    }
}