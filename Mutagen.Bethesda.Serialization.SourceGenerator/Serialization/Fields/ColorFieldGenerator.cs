using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class ColorFieldGenerator : PrimitiveFieldGenerator
{
    public override IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "System.Drawing";
    }

    public new static readonly string[] AssociatedTypes = new string[]
    {
        "Color",
        "System.Drawing.Color",
    };
    
    public ColorFieldGenerator()
        : base("Color", AssociatedTypes)
    {
    }
}