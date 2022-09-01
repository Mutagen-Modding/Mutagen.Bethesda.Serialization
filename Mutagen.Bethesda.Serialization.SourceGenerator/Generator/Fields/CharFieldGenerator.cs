namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class CharFieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "char",
        "Char"
    };
    
    public CharFieldGenerator()
        : base("Char", AssociatedTypes)
    {
    }
}