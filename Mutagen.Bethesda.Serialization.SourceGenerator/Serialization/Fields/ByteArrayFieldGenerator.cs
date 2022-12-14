using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

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
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;
    
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

    private ITypeSymbol GetSubtype(ITypeSymbol t)
    {
        t = t.PeelNullable();
        if (t is INamedTypeSymbol n)
        {
            return n.TypeArguments[0];
        }
        else if (t is IArrayTypeSymbol ar)
        {
            return ar.ElementType;
        }

        throw new NotImplementedException();
    }

    public void GenerateForSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj, 
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor, 
        string metaAccessor,
        bool isInsideCollection,
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

            if (isInsideCollection)
            {
                c.Add("checkDefaults: false");
            }
        }
    }

    public bool HasVariableHasSerialize => true;

    public void GenerateForHasSerialize(CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string metaAccessor,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        sb.AppendLine($"if (!MemorySliceExt.SequenceEqual<{GetSubtype(field)}>({fieldAccessor}, {defaultValueAccessor ?? $"default({field})"})) return true;");
    }

    public void GenerateForDeserialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string readerAccessor,
        string kernelAccessor,
        string metaAccessor,
        bool insideCollection,
        bool canSet,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        using (var c = sb.Call($"{fieldAccessor} = {kernelAccessor}.ReadBytes", linePerArgument: false))
        {
            c.Add(readerAccessor);
        }
    }
}