using Noggog;

namespace Mutagen.Bethesda.Serialization.Streams;

public record struct StreamPackage(Stream Stream, DirectoryPath? Path);