using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public interface ISerializationInterceptor
{
    public bool Applicable(CompilationUnit compilation, LoquiTypeSet typeSet);

    void GenerateSerialize(
        CompilationUnit compilation,
        LoquiTypeSet typeSet,
        StructuredStringBuilder sb,
        ITypeSymbol? baseType,
        PropertyCollection properties,
        SerializationGenerics generics);

    void GenerateDeserialize(
        CompilationUnit compilation, LoquiTypeSet typeSet, StructuredStringBuilder sb,
        ITypeSymbol? baseType, PropertyCollection properties, SerializationGenerics generics);
}