using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class PrimitiveFieldGenerator : ISerializationForFieldGenerator
{
    private readonly string _nickname;
    private Lazy<IEnumerable<string>> _associatedTypes;
    public IEnumerable<string> AssociatedTypes => _associatedTypes.Value;

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
        string writerAccessor,
        string kernelAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        sb.AppendLine($"{kernelAccessor}.Write{_nickname}({writerAccessor}, {(fieldName == null ? "null" : $"\"{fieldName}\"")}, {fieldAccessor});");
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