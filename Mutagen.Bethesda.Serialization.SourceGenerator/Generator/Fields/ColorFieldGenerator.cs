namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class ColorFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "Color",
        "System.Drawing.Color",
    };
    
    public ColorFieldGenerator()
        : base("Color", AssociatedTypes)
    {
    }
}