using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FormKeyFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "FormKey",
        "Mutagen.Bethesda.Plugins.FormKey",
    };
    
    public override IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Mutagen.Bethesda.Plugins";
    }

    public FormKeyFieldGenerator()
        : base("FormKey", AssociatedTypes)
    {
    }
}