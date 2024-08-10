using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public record Omission(string Name, ExpressionSyntax? Filter);

public record RecordCustomizationSpecifications(ITypeSymbol CustomizationClass, LoquiTypeSet Target)
{
    public Dictionary<string, Omission>? ToOmit;
    public HashSet<string>? ToEmbedRecordsInSameFile;
}