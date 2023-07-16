using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Testing.Passthrough;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.NoCustomization;

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
            Path.Combine(dir, modGetter.ModKey.ToString()),
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
    }

    public async Task<ISkyrimModGetter> JsonDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff, 
        IFileSystem fileSystem,
        ICreateStream streamCreator)
    {
        return await MutagenJsonConverter.Instance.Deserialize(
            Path.Combine(dir, modKey.ToString()),
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
            Path.Combine(dir, modGetter.ModKey.ToString()),
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
    }

    public async Task<ISkyrimModGetter> YamlDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem,
        ICreateStream streamCreator)
    {
        return await MutagenYamlConverter.Instance.Deserialize(
            Path.Combine(dir, modKey.ToString()),
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
    }
}