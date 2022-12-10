using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

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
    
    public void GenerateSerializeForField(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol fieldType,
        string writerAccessor,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string kernelAccessor,
        string metaDataAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var gen = GetGenerator(fieldType, cancel);
        if (gen != null)
        {
            gen.GenerateForSerialize(compilation,
                obj,
                fieldType,
                fieldName,
                fieldAccessor,
                defaultValueAccessor,
                writerAccessor,
                kernelAccessor,
                metaDataAccessor,
                insideCollection: true,
                sb,
                cancel);
        }
        else
        {
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType}\");");
        }
    }
    
    public void GenerateSerializeForField(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol fieldType,
        string writerAccessor,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        ISerializationForFieldGenerator? gen,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (gen != null)
        {
            gen.GenerateForSerialize(compilation,
                obj,
                fieldType,
                fieldName,
                fieldAccessor,
                defaultValueAccessor,
                writerAccessor,
                "kernel",
                "metaData",
                insideCollection: false,
                sb,
                cancel);
        }
        else
        {
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType}\");");
        }
    }
    
    public void GenerateDeserializeForField(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol fieldType,
        string readerAccessor,
        string? fieldName,
        string kernelAccessor,
        string metaDataAccessor,
        string fieldAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var gen = GetGenerator(fieldType, cancel);
        if (gen != null)
        {
            gen.GenerateForDeserialize(compilation,
                obj,
                fieldType,
                fieldName,
                fieldAccessor,
                readerAccessor,
                kernelAccessor,
                metaDataAccessor,
                insideCollection: true,
                canSet: false,
                sb,
                cancel);
        }
        else
        {
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType}\");");
        }
    }
    
    public void GenerateDeserializeForField(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol fieldType,
        string readerAccessor,
        string? fieldName,
        string fieldAccessor,
        bool canSet,
        ISerializationForFieldGenerator? gen,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (gen != null)
        {
            gen.GenerateForDeserialize(compilation,
                obj,
                fieldType,
                fieldName,
                fieldAccessor,
                readerAccessor,
                "kernel",
                "metaData",
                insideCollection: false,
                canSet: canSet,
                sb,
                cancel);
        }
        else
        {
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType}\");");
        }
    }
    
    public void GenerateHasSerializeForField(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol fieldType,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var gen = GetGenerator(fieldType, cancel);
        if (gen != null)
        {
            gen.GenerateForHasSerialize(compilation, obj, fieldType, fieldName, fieldAccessor, defaultValueAccessor, "metaData", sb, cancel);
        }
        else
        {
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType}\");");
        }
    }
    
    public void GenerateHasSerializeForField(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol fieldType,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        ISerializationForFieldGenerator? gen,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (gen != null)
        {
            gen.GenerateForHasSerialize(compilation, obj, fieldType, fieldName, fieldAccessor, defaultValueAccessor, "metaData", sb, cancel);
        }
        else
        {
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType}\");");
        }
    }

    public ISerializationForFieldGenerator? GetGenerator(
        ITypeSymbol fieldType,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (_fieldGeneratorDict.TryGetValue(fieldType.ToString(), out var gen))
        {
            return gen;
        }
        else
        {
            foreach (var fieldGenerator in _variableFieldGenerators)
            {
                cancel.ThrowIfCancellationRequested();
                if (fieldGenerator.Applicable(fieldType))
                {
                    return fieldGenerator;
                }
            }
        }

        return default;
    }
}