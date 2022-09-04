namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class P3UInt8FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P3UInt8",
        "Noggog.P3UInt8",
    };
    
    public P3UInt8FieldGenerator()
        : base("P3UInt8", AssociatedTypes)
    {
    }
}