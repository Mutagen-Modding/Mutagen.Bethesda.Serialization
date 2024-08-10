using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class EdidLinkGenerator : ISerializationForFieldGenerator
{
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    private static readonly HashSet<string> _expectedStrings = new()
    {
        "EDIDLink",
        "IEDIDLink",
        "IEDIDLinkGetter",
    };

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "Mutagen.Bethesda.Plugins";
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    private bool IsNullable(ITypeSymbol field) => false;

    public bool HasVariableHasSerialize => true;

    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationCatalog customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName,
        bool isInsideCollection)
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
        var nullable = IsNullable(field);
        using (var c = sb.Call($"{kernelAccessor}.WriteString", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add($"{fieldAccessor}.EDID.Type");
            if (defaultValueAccessor != null)
            {
                c.Add($"{defaultValueAccessor}.EDID.Type");
            }
            else
            {
                c.Add($"RecordType.Null.Type");
            }

            if (isInsideCollection)
            {
                c.Add("checkDefaults: false");
            }
        }
    }

    public string? GetDefault(ITypeSymbol field)
    {
        if (field is not INamedTypeSymbol namedTypeSymbol) return null;
        var arg = namedTypeSymbol.TypeArguments[0];
        return $"EDIDLink<{arg}>.Null";
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
            throw new ArgumentException($"Could not get compilation mapping for {sub} in {nameof(EdidLinkGenerator)}");
        }

        var linkStr = $"IEDIDLinkGetter<{typeSet.Getter}>";
        sb.AppendLine($"if (!EqualityComparer<{linkStr}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({linkStr})"})) return true;");
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
        if (insideCollection)
        {
            if (field is not INamedTypeSymbol named
                || !named.IsGenericType
                || named.TypeArguments.Length != 1)
            {
                throw new ArgumentException($"Field is not appropriate for {nameof(EdidLinkGenerator)}: {field}");
            }
            
            sb.AppendLine($"{fieldAccessor}new EDIDLink<{named.TypeArguments[0]}>({kernelAccessor}.ReadString({readerAccessor}).StripNull(\"{fieldName}\"));");
        }
        else
        {
            sb.AppendLine($"{fieldAccessor}.EDID = {kernelAccessor}.ReadString({readerAccessor}).StripNull(\"{fieldName}\");");
        }
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}