namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class Int64FieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "long",
        "Int64"
    };
    
    public Int64FieldGenerator()
        : base("Int64", AssociatedTypes)
    {
    }
}