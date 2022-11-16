using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class PrimitiveFieldGenerator : ISerializationForFieldGenerator
{
    private readonly string _nickname;
    private Lazy<IEnumerable<string>> _associatedTypes;
    public IEnumerable<string> AssociatedTypes => _associatedTypes.Value;
    
    public virtual IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel) 
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
                .ToArray();
        });
    }
    
    public bool Applicable(ITypeSymbol typeSymbol) => false;
    
    public virtual bool ShouldGenerate(IPropertySymbol propertySymbol)
    {
        return !propertySymbol.IsReadOnly;
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
        sb.AppendLine($"if (!EqualityComparer<{field}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({field})"})) return true;");
    }

    public virtual void GenerateForDeserialize(
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
        var setAccessor = $"{fieldAccessor}{(insideCollection ? null : " = ")}";
        if (field.IsNullable())
        {
            AddReadCall(sb, kernelAccessor, readerAccessor, setAccessor);
        }
        else
        {
            using (var strip = sb.Call($"{setAccessor}SerializationHelper.StripNull", linePerArgument: false))
            {
                strip.Add((subSb) =>
                {
                    AddReadCall(subSb, kernelAccessor, readerAccessor, string.Empty);
                });
                if (fieldName == null) throw new NullReferenceException();
                strip.Add($"name: \"{fieldName}\"");
            }
        }
    }

    private void AddReadCall(
        StructuredStringBuilder sb,
        string kernelAccessor,
        string readerAccessor,
        string setAccessor)
    {
        using (var c = sb.Call($"{setAccessor}{kernelAccessor}.Read{_nickname}", linePerArgument: false))
        {
            c.Add(readerAccessor);
        }
    }
}