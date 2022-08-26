namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class UInt64FieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "UInt64",
        "ulong"
    };
    
    public UInt64FieldGenerator()
        : base("UInt64", AssociatedTypes)
    {
    }
}