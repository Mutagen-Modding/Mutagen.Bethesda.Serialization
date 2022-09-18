using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

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
        => Enumerable.Empty<string>();

    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _interestStrings.Contains(typeSymbol.Name);
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

        sb.AppendLine($"{kernelAccessor}.StartArray2dSection({writerAccessor}, \"{fieldName}\");");
        sb.AppendLine($"for (int y = 0; y < {fieldAccessor}.Height; y++)");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"{kernelAccessor}.StartArray2dYSection({writerAccessor});");
            sb.AppendLine($"for (int x = 0; x < {fieldAccessor}.Width; x++)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"{kernelAccessor}.StartArray2dXSection({writerAccessor});");
                _forFieldGenerator().Value.GenerateForField(
                    compilation: compilation, 
                    obj: obj, 
                    fieldType: subType,
                    writerAccessor: writerAccessor, 
                    fieldName: null, 
                    fieldAccessor: $"{fieldAccessor}[x, y]", 
                    defaultValueAccessor: null,
                    sb: sb,
                    cancel: cancel);
                sb.AppendLine($"{kernelAccessor}.EndArray2dXSection({writerAccessor});");
            }
            sb.AppendLine($"{kernelAccessor}.EndArray2dYSection({writerAccessor});");
        }
        sb.AppendLine($"{kernelAccessor}.EndArray2dSection({writerAccessor});");
    }

    public bool HasVariableHasSerialize => false;

    public void GenerateForHasSerialize(CompilationUnit compilation,
        ITypeSymbol obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        throw new NotImplementedException();
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