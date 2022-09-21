using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public record CompilationUnit(Compilation Compilation, LoquiMapping Mapping);