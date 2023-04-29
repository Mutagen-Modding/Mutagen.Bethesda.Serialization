using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit.SpecificTests;

public class VirtualMachineAdapterTests
{
    [Fact]
    public void Test()
    {
        var groupRecStreamPackage = new StreamPackage(File.OpenRead("Files/VirtualMachineAdapterTestData.json"), null!);
        var kernel = new NewtonsoftJsonSerializationReaderKernel();
        var dataWriter = kernel.GetNewObject(groupRecStreamPackage);
        var metaData = new SerializationMetaData(GameRelease.SkyrimSE, new InlineWorkDropoff(), IFileSystemExt.DefaultFilesystem, NormalFileStreamCreator.Instance);
        var a = Activator_Serialization.Deserialize(dataWriter, kernel, metaData);
        // ToDo
        // Maybe add specific property checks
        // Was just added as there was an exception
    }
}