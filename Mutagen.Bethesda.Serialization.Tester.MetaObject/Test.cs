using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Testing.Passthrough;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Fallout4;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tester.MetaObject;

public class Test : IPassthroughTest
{
    public async Task JsonSerialize(
        IFallout4ModGetter modGetter, 
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

    public async Task<IFallout4ModGetter> JsonDeserialize(
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

        meta.String.ShouldBe("Hello");
        meta.Number.ShouldBe(23);

        return ret;
    }

    public async Task YamlSerialize(
        IFallout4ModGetter modGetter,
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

    public async Task<IFallout4ModGetter> YamlDeserialize(
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