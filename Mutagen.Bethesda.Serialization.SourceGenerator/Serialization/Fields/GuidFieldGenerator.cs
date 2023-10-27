namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class GuidFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "System.Guid",
    };
    
    public GuidFieldGenerator()
        : base("Guid", AssociatedTypes)
    {
    }
}