using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class ObjRequiresFolderTester
{
    private readonly IsMajorRecordTester _isMajorRecordTester;
    private readonly MajorRecordListFieldGenerator _majorRecordListFieldGenerator;

    public ObjRequiresFolderTester(
        IsMajorRecordTester isMajorRecordTester,
        MajorRecordListFieldGenerator majorRecordListFieldGenerator)
    {
        _isMajorRecordTester = isMajorRecordTester;
        _majorRecordListFieldGenerator = majorRecordListFieldGenerator;
    }

    public bool ObjRequiresFolder(
        LoquiTypeSet obj, ITypeSymbol typeSymbol,
        string? fieldName, CustomizationCatalog customization)
    {
        if (!customization.Overall.FilePerRecord) return false;
        foreach (var prop in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (customization.EmbedRecordForProperty(prop)) continue;
            if (_isMajorRecordTester.IsMajorRecord(prop.Type)) return true;
            if (_majorRecordListFieldGenerator.Applicable(obj, customization, prop.Type, prop.Name, false))
            {
                return true;
            }
        }

        return false;
    }
}