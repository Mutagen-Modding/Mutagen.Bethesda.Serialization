using System.IO.Abstractions;
using System.Reactive.Disposables;
using CommandLine;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Tester;
using Mutagen.Bethesda.Serialization.Testing;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;

Parser.Default.ParseArguments<RunPassthroughCommand>(args)
    .WithParsed<RunPassthroughCommand>(o =>
    {
        using var disp = new CompositeDisposable();
        DirectoryPath dir;
        if (o.TestFolder.IsNullOrWhitespace())
        {
            var tmp = TempFolder.Factory();
            dir = tmp.Dir;
            disp.Add(tmp);
        }
        else
        {
            dir = o.TestFolder;
        }
        
        var rel = o.GameRelease.ToSkyrimRelease();

        using var mod = SkyrimMod.CreateFromBinaryOverlay(o.Path, rel);
        
        var modKey = mod.ModKey;

        PassthroughTest.PassThrough<ISkyrimModGetter>(
            new FileSystem(),
            Path.Combine(dir, "Json"),
            mod,
            (m, s) => MutagenJsonConverter.Instance.Serialize(m, s),
            s => MutagenJsonConverter.Instance.Deserialize(s, modKey, rel)
        );
        
        PassthroughTest.PassThrough<ISkyrimModGetter>(
            new FileSystem(),
            Path.Combine(dir, "Yaml"),
            mod,
            (m, s) => MutagenYamlConverter.Instance.Serialize(m, s),
            s => MutagenYamlConverter.Instance.Deserialize(s, modKey, rel)
        );
    });