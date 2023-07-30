namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class TimeOnlyFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "System.TimeOnly",
        "TimeOnly",
    };
    
    public TimeOnlyFieldGenerator() 
        : base("TimeOnly", AssociatedTypes)
    {
    }
}