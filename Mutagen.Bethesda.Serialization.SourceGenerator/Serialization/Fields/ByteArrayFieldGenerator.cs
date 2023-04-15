using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class ByteArrayFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "Noggog";
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "ReadOnlyMemorySlice",
        "MemorySlice",
        "Noggog.ReadOnlyMemorySlice",
        "Noggog.MemorySlice",
    };
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;
    
    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName)
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
                c.Add(DefaultString(field));
            }

            if (isInsideCollection)
            {
                c.Add("checkDefaults: false");
            }
        }
    }

    public bool HasVariableHasSerialize => true;

    public string DefaultString(ITypeSymbol? field) => $"default(byte[]{(field != null && field.IsNullable() ? "?" : null)})";

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
        sb.AppendLine($"if (!MemorySliceExt.SequenceEqual<{GetSubtype(field)}>({fieldAccessor}, {defaultValueAccessor ?? DefaultString(field)})) return true;");
    }

    public void GenerateForDeserializeSingleFieldInto(
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
        Utility.WrapStripNull(
            field, 
            fieldName,
            fieldAccessor, 
            readerAccessor, 
            kernelAccessor,
            insideCollection,
            sb,
            AddReadCall);
    }

    private void AddReadCall(
        StructuredStringBuilder sb,
        ITypeSymbol? field,
        string kernelAccessor,
        string readerAccessor,
        string setAccessor)
    {
        using (var c = sb.Call($"{setAccessor}{kernelAccessor}.ReadBytes", linePerArgument: false))
        {
            c.Add(readerAccessor);
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}