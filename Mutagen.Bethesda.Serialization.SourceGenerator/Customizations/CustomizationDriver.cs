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
                x => _isGroupTester.IsGroup(x.Property.Type)
                    || _majorRecordListFieldGenerator.Applicable(
                        obj,
                        compilation.Customization.Overall, 
                        x.Property.Type,
                        x.Property.Name));
    }
    
    public void SerializationPreWork(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        StructuredStringBuilder sb,
        PropertyCollection propertyCollection)
    {
        if (ShouldMakeParallel(obj, compilation, propertyCollection))
        {
            sb.AppendLine("List<Action> parallelToDo = new List<Action>();");
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
            sb.AppendLine("Parallel.Invoke(parallelToDo.ToArray());");
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
            sb.AppendLine("List<Action> parallelToDo = new List<Action>();");
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
            sb.AppendLine("Parallel.Invoke(parallelToDo.ToArray());");
        }
    }
    
    public void WrapOmission(
        CompilationUnit compilation,
        StructuredStringBuilder sb, 
        PropertyMetadata property,
        Action toDo)
    {
        if (compilation.Customization.RecordSpecs == null)
        {
            toDo();
            return;
        }

        var name = property.Property.Name;

        if (!compilation.Customization.RecordSpecs.ToOmit.TryGetValue(name, out var omission))
        {
            toDo();
            return;
        }
        
        // ToDo
        // Add filter lambdas
    }
}