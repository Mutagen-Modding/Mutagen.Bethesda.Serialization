using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

public class ListFieldGenerator : ISerializationForFieldGenerator
{
    private readonly IsGroupTester _groupTester;
    
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public ListFieldGenerator(
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator, 
        IsGroupTester groupTester)
    {
        _forFieldGenerator = forFieldGenerator;
        _groupTester = groupTester;
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "List",
        "IReadOnlyList",
        "IList",
        "ExtendedList",
    };

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            if (arr.ElementType.Name == "Byte") return false;
            return true;
        }
        else
        {
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
            var typeMembers = namedTypeSymbol.TypeArguments;
            if (typeMembers.Length != 1) return false;
            return _listStrings.Contains(typeSymbol.Name);
        }
    }

    private ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

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
        
        ITypeSymbol subType;
        if (field is IArrayTypeSymbol arr)
        {
            subType = arr.ElementType;
        }
        else if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = GetSubtype(namedTypeSymbol);
        }
        else
        {
            return;
        }

        using (sb.CurlyBrace())
        {
            const string listWriterName = $"listWriter";
            sb.AppendLine($"var {listWriterName} = {kernelAccessor}.StartListSection({writerAccessor}, \"{fieldName}\");");
            sb.AppendLine($"foreach (var listItem in {fieldAccessor})");
            using (sb.CurlyBrace())
            {
                _forFieldGenerator().Value.GenerateForField(compilation, obj, subType, listWriterName, null, "listItem", sb, cancel);
            }
            sb.AppendLine($"{kernelAccessor}.EndListSection();");
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