using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class GroupFieldGenerator : ISerializationForFieldGenerator
{
    private readonly LoquiSerializationNaming _serializationNaming;
    private readonly LoquiNameRetriever _nameRetriever;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public GroupFieldGenerator(
        LoquiSerializationNaming serializationNaming,
        LoquiNameRetriever nameRetriever)
    {
        _serializationNaming = serializationNaming;
        _nameRetriever = nameRetriever;
    }
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeParameters.Length == 1)
        {
            var name = _nameRetriever.GetNames(typeSymbol.Name);
            if (name.Direct.EndsWith("Group"))
            {
                return true;
            }
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
        var subType = namedTypeSymbol.TypeArguments[0];
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldNames)) return;
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subNames)) return;
        sb.AppendLine($"{fieldNames.SerializationCall(serialize: true)}({fieldAccessor}, {writerAccessor}, {kernelAccessor});");
        sb.AppendLine($"foreach (var rec in {fieldAccessor}.Records)");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"{subNames.SerializationCall(serialize: true)}(rec, {writerAccessor}, {kernelAccessor});");
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