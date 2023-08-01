using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FormLinkOrIndexFieldGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    private static readonly HashSet<string> _expectedStrings = new()
    {
        "FormLinkOrIndex",
        "IFormLinkOrIndex",
        "IFormLinkOrIndexGetter",
    };

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "Mutagen.Bethesda.Plugins";
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public bool HasVariableHasSerialize => true;

    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _expectedStrings.Contains(typeSymbol.Name);
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
        using (var c = sb.Call($"SerializationHelper.WriteFormLinkOrIndex", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add(fieldAccessor);
            c.Add(metaAccessor);
            c.Add(kernelAccessor);
        }
    }

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException($"No GetDefault defined for {typeof(FormLinkOrIndexFieldGenerator)}");
    }

    public void GenerateForHasSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        var named = (INamedTypeSymbol)field;
        var sub = named.TypeArguments[0];
        if (!compilation.Mapping.TryGetTypeSet(sub, out var typeSet))
        {
            throw new ArgumentException($"Could not get compilation mapping for {sub} in {nameof(FormLinkOrIndexFieldGenerator)}");
        }

        sb.AppendLine($"if (!FormLinkOrIndex<{typeSet.Getter}>.EqualityComparer.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default(FormLinkOrIndex<{typeSet.Getter}>)"})) return true;");
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
        using (var c = sb.Call($"SerializationHelper.ReadFormLinkOrIndex", linePerArgument: false))
        {
            c.Add(readerAccessor);
            c.Add(kernelAccessor);
            c.Add(fieldAccessor);
            c.Add(metaAccessor);
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}