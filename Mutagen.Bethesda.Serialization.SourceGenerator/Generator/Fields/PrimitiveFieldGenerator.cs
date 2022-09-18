using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

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
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"if (!EqualityComparer<{field}>.Default.Equals({fieldAccessor}, {defaultValueAccessor ?? $"default({field})"})) return true;");
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