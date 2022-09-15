using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class FormLinkFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    private static readonly HashSet<string> _expectedStrings = new()
    {
        "FormLink",
        "FormLinkNullable",
        "IFormLink",
        "IFormLinkNullable",
        "IFormLinkGetter",
        "IFormLinkNullableGetter",
    };

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _expectedStrings.Contains(typeSymbol.Name);
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
        sb.AppendLine($"{kernelAccessor}.WriteFormKey({writerAccessor}, {(fieldName == null ? "null" : $"\"{fieldName}\"")}, {fieldAccessor}.FormKeyNullable);");
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