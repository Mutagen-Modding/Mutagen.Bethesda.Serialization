using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class CacheFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    private static readonly HashSet<string> _listStrings = new()
    {
        "IReadOnlyCache",
        "Cache",
        "ICache",
        "Noggog.IReadOnlyCache",
        "Noggog.Cache",
        "Noggog.ICache",
    };

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel) => Enumerable.Empty<string>();

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        typeSymbol = typeSymbol.PeelNullable();
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 2) return false;
        return _listStrings.Contains(typeSymbol.Name);
    }

    public void GenerateForSerialize(CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor, 
        string kernelAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
    }

    public bool HasVariableHasSerialize => false;

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        throw new NotImplementedException();
    }

    public void GenerateForDeserialize(CompilationUnit compilation, 
        ITypeSymbol obj, 
        IPropertySymbol propertySymbol,
        string itemAccessor, 
        string writerAccessor, 
        string kernelAccessor, 
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
    }
}