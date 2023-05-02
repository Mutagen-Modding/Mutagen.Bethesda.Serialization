using CommandLine;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

[Verb("run-passthrough")]
public class RunPassthroughCommand
{
    [Option('p', "Path", Required = true)] 
    public string Path { get; set; } = string.Empty;

    [Option('g', "GameRelease", Required = true)]
    public GameRelease GameRelease { get; set; }
    
    [Option('t', "TestFolder")]
    public string? TestFolder { get; set; }

    [Option('l', "Parallel")]
    public bool Parallel { get; set; }
}