namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class Int16FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "short",
        "Int16"
    };
    
    public Int16FieldGenerator()
        : base("Int16", AssociatedTypes)
    {
    }
}