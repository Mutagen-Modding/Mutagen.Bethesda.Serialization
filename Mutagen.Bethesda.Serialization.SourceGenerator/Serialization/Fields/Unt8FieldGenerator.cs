namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class UInt8FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "byte",
        "Byte",
        "UInt8"
    };
    
    public UInt8FieldGenerator()
        : base("UInt8", AssociatedTypes)
    {
    }
}