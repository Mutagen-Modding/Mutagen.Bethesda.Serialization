namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class StringFieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "string",
        "String"
    };
    
    public StringFieldGenerator()
        : base("String", AssociatedTypes)
    {
    }
}