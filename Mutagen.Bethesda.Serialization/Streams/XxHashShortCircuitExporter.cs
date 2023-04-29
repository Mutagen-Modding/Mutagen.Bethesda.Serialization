using System.Data.HashFunction.xxHash;
using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Streams;

public class XxHashShortCircuitExporter : IExporter
{
    public static readonly IxxHash _hash = xxHashFactory.Instance.Create();

    public static FilePath GetHashPath(FilePath path)
    {
        return Path.Combine(
            Path.GetDirectoryName(path)!,
            $"{Path.GetFileNameWithoutExtension(path)}.xxhash");
    }
    
    public async Task<bool> WriteOut(Stream stream, IFileSystem fileSystem, FilePath path,
        CancellationToken cancel)
    {
        var hash = _hash.ComputeHash(stream, cancel);
        var hashString = hash.AsHexString();
        var hashPath = GetHashPath(path);
        if (fileSystem.File.Exists(path)
            && fileSystem.File.Exists(hashPath))
        {
            var info = fileSystem.FileInfo.New(hashPath);
            if (info.Length == 8)
            {
                var existingHashText = await fileSystem.File.ReadAllTextAsync(hashPath);
                if (existingHashText.Equals(hashString))
                {
                    return false;
                }
            }
        }

        if (fileSystem.File.Exists(hashPath))
        {
            fileSystem.File.Delete(hashPath);
        }
        
        stream.Position = 0;
        using (var fileOut = fileSystem.File.Create(path))
        {
            await stream.CopyToAsync(fileOut);
        }
        await fileSystem.File.WriteAllTextAsync(hashPath, hashString);
        return true;
    }
}