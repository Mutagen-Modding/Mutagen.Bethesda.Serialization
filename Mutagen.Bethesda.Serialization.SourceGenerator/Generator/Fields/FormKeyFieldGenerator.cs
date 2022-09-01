namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class FormKeyFieldGenerator : PrimitiveFieldGenerator
{
    public static readonly string[] AssociatedTypes = new string[]
    {
        "FormKey",
        "Mutagen.Bethesda.Plugins.FormKey",
    };
    
    public FormKeyFieldGenerator()
        : base("FormKey", AssociatedTypes)
    {
    }
}