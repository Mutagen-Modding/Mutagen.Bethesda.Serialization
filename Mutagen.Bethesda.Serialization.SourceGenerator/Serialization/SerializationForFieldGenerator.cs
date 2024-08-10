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
        var gen = GetGenerator(obj, compilation, fieldType, fieldName);
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
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType} for field {fieldName}\");");
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
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType} for field {fieldName}\");");
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
        var gen = GetGenerator(obj, compilation, fieldType, fieldName);
        if (gen != null)
        {
            gen.GenerateForDeserializeSingleFieldInto(compilation,
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
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType} for field {fieldName}\");");
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
            gen.GenerateForDeserializeSingleFieldInto(compilation,
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
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType} for field {fieldName}\");");
        }
    }
    
    public void GenerateForDeserializeSection(
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
            gen.GenerateForDeserializeSection(compilation,
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
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType} for field {fieldName}\");");
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
        var gen = GetGenerator(obj, compilation, fieldType, fieldName);
        if (gen != null)
        {
            gen.GenerateForHasSerialize(compilation, obj, fieldType, fieldName, fieldAccessor, defaultValueAccessor, "metaData", sb, cancel);
        }
        else
        {
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType} for field {fieldName}\");");
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
            sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {fieldType} for field {fieldName}\");");
        }
    }

    public ISerializationForFieldGenerator? GetGenerator(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol fieldType, 
        string? fieldName)
    {
        compilation.Context.CancellationToken.ThrowIfCancellationRequested();
        if (_fieldGeneratorDict.TryGetValue(fieldType.ToString(), out var gen))
        {
            return gen;
        }
        else
        {
            foreach (var fieldGenerator in _variableFieldGenerators)
            {
                compilation.Context.CancellationToken.ThrowIfCancellationRequested();
                if (fieldGenerator.Applicable(obj, compilation.Customization, fieldType, fieldName))
                {
                    return fieldGenerator;
                }
            }
        }

        return default;
    }
}