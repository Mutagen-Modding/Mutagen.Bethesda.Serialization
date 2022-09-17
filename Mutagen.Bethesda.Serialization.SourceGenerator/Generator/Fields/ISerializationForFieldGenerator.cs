using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public interface ISerializationForFieldGenerator
{
    IEnumerable<string> AssociatedTypes { get; }

    IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel);

    bool Applicable(ITypeSymbol typeSymbol);
    
    void GenerateForSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel);
    
    void GenerateForDeserialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        IPropertySymbol propertySymbol, 
        string itemAccessor,
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel);
}