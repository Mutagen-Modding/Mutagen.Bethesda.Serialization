using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class ColorFieldGenerator : PrimitiveFieldGenerator
{
    public override IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
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