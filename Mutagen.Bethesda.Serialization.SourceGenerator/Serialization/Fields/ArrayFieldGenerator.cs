using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class ArrayFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<ListFieldGenerator>> _listGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public ArrayFieldGenerator(Func<IOwned<ListFieldGenerator>> listGenerator)
    {
        _listGenerator = listGenerator;
    }
    
    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
        => Enumerable.Empty<string>();
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            return arr.ElementType.Name != "Byte";
        }

        return false;
    }

    public bool HasVariableHasSerialize => true;

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor, 
        string metaAccessor,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count > 0) return true;");
    }

    public void GenerateForSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field, 
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        _listGenerator().Value.GenerateForSerialize(
            compilation: compilation,
            obj: obj, 
            field: field,
            fieldName: fieldName, 
            fieldAccessor: fieldAccessor,
            defaultValueAccessor: defaultValueAccessor, 
            writerAccessor: writerAccessor,
            kernelAccessor: kernelAccessor, 
            metaAccessor: metaAccessor,
            sb: sb,
            cancel: cancel);
    }

    public void GenerateForDeserialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string readerAccessor,
        string kernelAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        _listGenerator().Value.GenerateForDeserialize(
            compilation: compilation,
            obj: obj, 
            field: field,
            fieldName: fieldName, 
            fieldAccessor: fieldAccessor,
            readerAccessor: readerAccessor,
            kernelAccessor: kernelAccessor, 
            metaAccessor: metaAccessor,
            sb: sb,
            cancel: cancel);
    }
}