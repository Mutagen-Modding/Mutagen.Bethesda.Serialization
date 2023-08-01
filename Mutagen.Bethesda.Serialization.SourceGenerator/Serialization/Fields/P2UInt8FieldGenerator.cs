namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class P2UInt8FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P2UInt8",
        "Noggog.P2UInt8",
    };
    
    public P2UInt8FieldGenerator()
        : base("P2UInt8", AssociatedTypes)
    {
    }
}