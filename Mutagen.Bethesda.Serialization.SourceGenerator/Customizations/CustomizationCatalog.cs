using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record Omission(string Name, ExpressionSyntax? Filter);

public record CustomizationCatalog(ITypeSymbol CustomizationClass, ITypeSymbol Target)
{
    public Dictionary<string, Omission> ToOmit = new();
}