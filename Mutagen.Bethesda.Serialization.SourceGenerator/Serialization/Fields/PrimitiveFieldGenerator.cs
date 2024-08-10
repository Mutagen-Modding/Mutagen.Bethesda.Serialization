using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class PrimitiveFieldGenerator : ISerializationForFieldGenerator
{
    private readonly string _nickname;
    private Lazy<IEnumerable<string>> _associatedTypes;
    public IEnumerable<string> AssociatedTypes => _associatedTypes.Value;
    
    public virtual IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
        => Enumerable.Empty<string>();
    
    public PrimitiveFieldGenerator(
        string nickname,
        IReadOnlyCollection<string> associatedTypes)
    {
        _nickname = nickname;
        _associatedTypes = new Lazy<IEnumerable<string>>(() =>
        {
            return associatedTypes
                .Concat(associatedTypes.Select(x => $"{x}?"))
                .Concat(associatedTypes.Select(x => $"Nullable<{x}>"))
                .Concat(associatedTypes.Select(x => $"System.Nullable<{x}>"))
                .Concat(associatedTypes.Select(x => $"Nullable<{x}?>"))
                .Concat(associatedTypes.Select(x => $"System.Nullable<{x}?>"))
                .Concat(associatedTypes.Select(x => $"Nullable<{x}>?"))
                .Concat(associatedTypes.Select(x => $"System.Nullable<{x}>?"))
                .Concat(associatedTypes.Select(x => $"Nullable<{x}?>?"))
                .Concat(associatedTypes.Select(x => $"System.Nullable<{x}?>?"))
                .ToArray();
        });
    }
    
    public bool Applicable(
        LoquiTypeSet obj, 
        CustomizationCatalog customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName) => false;
    
    public virtual bool ShouldGenerate(IPropertySymbol propertySymbol)
    {
        return !propertySymbol.IsReadOnly;
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
        using (var c = sb.Call($"{kernelAccessor}.Write{_nickname}", linePerArgument: false))
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

    public string? GetDefault(ITypeSymbol field)
    {
        return $"default({field})";
    }

    public virtual void GenerateForHasSerialize(
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
        sb.AppendLine($"if (!EqualityComparer<{field}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({field})"})) return true;");
    }

    public virtual void GenerateForDeserializeSingleFieldInto(
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
        using (var c = sb.Call($"{setAccessor}{kernelAccessor}.Read{_nickname}", linePerArgument: false))
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