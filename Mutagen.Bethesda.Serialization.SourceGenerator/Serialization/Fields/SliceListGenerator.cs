using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class SliceListGenerator : ISerializationForFieldGenerator
{
    private readonly ByteArrayFieldGenerator _byteArrGenerator;
    
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();

    public IEnumerable<string> RequiredNamespaces(
        LoquiTypeSet obj,
        CompilationUnit compilation,
        ITypeSymbol typeSymbol)
    {
        yield return "Noggog";
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "SliceList",
    };

    public SliceListGenerator(
        ByteArrayFieldGenerator byteArrGenerator)
    {
        _byteArrGenerator = byteArrGenerator;
    }
    
    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;
    
    public bool Applicable(
        LoquiTypeSet obj, 
        CompilationUnit compilation,
        ITypeSymbol typeSymbol, 
        string? fieldName,
        bool isInsideCollection)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        namedTypeSymbol = namedTypeSymbol.PeelNullable();
        return _listStrings.Contains(namedTypeSymbol.Name) && namedTypeSymbol.TypeArguments[0].Name == "Byte";
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
        if (field is not INamedTypeSymbol namedTypeSymbol) throw new ArgumentException($"Field not a named type symbol: {field}");
        sb.AppendLine($"{kernelAccessor}.StartListSection({writerAccessor}, \"{fieldName}\");");
        sb.AppendLine($"foreach (var row in {fieldAccessor})");
        using (sb.CurlyBrace())
        {
            _byteArrGenerator.GenerateForSerialize(
                compilation: compilation,
                obj: obj,
                writerAccessor: writerAccessor, 
                kernelAccessor: kernelAccessor,
                field: null!,
                fieldName: null, 
                fieldAccessor: "row",
                metaAccessor: metaAccessor,
                isInsideCollection: true,
                defaultValueAccessor: null,
                sb: sb,
                cancel: cancel);
        }
        sb.AppendLine($"{kernelAccessor}.EndListSection({writerAccessor});");
    }

    public bool HasVariableHasSerialize => true;

    public string? GetDefault(ITypeSymbol field)
    {
        throw new NotImplementedException($"No GetDefault defined for {typeof(SliceListGenerator)}");
    }

    public void GenerateForHasSerialize(CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string metaAccessor,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}.Count != 0) return true;");
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
        field = field.PeelNullable();

        var nullable = field.IsNullable();

        if (nullable)
        {
            sb.AppendLine($"{fieldAccessor} ??= new();");
        }
        
        sb.AppendLine($"{kernelAccessor}.StartListSection({readerAccessor});");
        sb.AppendLine($"while ({kernelAccessor}.TryHasNextItem({readerAccessor}))");
        using (sb.CurlyBrace())
        {
            _byteArrGenerator.GenerateForDeserializeSingleFieldInto(
                compilation: compilation,
                obj: obj,
                field: null!,
                readerAccessor: readerAccessor,
                kernelAccessor: kernelAccessor,
                metaAccessor: metaAccessor,
                canSet: false,
                insideCollection: true,
                fieldName: fieldName, 
                fieldAccessor: "var item = ",
                sb: sb,
                cancel: cancel);
            sb.AppendLine($"if (item != null)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"{fieldAccessor}.Add(item.Value);");
            }
        }
        sb.AppendLine($"{kernelAccessor}.EndListSection({readerAccessor});");
    }

    public void GenerateForDeserializeSection(CompilationUnit compilation, LoquiTypeSet obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}