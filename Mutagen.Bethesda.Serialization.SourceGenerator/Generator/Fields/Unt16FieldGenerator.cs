namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class UInt16FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "UInt16",
        "ushort"
    };
    
    public UInt16FieldGenerator()
        : base("UInt16", AssociatedTypes)
    {
    }
}