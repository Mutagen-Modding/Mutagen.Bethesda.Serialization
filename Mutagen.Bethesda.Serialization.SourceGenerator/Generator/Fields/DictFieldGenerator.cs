using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class DictFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public DictFieldGenerator(Func<IOwned<SerializationFieldGenerator>> forFieldGenerator)
    {
        _forFieldGenerator = forFieldGenerator;
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "Dictionary",
        "IReadOnlyDictionary",
        "IDictionary",
    };

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 2) return false;
        return _listStrings.Contains(typeSymbol.Name);
    }

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
        ITypeSymbol keyType;
        ITypeSymbol valType;
        if (field is INamedTypeSymbol namedTypeSymbol)
        {
            keyType = namedTypeSymbol.TypeArguments[0];
            valType = namedTypeSymbol.TypeArguments[1];
        }
        else
        {
            return;
        }

        using (sb.CurlyBrace())
        {
            var dictWriterName = $"{fieldName}DictWriter";
            sb.AppendLine($"var {dictWriterName} = {kernelAccessor}.StartDictionarySection({writerAccessor}, \"{fieldName}\");");
            sb.AppendLine($"foreach (var kv in {fieldAccessor})");
            using (sb.CurlyBrace())
            {
                var dictItemWriterName = "itemWriter";
                var dictKeyWriterName = "keyWriter";
                var dictValueWriterName = "valueWriter";
                sb.AppendLine($"var {dictItemWriterName} = {kernelAccessor}.StartDictionaryItem({dictWriterName});");
                sb.AppendLine($"var {dictKeyWriterName} = {kernelAccessor}.StartDictionaryKey({dictItemWriterName});");
                _forFieldGenerator().Value.GenerateForField(compilation, obj, keyType, dictKeyWriterName, null, "kv.Key", sb, cancel);
                sb.AppendLine($"{kernelAccessor}.StopDictionaryKey();");
                sb.AppendLine($"var {dictValueWriterName} = {kernelAccessor}.StartDictionaryValue({dictItemWriterName});");
                _forFieldGenerator().Value.GenerateForField(compilation, obj, valType, dictValueWriterName, null, "kv.Value", sb, cancel);
                sb.AppendLine($"{kernelAccessor}.StopDictionaryValue();");
                sb.AppendLine($"{kernelAccessor}.StopDictionaryItem();");
            }
            sb.AppendLine($"{kernelAccessor}.StopDictionarySection();");
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