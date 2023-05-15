using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Testing.Passthrough;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.MetaObject;

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
            streamCreator: streamCreator,
            extraMeta: new Meta()
            {
                String = "Hello",
                Number = 23,
            });
    }

    public async Task<ISkyrimModGetter> JsonDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff, 
        IFileSystem fileSystem,
        ICreateStream streamCreator)
    {
        var meta = new Meta();
        var ret = await MutagenJsonConverter.Instance.Deserialize(
            Path.Combine(dir, modKey.ToString()),
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator,
            extraMeta: meta);

        meta.String.Should().Be("Hello");
        meta.Number.Should().Be(23);

        return ret;
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
            streamCreator: streamCreator,
            extraMeta: new Meta()
            {
                String = "Hello",
                Number = 23,
            });
    }

    public async Task<ISkyrimModGetter> YamlDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem,
        ICreateStream streamCreator)
    {
        var meta = new Meta();
        var ret = await MutagenYamlConverter.Instance.Deserialize(
            Path.Combine(dir, modKey.ToString()),
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator,
            extraMeta: meta);

        meta.String.Should().Be("Hello");
        meta.Number.Should().Be(23);

        return ret;
    }
}