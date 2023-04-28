using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Serialization;

public record struct StreamPackage(Stream Stream, DirectoryPath? Path);