using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class PropertyCollection
{
    public Dictionary<string, PropertyMetadata> Lookup = new();
    public List<PropertyMetadata> InOrder = new();
    
    public void Register(PropertyMetadata propertyMetadata)
    {
        InOrder.Add(propertyMetadata);
        Lookup[propertyMetadata.Property.Name] = propertyMetadata;
    }
}

public record PropertyMetadata(IPropertySymbol Property, ISerializationForFieldGenerator? Generator)
{
    public IFieldSymbol? Default { get; set; }

    private string? _defaultString;
    public string? DefaultString
    {
        get => _defaultString ?? GetDefaultDefaultString();
        set => _defaultString = value;
    }

    private string? GetDefaultDefaultString()
    {
        if (Default == null) return null;
        var className = Default.ContainingSymbol.Name;
        if (Default.ContainingSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
        {
            className += $"<{string.Join(", ", namedTypeSymbol.TypeArguments)}>";
        }
        return $"{Default.ContainingSymbol.ContainingNamespace}.{className}.{Default.Name}";
    }
}

public class PropertyCollectionRetriever
{
    private readonly PropertyFilter _propertyFilter;
    private readonly SerializationFieldGenerator _forFieldGenerator;

    public PropertyCollectionRetriever(
        PropertyFilter propertyFilter, 
        SerializationFieldGenerator forFieldGenerator)
    {
        _propertyFilter = propertyFilter;
        _forFieldGenerator = forFieldGenerator;
    }

    public PropertyCollection GetPropertyCollection(
        CompilationUnit compilation, 
        LoquiTypeSet obj)
    {
        var ret = new PropertyCollection();
        
        FillMembers(compilation, obj, ret);

        FillDefaults(obj, ret);
        
        return ret;
    }
    
    private void FillMembers(
        CompilationUnit compilation, 
        LoquiTypeSet obj,
        PropertyCollection collection)
    {
        var objTarget = obj.Setter ?? obj.Direct ?? throw new NullReferenceException();
        foreach (var prop in objTarget.GetMembers().WhereCastable<ISymbol, IPropertySymbol>())
        {
            compilation.Context.CancellationToken.ThrowIfCancellationRequested();
            var gen = _forFieldGenerator.GetGenerator(obj, compilation, prop.Type, prop.Name, isInsideCollection: false);
            if (_propertyFilter.Skip(obj, compilation, prop, gen)) continue;
            var meta = new PropertyMetadata(prop, gen);
            collection.Register(meta);
        }
    }

    private static void FillDefaults(LoquiTypeSet obj, PropertyCollection collection)
    {
        if (obj.Direct == null) return;
        
        foreach (var field in obj.Direct.GetMembers().WhereCastable<ISymbol, IFieldSymbol>())
        {
            if (!field.IsStatic || !field.IsReadOnly) continue;
            if (!field.Name.EndsWith("Default")) continue;
            if (collection.Lookup.TryGetValue(field.Name.TrimStringFromEnd("Default"), out var prop))
            {
                prop.Default = field;
            }
        }

        AddOneOffDefaults(obj, collection);
    }

    private static void AddOneOffDefaults(LoquiTypeSet obj, PropertyCollection collection)
    {
        if (collection.Lookup.TryGetValue("FormVersion", out var prop))
        {
            prop.DefaultString = $"metaData.Constants.DefaultFormVersion ?? 0";
        }
    }
}