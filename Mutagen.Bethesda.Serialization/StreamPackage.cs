using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Serialization;

public record StreamPackage(Stream Stream, DirectoryPath? Path, IFileSystem FileSystem);