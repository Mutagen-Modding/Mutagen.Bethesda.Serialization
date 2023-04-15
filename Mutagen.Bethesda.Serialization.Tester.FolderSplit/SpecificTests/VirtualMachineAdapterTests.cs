using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit.SpecificTests;

public class VirtualMachineAdapterTests
{
    [Fact]
    public void Test()
    {
        var groupRecStreamPackage = new StreamPackage(File.OpenRead("Files/VirtualMachineAdapterTestData.json"), null!, IFileSystemExt.DefaultFilesystem);
        var kernel = new NewtonsoftJsonSerializationReaderKernel();
        var dataWriter = kernel.GetNewObject(groupRecStreamPackage);
        var a = Activator_Serialization.Deserialize(dataWriter, kernel, new SerializationMetaData(GameRelease.SkyrimSE));
        // ToDo
        // Maybe add specific property checks
        // Was just added as there was an exception
    }
}