using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class GenderedTypeFieldGenerator : ISerializationForFieldGenerator
{
    private readonly Func<IOwned<SerializationFieldGenerator>> _forFieldGenerator;
    public IEnumerable<string> AssociatedTypes => Array.Empty<string>();
    
    public GenderedTypeFieldGenerator(Func<IOwned<SerializationFieldGenerator>> forFieldGenerator)
    {
        _forFieldGenerator = forFieldGenerator;
    }

    private static readonly HashSet<string> _listStrings = new()
    {
        "GenderedItem",
        "IGenderedItem",
        "IGenderedItemGetter",
    };

    public IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel) => Enumerable.Empty<string>();
    
    public bool Applicable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        var typeMembers = namedTypeSymbol.TypeArguments;
        if (typeMembers.Length != 1) return false;
        return _listStrings.Contains(typeSymbol.Name);
    }

    private ITypeSymbol GetSubtype(INamedTypeSymbol t) => t.TypeArguments[0];

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

        _forFieldGenerator().Value.GenerateSerializeForField(
            compilation: compilation,
            obj: obj, fieldType: subType,
            writerAccessor: writerAccessor,
            fieldName: fieldName == null ? null : $"{fieldName}Male",
            fieldAccessor: $"{fieldAccessor}.Male",
            defaultValueAccessor: null,
            sb: sb,
            cancel: cancel);
        _forFieldGenerator().Value.GenerateSerializeForField(
            compilation: compilation,
            obj: obj, 
            fieldType: subType,
            writerAccessor: writerAccessor, 
            fieldName: fieldName == null ? null : $"{fieldName}Female", 
            fieldAccessor: $"{fieldAccessor}.Female",
            defaultValueAccessor: null,
            sb: sb,
            cancel: cancel);
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

        _forFieldGenerator().Value.GenerateHasSerializeForField(
            compilation: compilation,
            obj: obj, 
            fieldType: subType,
            fieldName: fieldName == null ? null : $"{fieldName}Male",
            fieldAccessor: $"{fieldAccessor}.Male",
            defaultValueAccessor: null,
            sb: sb,
            cancel: cancel);
        _forFieldGenerator().Value.GenerateHasSerializeForField(
            compilation: compilation,
            obj: obj, 
            fieldType: subType,
            fieldName: fieldName == null ? null : $"{fieldName}Female", 
            fieldAccessor: $"{fieldAccessor}.Female",
            defaultValueAccessor: null,
            sb: sb,
            cancel: cancel);
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
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
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

        _forFieldGenerator().Value.GenerateDeserializeForField(
            compilation: compilation,
            obj: obj, fieldType: subType,
            readerAccessor: readerAccessor,
            fieldName: fieldName == null ? null : $"{fieldName}Male",
            fieldAccessor: $"{fieldAccessor}.Male",
            sb: sb,
            cancel: cancel);
        _forFieldGenerator().Value.GenerateDeserializeForField(
            compilation: compilation,
            obj: obj, 
            fieldType: subType,
            readerAccessor: readerAccessor, 
            fieldName: fieldName == null ? null : $"{fieldName}Female", 
            fieldAccessor: $"{fieldAccessor}.Female",
            sb: sb,
            cancel: cancel);
    }
}