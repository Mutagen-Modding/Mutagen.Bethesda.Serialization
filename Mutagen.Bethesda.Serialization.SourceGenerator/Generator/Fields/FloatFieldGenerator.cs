namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class FloatFieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "float",
        "Single"
    };
    
    public FloatFieldGenerator()
        : base("Float", AssociatedTypes)
    {
    }
}