using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;

public class CustomizationDriver
{
    private readonly IsGroupTester _isGroupTester;
    private readonly MajorRecordListFieldGenerator _majorRecordListFieldGenerator;

    public CustomizationDriver(
        IsGroupTester isGroupTester,
        MajorRecordListFieldGenerator majorRecordListFieldGenerator)
    {
        _isGroupTester = isGroupTester;
        _majorRecordListFieldGenerator = majorRecordListFieldGenerator;
    }

    public bool ShouldMakeParallel(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        PropertyCollection propertyCollection)
    {
        return compilation.Customization.Overall.FilePerRecord
            && propertyCollection.InOrder.Any(
                x =>
                {
                    var isTarget = _isGroupTester.IsGroup(x.Property.Type)
                        || _majorRecordListFieldGenerator.Applicable(
                            obj,
                            compilation,
                            x.Property.Type,
                            x.Property.Name,
                            isInsideCollection: false);
                    if (isTarget
                        && (compilation.Customization.TargetRecordSpecs?.EmbedRecordForProperty(x.Property) ?? false))
                    {
                        return false;
                    }

                    return isTarget;
                });
    }
    
    public void SerializationPreWork(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        StructuredStringBuilder sb,
        PropertyCollection propertyCollection)
    {
        if (ShouldMakeParallel(obj, compilation, propertyCollection))
        {
            sb.AppendLine("var tasks = new List<Task>();");
        }
    }
    
    public void SerializationPostWork(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        StructuredStringBuilder sb,
        PropertyCollection propertyCollection)
    {
        if (ShouldMakeParallel(obj, compilation, propertyCollection))
        {
            sb.AppendLine("await Task.WhenAll(tasks.ToArray());");
        }
    }
    
    public void DeserializationPreWork(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        StructuredStringBuilder sb,
        PropertyCollection propertyCollection)
    {
        if (ShouldMakeParallel(obj, compilation, propertyCollection))
        {
            sb.AppendLine("var tasks = new List<Task>();");
        }
    }
    
    public void DeserializationPostWork(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        StructuredStringBuilder sb,
        PropertyCollection propertyCollection)
    {
        if (ShouldMakeParallel(obj, compilation, propertyCollection))
        {
            sb.AppendLine("await Task.WhenAll(tasks.ToArray());");
        }
    }
    
    public void WrapOmission(
        CompilationUnit compilation,
        StructuredStringBuilder sb,
        LoquiTypeSet obj,
        PropertyMetadata property,
        Action toDo)
    {
        var name = property.Property.Name;
        if (compilation.Customization.Overall.OmitLastModifiedData
            && name == "LastModified")
        {
            return;
        }
        
        if (compilation.Customization.Overall.OmitTimestampData)
        {
            switch (name)
            {
                case "Timestamp":
                case "PersistentTimestamp":
                case "TemporaryTimestamp":
                case "SubCellsTimestamp":
                    return;
            }
        }

        if (compilation.Customization.Overall.OmitUnknownGroupData)
        {
            switch (name)
            {
                case "UnknownGroupData":
                case "PersistentUnknownGroupData":
                case "TemporaryUnknownGroupData":
                    return;
            }
        }

        if (compilation.Customization.Overall.OmitUnusedConditionDataFields)
        {
            var objName = obj.GetAny().Name;
            if (objName.Contains("ConditionData", StringComparison.Ordinal)
                && name.Contains("unused", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        if (compilation.Customization.TargetRecordSpecs == null)
        {
            toDo();
            return;
        }

        if (compilation.Customization.TargetRecordSpecs.ToOmit == null
            || !compilation.Customization.TargetRecordSpecs.ToOmit.TryGetValue(name, out _))
        {
            toDo();
            return;
        }
        
        // ToDo
        // Add filter lambdas
    }
}