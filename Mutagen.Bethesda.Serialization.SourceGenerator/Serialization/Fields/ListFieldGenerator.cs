using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

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
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        var subType = GetSubtype((INamedTypeSymbol)typeSymbol);
        var gen = _forFieldGenerator().Value
            .GetGenerator(subType, cancel);
        return gen?.RequiredNamespaces(subType, cancel) ?? Enumerable.Empty<string>();
    }

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arr)
        {
            if (arr.ElementType.Name == "Byte") return false;
            return true;
        }
        else
        {
            typeSymbol = typeSymbol.PeelNullable();
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
            var typeMembers = namedTypeSymbol.TypeArguments;
            if (typeMembers.Length != 1) return false;
            return _listStrings.Contains(typeSymbol.Name);
        }
    }

    private ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

    private string GetCountAccessor(ITypeSymbol t)
    {
        switch (t.Name)
        {
            case "List":
            case "IReadOnlyList":
            case "IList":
            case "ExtendedList":
                return ".Count";
            default:
                return ".Length";
        }
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
        if (_groupTester.IsGroup(obj)) return;

        var nullable = field.IsNullable();

        field = field.PeelNullable();
        
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

        using (var i = sb.If(ands: true))
        {
            i.Add($"{fieldAccessor} is {{}} checked{fieldName}");
            if (!nullable)
            {
                i.Add($"checked{fieldName}{GetCountAccessor(field)} > 0");
            }
        }
        fieldAccessor = $"checked{fieldName}";
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"{kernelAccessor}.StartListSection({writerAccessor}, \"{fieldName}\");");
            sb.AppendLine($"foreach (var listItem in {fieldAccessor})");
            using (sb.CurlyBrace())
            {
                _forFieldGenerator().Value.GenerateSerializeForField(
                    compilation: compilation,
                    obj: obj,
                    fieldType: subType,
                    writerAccessor: writerAccessor, 
                    fieldName: null, 
                    fieldAccessor: "listItem",
                    defaultValueAccessor: null,
                    sb: sb,
                    cancel: cancel);
            }
            sb.AppendLine($"{kernelAccessor}.EndListSection({writerAccessor});");
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
        sb.AppendLine($"if ({fieldAccessor}{field.NullChar()}{GetCountAccessor(field)} > 0) return true;");
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
        if (_groupTester.IsGroup(obj)) return;

        field = field.PeelNullable();
        
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

        var nullable = field.IsNullable();

        if (nullable)
        {
            sb.AppendLine($"{fieldAccessor} ??= new();");
        }
        
        sb.AppendLine($"{kernelAccessor}.StartListSection({readerAccessor});");
        sb.AppendLine($"while ({kernelAccessor}.TryHasNextItem({readerAccessor}))");
        using (sb.CurlyBrace())
        {
            _forFieldGenerator().Value.GenerateDeserializeForField(
                compilation: compilation,
                obj: obj,
                fieldType: subType,
                readerAccessor: readerAccessor, 
                fieldName: fieldName, 
                fieldAccessor: "var item = ",
                sb: sb,
                cancel: cancel);
            sb.AppendLine($"{fieldAccessor}.Add(item);");
        }
        sb.AppendLine($"{kernelAccessor}.EndListSection({readerAccessor});");
    }
}