using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class DictFieldGenerator : ISerializationForFieldGenerator
{
    private readonly IsGroupTester _groupTester;
    
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public DictFieldGenerator(
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator,
        IsGroupTester groupTester)
    {
        _forFieldGenerator = forFieldGenerator;
        _groupTester = groupTester;
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
        if (_groupTester.IsGroup(obj)) return;
        
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

        sb.AppendLine($"{kernelAccessor}.StartDictionarySection({writerAccessor}, \"{fieldName}\");");
        sb.AppendLine($"foreach (var kv in {fieldAccessor})");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"{kernelAccessor}.StartDictionaryItem({writerAccessor});");
            sb.AppendLine($"{kernelAccessor}.StartDictionaryKey({writerAccessor});");
            _forFieldGenerator().Value.GenerateForField(compilation, obj, keyType, writerAccessor, null, "kv.Key", sb, cancel);
            sb.AppendLine($"{kernelAccessor}.EndDictionaryKey({writerAccessor});");
            sb.AppendLine($"{kernelAccessor}.StartDictionaryValue({writerAccessor});");
            _forFieldGenerator().Value.GenerateForField(compilation, obj, valType, writerAccessor, null, "kv.Value", sb, cancel);
            sb.AppendLine($"{kernelAccessor}.EndDictionaryValue({writerAccessor});");
            sb.AppendLine($"{kernelAccessor}.EndDictionaryItem({writerAccessor});");
        }
        sb.AppendLine($"{kernelAccessor}.EndDictionarySection({writerAccessor});");
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