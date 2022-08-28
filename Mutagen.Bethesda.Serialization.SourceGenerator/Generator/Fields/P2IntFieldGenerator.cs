namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class P2IntFieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "P2Int",
        "Noggog.P2Int",
    };
    
    public P2IntFieldGenerator()
        : base("PInt", AssociatedTypes)
    {
    }
}