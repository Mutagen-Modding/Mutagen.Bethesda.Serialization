using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;
using Noggog;

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
        LoquiTypeSet obj, 
        ITypeSymbol typeSymbol,
        string? fieldName,
        CompilationUnit compilation)
    {
        if (!compilation.Customization.Overall.FilePerRecord) return false;
        RecordCustomizationSpecifications? targetRecordCustomizations = null;
        if (compilation.Mapping.TryGetTypeSet(typeSymbol, out var loqui)
            && compilation.Customization.RecordSpecs.TryGetValue(loqui, out targetRecordCustomizations))
        {
        }
        foreach (var prop in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (targetRecordCustomizations != null
                && targetRecordCustomizations.EmbedRecordForProperty(prop)) return false;
            if (_isMajorRecordTester.IsMajorRecord(prop.Type)) return true;
            if (_majorRecordListFieldGenerator.Applicable(obj, compilation, prop.Type, prop.Name, false))
            {
                return true;
            }
        }

        return false;
    }
}