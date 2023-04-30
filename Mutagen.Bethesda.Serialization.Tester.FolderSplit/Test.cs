using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Testing.Passthrough;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

public class Test : IPassthroughTest
{
    public async Task JsonSerialize(
        ISkyrimModGetter modGetter,
        DirectoryPath dir,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator)
    {
        await MutagenJsonConverter.Instance.Serialize(
            modGetter,
            dir,
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
    }

    public async Task<ISkyrimModGetter> JsonDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        SkyrimRelease release, 
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator)
    {
        return await MutagenJsonConverter.Instance.Deserialize(
            dir,
            modKey,
            release,
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
    }

    public async Task YamlSerialize(
        ISkyrimModGetter modGetter,
        DirectoryPath dir,
        IWorkDropoff workDropoff, 
        IFileSystem fileSystem, 
        ICreateStream streamCreator)
    {
        await MutagenYamlConverter.Instance.Serialize(
            modGetter,
            dir,
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
    }

    public async Task<ISkyrimModGetter> YamlDeserialize(
        DirectoryPath dir,
        ModKey modKey, 
        SkyrimRelease release,
        IWorkDropoff workDropoff, 
        IFileSystem fileSystem, 
        ICreateStream streamCreator)
    {
        return await MutagenYamlConverter.Instance.Deserialize(
            dir,
            modKey,
            release,
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
    }
}