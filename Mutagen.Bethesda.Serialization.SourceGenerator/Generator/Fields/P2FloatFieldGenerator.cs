namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class P2FloatFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P2Float",
        "Noggog.P2Float",
    };
    
    public P2FloatFieldGenerator()
        : base("P2Float", AssociatedTypes)
    {
    }
}