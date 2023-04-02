using System.IO.Abstractions;

namespace Noggog.Verify.FileAbstractions;

public static class VerifyFileAbstractions
{
    public static void Enable()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterFileConverter<IFileSystem>(Convert);
    }
    
    static ConversionResult Convert(IFileSystem target, IReadOnlyDictionary<string, object> context)
    {
        var targets = new List<Target>();
        foreach (var drive in target.Directory.GetLogicalDrives())
        {
            foreach (var file in target.Directory.GetFiles(drive, "*", SearchOption.AllDirectories))
            {
                targets.Add(FileToTarget(target, file));
            }
        }

        return new(null, targets);
    }

    static Target FileToTarget(IFileSystem fileSystem, string filePath)
    {
        var data = fileSystem.File.ReadAllText(filePath);
        return new(Path.GetExtension(filePath).TrimStart('.'), data, Path.GetFileNameWithoutExtension(filePath));
    }
}