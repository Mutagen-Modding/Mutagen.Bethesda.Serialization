using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class ByteArrayFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Noggog";
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "ReadOnlyMemorySlice",
        "Noggog.ReadOnlyMemorySlice",
    };

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            return arr.ElementType.Name == "Byte";
        }
        else
        {
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
            var typeMembers = namedTypeSymbol.TypeArguments;
            if (typeMembers.Length != 1) return false;
            namedTypeSymbol = namedTypeSymbol.PeelNullable();
            return _listStrings.Contains(namedTypeSymbol.Name) && namedTypeSymbol.TypeArguments[0].Name == "Byte";
        }
    }

    private ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

    public void GenerateForSerialize(
        CompilationUnit compilation,
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
        using (var c = sb.Call($"{kernelAccessor}.WriteBytes", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add(fieldAccessor);
            if (defaultValueAccessor != null)
            {
                c.Add(defaultValueAccessor);
            }
            else
            {
                c.Add($"default({field})");
            }
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