using System.IO.Abstractions;
using System.Reactive.Disposables;
using CommandLine;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public static class RunPassthrough
{
    public static void Run(string[] args, IPassthroughTest test)
    {
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

                Console.WriteLine($"Testing passthrough of {o.Path}");
                using var mod = SkyrimMod.CreateFromBinaryOverlay(o.Path, rel);
        
                var modKey = mod.ModKey;

                Console.WriteLine("Testing JSON");
                PassthroughTest.PassThrough<ISkyrimModGetter>(
                    new FileSystem(),
                    Path.Combine(dir, "Json"),
                    mod,
                    (m, s) => test.JsonSerialize(m, s),
                    s => test.JsonDeserialize(s, modKey, rel)
                );

                Console.WriteLine("Testing YAML");
                PassthroughTest.PassThrough<ISkyrimModGetter>(
                    new FileSystem(),
                    Path.Combine(dir, "Yaml"),
                    mod,
                    (m, s) => test.YamlSerialize(m, s),
                    s => test.YamlDeserialize(s, modKey, rel)
                );
            });
    }
}