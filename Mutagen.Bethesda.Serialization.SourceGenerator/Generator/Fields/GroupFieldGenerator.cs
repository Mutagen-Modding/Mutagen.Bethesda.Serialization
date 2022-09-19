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

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel) => Enumerable.Empty<string>();

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
        string? defaultValueAccessor,
        string writerAccessor, 
        string kernelAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        if (field is not INamedTypeSymbol namedTypeSymbol) return;
        var subType = namedTypeSymbol.TypeArguments[0];
        if (!_serializationNaming.TryGetSerializationItems(field, out var fieldSerializationNames)) return;
        if (!_serializationNaming.TryGetSerializationItems(subType, out var subSerializationNames)) return;
        var groupNames = _nameRetriever.GetNames(field);
        var subNames = _nameRetriever.GetNames(subType);
        using (var f = sb.Call($"SerializationHelper.WriteGroup<TKernel, TWriteObject, {groupNames.Getter}<{subNames.Getter}>, {subNames.Getter}>"))
        {
            f.Add($"writer: {writerAccessor}");
            f.Add($"group: {fieldAccessor}");
            f.Add($"fieldName: {(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            f.Add($"meta: {metaAccessor}");
            f.Add($"kernel: {kernelAccessor}");
            f.Add($"groupWriter: static (w, i, k, m) => {fieldSerializationNames.SerializationCall(serialize: true)}<TKernel, TWriteObject, {subNames.Getter}>(w, i, k, m)");
            f.Add($"itemWriter: static (w, i, k, m) => {subSerializationNames.SerializationCall(serialize: true)}<TKernel, TWriteObject>(w, i, k, m)");
        }
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