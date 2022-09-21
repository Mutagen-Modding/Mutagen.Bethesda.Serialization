namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class PercentFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "Percent",
        "Noggog.Percent",
    };
    
    public PercentFieldGenerator()
        : base("Percent", AssociatedTypes)
    {
    }
}