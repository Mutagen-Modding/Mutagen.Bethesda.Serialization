namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FloatFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "float",
        "Single"
    };
    
    public FloatFieldGenerator()
        : base("Float", AssociatedTypes)
    {
    }
}