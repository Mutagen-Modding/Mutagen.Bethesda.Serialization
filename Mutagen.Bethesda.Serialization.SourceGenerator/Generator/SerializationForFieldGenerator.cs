using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationFieldGenerator
{
    private readonly Dictionary<string, ISerializationForFieldGenerator> _fieldGeneratorDict = new();
    private readonly ISerializationForFieldGenerator[] _variableFieldGenerators;

    public SerializationFieldGenerator(ISerializationForFieldGenerator[] fieldGenerators)
    {
        _variableFieldGenerators = fieldGenerators
            .Where(x => !x.AssociatedTypes.Any())
            .ToArray();
        foreach (var f in fieldGenerators)
        {
            foreach (var associatedType in f.AssociatedTypes)
            {
                _fieldGeneratorDict[associatedType] = f;
            }
        }
    }
    
    public void GenerateForField(
        Compilation compilation,
        ITypeSymbol obj,
        ITypeSymbol fieldType,
        string writerAccessor,
        string? fieldName,
        string fieldAccessor, 
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (_fieldGeneratorDict.TryGetValue(fieldType.ToString(), out var gen))
        {
            gen.GenerateForSerialize(compilation, obj, fieldType, fieldName, fieldAccessor, writerAccessor, "kernel", sb, cancel);
        }
        else
        {
            foreach (var fieldGenerator in _variableFieldGenerators)
            {
                cancel.ThrowIfCancellationRequested();
                if (fieldGenerator.Applicable(fieldType))
                {
                    fieldGenerator.GenerateForSerialize(compilation, obj, fieldType, fieldName, fieldAccessor, writerAccessor, "kernel", sb, cancel);
                    return;
                }
            }
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType}\");");
        }
    }
}