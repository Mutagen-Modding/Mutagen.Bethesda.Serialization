using FluentAssertions;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
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
        var metaData = new SerializationMetaData(GameRelease.SkyrimSE, new InlineWorkDropoff(), IFileSystemExt.DefaultFilesystem, NormalFileStreamCreator.Instance, CancellationToken.None);
        var a = await SkyrimModHeader_Serialization.Deserialize(dataWriter, kernel, metaData);
        a.Stats.NumRecords.Should().Be(0);
        a.Stats.NextFormID.Should().Be(0x800);
    }
}