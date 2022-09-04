namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class Int8FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "sbyte",
        "SByte",
        "Int8",
    };
    
    public Int8FieldGenerator()
        : base("Int8", AssociatedTypes)
    {
    }
}