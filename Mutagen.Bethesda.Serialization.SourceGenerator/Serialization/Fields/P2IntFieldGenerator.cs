namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class P2IntFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P2Int",
        "Noggog.P2Int",
    };
    
    public P2IntFieldGenerator()
        : base("P2Int", AssociatedTypes)
    {
    }
}