namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class P3FloatFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P3Float",
        "Noggog.P3Float",
    };
    
    public P3FloatFieldGenerator()
        : base("P3Float", AssociatedTypes)
    {
    }
}