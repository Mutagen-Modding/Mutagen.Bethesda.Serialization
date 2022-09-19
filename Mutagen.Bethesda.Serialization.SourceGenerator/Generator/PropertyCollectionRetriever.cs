using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

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
    
    public string? DefaultString => Default == null ? null : $"{Default.ContainingSymbol.ContainingNamespace}.{Default.ContainingSymbol.Name}.{Default.Name}";
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
        SourceProductionContext context, 
        LoquiTypeSet obj)
    {
        var ret = new PropertyCollection();
        
        FillMembers(context, obj, ret);

        FillDefaults(obj, ret);
        
        return ret;
    }
    
    private void FillMembers(
        SourceProductionContext context, 
        LoquiTypeSet obj,
        PropertyCollection collection)
    {
        foreach (var prop in obj.Getter.GetMembers().WhereCastable<ISymbol, IPropertySymbol>())
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (_propertyFilter.Skip(obj, prop)) continue;
            var gen = _forFieldGenerator.GetGenerator(prop.Type, context.CancellationToken);
            var meta = new PropertyMetadata(prop, gen);
            collection.Register(meta);
        }
    }

    private static void FillDefaults(LoquiTypeSet obj, PropertyCollection ret)
    {
        if (obj.Direct == null) return;
        
        foreach (var field in obj.Direct.GetMembers().WhereCastable<ISymbol, IFieldSymbol>())
        {
            if (!field.IsStatic || !field.IsReadOnly) continue;
            if (!field.Name.EndsWith("Default")) continue;
            if (ret.Lookup.TryGetValue(field.Name.TrimEnd("Default"), out var prop))
            {
                prop.Default = field;
            }
        }
    }
}