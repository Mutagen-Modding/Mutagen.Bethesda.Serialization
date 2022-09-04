namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class RecordTypeFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "RecordType",
        "Mutagen.Bethesda.Plugins.RecordType",
    };
    
    public RecordTypeFieldGenerator()
        : base("RecordType", AssociatedTypes)
    {
    }
}