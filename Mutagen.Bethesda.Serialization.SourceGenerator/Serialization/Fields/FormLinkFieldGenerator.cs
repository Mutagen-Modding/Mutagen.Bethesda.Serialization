using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

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

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Mutagen.Bethesda.Plugins";
    }

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    private bool IsNullable(ITypeSymbol field) => field.Name.Contains("Nullable");

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
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor, 
        string metaAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        var nullable = IsNullable(field);
        using (var c = sb.Call($"{kernelAccessor}.WriteFormKey", linePerArgument: false))
        {
            c.Add(writerAccessor);
            c.Add($"{(fieldName == null ? "null" : $"\"{fieldName}\"")}");
            c.Add($"{fieldAccessor}.FormKeyNullable");
            if (defaultValueAccessor != null)
            {
                c.Add($"{defaultValueAccessor}.FormKeyNullable");
            }
            else
            {
                c.Add($"default(FormKey{Utility.NullChar(nullable)})");
            }
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
        var named = (INamedTypeSymbol)field;
        var nullable = IsNullable(field);
        var sub = named.TypeArguments[0];
        if (!compilation.Mapping.TryGetTypeSet(sub, out var typeSet))
        {
            throw new NotImplementedException();
        }

        var linkStr = $"IFormLink{(nullable ? "Nullable" : null)}Getter<{typeSet.Getter}>";
        sb.AppendLine($"if (!EqualityComparer<{linkStr}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({linkStr})"})) return true;");
    }

    public void GenerateForDeserialize(
        CompilationUnit compilation,
        ITypeSymbol obj,
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
        var nullable = IsNullable(field);
        if (insideCollection)
        {
            if (field is not INamedTypeSymbol named
                || !named.IsGenericType
                || named.TypeArguments.Length != 1)
            {
                throw new NotImplementedException();
            }

            if (nullable)
            {
                sb.AppendLine($"{fieldAccessor}{kernelAccessor}.ReadFormKey({readerAccessor}).AsNullableLink<{named.TypeArguments[0]}>();");
            }
            else
            {
                sb.AppendLine($"{fieldAccessor}SerializationHelper.StripNull({kernelAccessor}.ReadFormKey({readerAccessor}), \"{fieldName}\").AsLink<{named.TypeArguments[0]}>();");
            }
        }
        else
        {
            if (nullable)
            {
                sb.AppendLine($"{fieldAccessor}.SetTo(SerializationHelper.StripNull({kernelAccessor}.ReadFormKey({readerAccessor}), \"{fieldName}\"));");
            }
            else
            {
                sb.AppendLine($"{fieldAccessor}.SetTo({kernelAccessor}.ReadFormKey({readerAccessor}));");   
            }
        }
    }
}