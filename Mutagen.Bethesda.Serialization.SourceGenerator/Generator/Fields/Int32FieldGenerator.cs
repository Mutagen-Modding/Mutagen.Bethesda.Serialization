namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class Int32FieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "int",
        "Int32"
    };
    
    public Int32FieldGenerator()
        : base("Int32", AssociatedTypes)
    {
    }
}