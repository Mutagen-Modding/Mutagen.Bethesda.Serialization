namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class BoolFieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "bool",
        "Boolean"
    };
    
    public BoolFieldGenerator()
        : base("Bool", AssociatedTypes)
    {
    }
}