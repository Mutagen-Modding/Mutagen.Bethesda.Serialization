namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class ModKeyFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "ModKey",
        "Mutagen.Bethesda.Plugins.ModKey",
    };
    
    public ModKeyFieldGenerator()
        : base("ModKey", AssociatedTypes)
    {
    }
}