using Microsoft.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public record Names(
    string Direct,
    string Getter,
    string Setter);

public class LoquiNameRetriever
{
    public Names GetNames(ITypeSymbol typeSymbol)
    {
        return GetNames(typeSymbol.Name);
    }
    
    public Names GetNames(string name)
    {
        if (name.StartsWith("I")
            && name.Length > 2
            && char.IsUpper(name[1]))
        {
            name = name.Substring(1);
        }
        name = name.TrimEnd("Getter");
        return new Names(
            name,
            $"I{name}Getter",
            $"I{name}");
    }
}