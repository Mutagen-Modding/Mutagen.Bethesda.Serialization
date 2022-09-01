using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class EnumFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        typeSymbol = Utility.PeelNullable(typeSymbol);
        return typeSymbol.TypeKind == TypeKind.Enum
            || (typeSymbol.BaseType is { Name: "Enum" } 
                && typeSymbol.BaseType.ContainingNamespace.ToString() == "System");
    }

    public void GenerateForSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj, 
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string writerAccessor,
        string kernelAccessor, 
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"{kernelAccessor}.WriteEnum({writerAccessor}, {(fieldName == null ? "null" : $"\"{fieldName}\"")}, {fieldAccessor});");
    }

    public void GenerateForDeserialize(
        CompilationUnit compilation,
        ITypeSymbol obj, 
        IPropertySymbol propertySymbol,
        string itemAccessor,
        string writerAccessor, 
        string kernelAccessor, 
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        throw new NotImplementedException();
    }
}