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

        using (sb.CurlyBrace())
        {
            var writerItem = $"{fieldName}A2Writer";
            sb.AppendLine($"var {writerItem} = {kernelAccessor}.StartArray2dSection({writerAccessor}, \"{fieldName}\");");
            sb.AppendLine($"foreach (var kv in {fieldAccessor})");
            using (sb.CurlyBrace())
            {
                var itemWriter = "itemWriter";
                sb.AppendLine($"var {itemWriter} = {kernelAccessor}.StartArray2dItem({writerItem}, kv.Key.X, kv.Key.Y);");
                _forFieldGenerator().Value.GenerateForField(compilation, obj, subType, itemWriter, null, "kv.Value", sb, cancel);
                sb.AppendLine($"{kernelAccessor}.StopArray2dItem();");
            }
            sb.AppendLine($"{kernelAccessor}.StopArray2dSectionSection();");
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