namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class Int64FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "long",
        "Int64"
    };
    
    public Int64FieldGenerator()
        : base("Int64", AssociatedTypes)
    {
    }
}