using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record Omission(string Name, ExpressionSyntax? Filter);

public record CustomizationCatalog(ITypeSymbol CustomizationClass, LoquiTypeSet Target)
{
    public Dictionary<string, Omission> ToOmit = new();
}