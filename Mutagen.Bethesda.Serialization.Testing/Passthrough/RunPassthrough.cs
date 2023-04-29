using System.IO.Abstractions;
using System.Reactive.Disposables;
using CommandLine;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public static class RunPassthrough
{
    public static async Task Run(string[] args, IPassthroughTest test)
    {
        await Parser.Default.ParseArguments<RunPassthroughCommand>(args)
            .WithParsedAsync<RunPassthroughCommand>(async o =>
            {
                await RunTest(test, o, new FileSystem());
            });
    }

    public static async Task RunTest(
        IPassthroughTest test,
        RunPassthroughCommand command,
        IFileSystem fileSystem)
    {
        using var disp = new CompositeDisposable();
        DirectoryPath dir;
        if (command.TestFolder.IsNullOrWhitespace())
        {
            var tmp = TempFolder.Factory(fileSystem: fileSystem);
            dir = tmp.Dir;
            disp.Add(tmp);
        }
        else
        {
            dir = command.TestFolder;
        }
        
        var rel = command.GameRelease.ToSkyrimRelease();

        Console.WriteLine($"Testing passthrough of {command.Path}");
        using var mod = SkyrimMod.CreateFromBinaryOverlay(command.Path, rel, fileSystem: fileSystem);
        
        var modKey = mod.ModKey;

        IWorkDropoff workDropoff = command.Parallel ? new ParallelWorkDropoff() : new InlineWorkDropoff();
        //IWorkQueue? workQueue = workDropoff as IWorkQueue;
        //IWorkConsumer? workConsumer = workQueue == null
        //    ? null
        //    : new WorkConsumer(new NumWorkThreadsUnopinionated(), workQueue);
        //workConsumer?.Start();

        var streamCreator = new InterceptionStreamCreator(new XxHashShortCircuitExporter());

        Console.WriteLine("Testing JSON");
        await PassthroughTest.PassThrough<ISkyrimModGetter>(
            fileSystem,
            Path.Combine(dir, "Json"),
            mod,
            (m, s) => test.JsonSerialize(m, s, workDropoff, streamCreator),
            s => test.JsonDeserialize(s, modKey, rel, workDropoff, streamCreator)
        );

        Console.WriteLine("Testing YAML");
        await PassthroughTest.PassThrough<ISkyrimModGetter>(
            fileSystem,
            Path.Combine(dir, "Yaml"),
            mod,
            (m, s) => test.YamlSerialize(m, s, workDropoff, streamCreator),
            s => test.YamlDeserialize(s, modKey, rel, workDropoff, streamCreator)
        );
    }
}