namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class P3UInt16FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P3UInt16",
        "Noggog.P3UInt16",
    };
    
    public P3UInt16FieldGenerator()
        : base("P3UInt16", AssociatedTypes)
    {
    }
}