namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class PercentFieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "Percent",
        "Noggog.Percent",
    };
    
    public PercentFieldGenerator()
        : base("Percent", AssociatedTypes)
    {
    }
}