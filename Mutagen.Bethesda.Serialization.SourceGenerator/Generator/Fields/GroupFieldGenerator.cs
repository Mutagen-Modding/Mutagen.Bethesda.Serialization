using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class GroupFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeParameters.Length == 1
            && typeSymbol.Name.EndsWith("Group"))
        {
            return true;
        }

        return false;
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
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        sb.AppendLine($"{field.Name}_Serialization.Serialize({fieldAccessor}, {writerAccessor}, {kernelAccessor});");
        sb.AppendLine($"foreach (var item in {fieldAccessor}.Records)");
        using (sb.CurlyBrace())
        {
            var subType = namedTypeSymbol.TypeArguments[0];
            sb.AppendLine($"{subType.Name}_Serialization.Serialize(item, {writerAccessor}, {kernelAccessor});");
        }
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