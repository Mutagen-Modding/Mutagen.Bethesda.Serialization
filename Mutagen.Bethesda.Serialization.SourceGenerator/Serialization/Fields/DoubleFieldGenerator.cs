namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class DoubleFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "double",
    };
    
    public DoubleFieldGenerator()
        : base("Double", AssociatedTypes)
    {
    }
}