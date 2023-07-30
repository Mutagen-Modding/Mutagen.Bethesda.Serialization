namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class DateOnlyFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "DateOnly",
    };
    
    public DateOnlyFieldGenerator() 
        : base("DateOnly", AssociatedTypes)
    {
    }
}