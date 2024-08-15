using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class ShouldSkipDuringSerializationTester
{
    public IsGroupTester GroupTester { get; }

    public ShouldSkipDuringSerializationTester(
        IsGroupTester groupTester)
    {
        GroupTester = groupTester;
    }
    
    public bool ShouldSkip(
        CustomizationSpecifications customization,
        LoquiTypeSet obj, 
        string? fieldName)
    {
        if (GroupTester.IsGroup(obj.Getter)) return true;
        if (customization.FilePerRecord)
        {
            if (obj.Direct == null) return false;
            if (obj.Direct.Name.Equals("CellBlock")) return true;
            if (obj.Direct.Name.Equals("CellSubBlock")) return true;
            if (obj.Direct.Name.Equals("WorldspaceBlock")) return true;
            if (obj.Direct.Name.Equals("WorldspaceSubBlock")) return true;
            if (obj.Direct.Name == "Worldspace" && fieldName == "SubCells") return true;
        }
        return false;
    }
}