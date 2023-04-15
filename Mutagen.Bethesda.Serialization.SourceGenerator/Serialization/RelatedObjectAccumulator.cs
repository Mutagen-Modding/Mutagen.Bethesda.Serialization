using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class RelatedObjectAccumulator
{
    private readonly IsGroupTester _isGroupTester;
    private readonly GenderedTypeFieldGenerator _genderedTypeFieldGenerator;
    private readonly ArrayFieldGenerator _arrayFieldGenerator;
    private readonly ListFieldGenerator _listFieldGenerator;
    private readonly IsLoquiObjectTester _loquiObjectTester;

    public RelatedObjectAccumulator(
        IsGroupTester isGroupTester,
        GenderedTypeFieldGenerator genderedTypeFieldGenerator,
        ListFieldGenerator listFieldGenerator,
        IsLoquiObjectTester loquiObjectTester,
        ArrayFieldGenerator arrayFieldGenerator)
    {
        _isGroupTester = isGroupTester;
        _genderedTypeFieldGenerator = genderedTypeFieldGenerator;
        _listFieldGenerator = listFieldGenerator;
        _loquiObjectTester = loquiObjectTester;
        _arrayFieldGenerator = arrayFieldGenerator;
    }
    
    public ImmutableHashSet<LoquiTypeSet> GetRelatedObjects(
        LoquiMapping mapper,
        ITypeSymbol details, 
        CustomizationSpecifications customization,
        CancellationToken cancel)
    {
        if (!mapper.TryGetTypeSet(details, out var typeSet)) return ImmutableHashSet<LoquiTypeSet>.Empty;
        
        var objs = new HashSet<LoquiTypeSet>();
        GetRelatedObjects(mapper, typeSet, objs, customization, cancel);
        return objs.ToImmutableHashSet<LoquiTypeSet>();
    }

    private void GetRelatedObjects(
        LoquiMapping mapper,
        LoquiTypeSet obj, 
        HashSet<LoquiTypeSet> processedDetails,
        CustomizationSpecifications customization,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var details = obj.Getter;
        if (!_loquiObjectTester.IsLoqui(details)) return;
        if (!processedDetails.Add(obj)) return;
        var baseType = mapper.TryGetBaseClass(obj);
        if (baseType != null
            && mapper.TryGetTypeSet(baseType, out var baseSet))
        {
            GetRelatedObjects(mapper, baseSet, processedDetails, customization, cancel);
        }

        var inheriting = mapper.TryGetInheritingClasses(obj);
        foreach (var inherit in inheriting)
        {
            cancel.ThrowIfCancellationRequested();
            GetRelatedObjects(mapper, inherit, processedDetails, customization, cancel);
        }
        foreach (var memb in details.GetMembers())
        {
            cancel.ThrowIfCancellationRequested();
            if (memb is not IPropertySymbol prop) continue;
            if (prop.IsStatic) continue;

            foreach (var type in TransformSymbol(obj, customization, prop.Type, prop.Name))
            {
                if (!mapper.TryGetTypeSet(type, out var transformedTypeSet)) continue;
                GetRelatedObjects(mapper, transformedTypeSet, processedDetails, customization, cancel);
            }
        }
    }

    private ITypeSymbol PeelList(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (_listFieldGenerator.Applicable(obj, customization, typeSymbol, fieldName)
            && typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.TypeArguments[0];
        }

        if (_arrayFieldGenerator.Applicable(obj, customization, typeSymbol, fieldName))
        {
            return _arrayFieldGenerator.GetSubtype(typeSymbol);
        }

        return typeSymbol;
    }

    private IEnumerable<ITypeSymbol> TransformSymbol(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (_isGroupTester.IsGroup(typeSymbol))
        {
            yield return PeelList(obj, customization, typeSymbol, fieldName);
        }

        if (typeSymbol.Name == "IReadOnlyDictionary"
            && typeSymbol is INamedTypeSymbol named
            && named.TypeArguments.Length == 2)
        {
            if (_loquiObjectTester.IsLoqui(named.TypeArguments[0]))
            {
                yield return named.TypeArguments[0];
            }
            if (_loquiObjectTester.IsLoqui(named.TypeArguments[1]))
            {
                yield return named.TypeArguments[1];
            }
        }
        
        while (true)
        {
            var old = typeSymbol;
            typeSymbol = InternalTransformSymbol(obj, customization, typeSymbol, fieldName);
            if (ReferenceEquals(old, typeSymbol)) break;
        }

        yield return typeSymbol;
    }

    private ITypeSymbol InternalTransformSymbol(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization,
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (TryGetAsGroup(typeSymbol, out var replacement))
        {
            return replacement;
        }
        
        if (TryGetAsGenderedItem(obj, customization, typeSymbol, fieldName, out replacement))
        {
            return replacement;
        }

        if (typeSymbol.NullableAnnotation != NullableAnnotation.None)
        {
            return typeSymbol.WithNullableAnnotation(NullableAnnotation.None);
        }

        typeSymbol = PeelList(obj, customization, typeSymbol, fieldName);

        return typeSymbol;
    }

    private bool TryGetAsGroup(ITypeSymbol typeSymbol, out ITypeSymbol replacement)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && _isGroupTester.IsGroup(namedTypeSymbol))
        {
            replacement = namedTypeSymbol.TypeArguments[0];
            return true;
        }
        replacement = default!;
        return false;
    }

    private bool TryGetAsGenderedItem(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName,
        out ITypeSymbol replacement)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && _genderedTypeFieldGenerator.Applicable(obj, customization, namedTypeSymbol, fieldName))
        {
            replacement = namedTypeSymbol.TypeArguments[0];
            return true;
        }
        replacement = default!;
        return false;
    }
}