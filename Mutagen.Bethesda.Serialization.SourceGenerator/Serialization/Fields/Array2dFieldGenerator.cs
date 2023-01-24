using Microsoft.CodeAnalysis;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class Array2dFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    
    private static readonly HashSet<string> _interestStrings = new()
    {
        "Array2d",
        "IArray2d",
        "IReadOnlyArray2d",
    };

    public Array2dFieldGenerator(Func<IOwned<SerializationFieldGenerator>> forFieldGenerator)
    {
        _forFieldGenerator = forFieldGenerator;
    }

    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();
    
    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
        => "Noggog".AsEnumerable();

    public bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _interestStrings.Contains(typeSymbol.Name);
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
        ITypeSymbol subType;
        if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = namedTypeSymbol.TypeArguments[0];
        }
        else
        {
            return;
        }
        
        var nullable = field.IsNullable();

        using (var i = sb.If(ands: true))
        {
            i.Add($"{fieldAccessor} is {{}} checked{fieldName}");
            fieldAccessor = $"checked{fieldName}";
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"{kernelAccessor}.StartArray2dSection({writerAccessor}, \"{fieldName}\");");
            sb.AppendLine($"for (int y = 0; y < {fieldAccessor}.Height; y++)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"{kernelAccessor}.StartArray2dYSection({writerAccessor});");
                sb.AppendLine($"for (int x = 0; x < {fieldAccessor}.Width; x++)");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"{kernelAccessor}.StartArray2dXItem({writerAccessor});");
                    _forFieldGenerator().Value.GenerateSerializeForField(
                        compilation: compilation, 
                        obj: obj, 
                        fieldType: subType,
                        writerAccessor: writerAccessor, 
                        kernelAccessor: kernelAccessor,
                        metaDataAccessor: metaAccessor,
                        fieldName: null, 
                        fieldAccessor: $"{fieldAccessor}[x, y]", 
                        defaultValueAccessor: null,
                        sb: sb,
                        cancel: cancel);
                    sb.AppendLine($"{kernelAccessor}.EndArray2dXItem({writerAccessor});");
                }
                sb.AppendLine($"{kernelAccessor}.EndArray2dYSection({writerAccessor});");
            }
            sb.AppendLine($"{kernelAccessor}.EndArray2dSection({writerAccessor});");   
        }
    }

    public bool HasVariableHasSerialize => false;

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
        throw new NotImplementedException();
    }

    public void GenerateForDeserialize(
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
        ITypeSymbol subType;
        if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = namedTypeSymbol.TypeArguments[0];
        }
        else
        {
            return;
        }

        var nullable = field.IsNullable();

        if (nullable)
        {
            if (obj.Direct == null)
            {
                throw new NullReferenceException("Object with Array2D was not concrete");
            }
            sb.AppendLine($"{fieldAccessor} = new Array2d<{subType.Name}>({obj.Direct.Name}.{fieldName}FixedSize);");
        }
        using (var i = sb.If(ands: true))
        {
            i.Add($"{fieldAccessor} is {{}} checked{fieldName}");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"{kernelAccessor}.StartArray2dSection({readerAccessor});");
            sb.AppendLine("int y = 0;");
            sb.AppendLine($"while ({kernelAccessor}.TryHasNextArray2dYSection({readerAccessor}))");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"{kernelAccessor}.StartArray2dYSection({readerAccessor});");
                sb.AppendLine("int x = 0;");
                sb.AppendLine($"while ({kernelAccessor}.TryHasNextArray2dXItem({readerAccessor}))");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"{kernelAccessor}.StartArray2dXItem({readerAccessor});");
                    _forFieldGenerator().Value.GenerateDeserializeForField(
                        compilation: compilation,
                        obj: obj,
                        fieldType: subType,
                        readerAccessor: readerAccessor, 
                        kernelAccessor: kernelAccessor,
                        metaDataAccessor: metaAccessor,
                        fieldName: fieldName, 
                        fieldAccessor: "var item = ",
                        sb: sb,
                        cancel: cancel);
                    sb.AppendLine($"{fieldAccessor}[x, y] = item;");
                    sb.AppendLine($"{kernelAccessor}.EndArray2dXItem({readerAccessor});");
                    sb.AppendLine($"x++;");
                }
                sb.AppendLine($"{kernelAccessor}.EndArray2dYSection({readerAccessor});");
                sb.AppendLine($"y++;");
            }
            sb.AppendLine($"{kernelAccessor}.EndArray2dSection({readerAccessor});");
        }
    }
}