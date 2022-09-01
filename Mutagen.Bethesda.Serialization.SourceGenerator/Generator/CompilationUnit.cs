using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public record CompilationUnit(Compilation Compilation, LoquiMapping Mapping);