using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Fallout4;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit.SpecificTests;

public class VirtualMachineAdapterTests
{
    [Fact]
    public async Task Test()
    {
        var groupRecStreamPackage = new StreamPackage(File.OpenRead("Files/VirtualMachineAdapterTestData.json"), null!);
        var kernel = new NewtonsoftJsonSerializationReaderKernel();
        var dataWriter = kernel.GetNewObject(groupRecStreamPackage);
        var metaData = new SerializationMetaData(GameRelease.Fallout4, new InlineWorkDropoff(), IFileSystemExt.DefaultFilesystem, NormalFileStreamCreator.Instance, CancellationToken.None);
        var a = await Activator_Serialization.Deserialize(dataWriter, kernel, metaData);
        // ToDo
        // Maybe add specific property checks
        // Was just added as there was an exception
    }
}