using Microsoft.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public record Names(
    string Direct,
    string Getter,
    string Setter);

public class LoquiNameRetriever
{
    public Names GetNames(ITypeSymbol typeSymbol)
    {
        var name = typeSymbol.Name;
        name = name.TrimStart('I');
        name = name.TrimEnd("Getter");
        return new Names(
            name,
            $"I{name}Getter",
            $"I{name}");
    }
}