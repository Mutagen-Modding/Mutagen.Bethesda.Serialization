using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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
        CancellationToken cancel)
    {
        if (!mapper.TryGetTypeSet(details, out var typeSet)) return ImmutableHashSet<LoquiTypeSet>.Empty;
        
        var objs = new HashSet<LoquiTypeSet>();
        GetRelatedObjects(mapper, typeSet, objs, cancel);
        return objs.ToImmutableHashSet<LoquiTypeSet>();
    }

    private void GetRelatedObjects(
        LoquiMapping mapper,
        LoquiTypeSet typeSet, 
        HashSet<LoquiTypeSet> processedDetails,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var details = typeSet.Getter;
        if (!_loquiObjectTester.IsLoqui(details)) return;
        if (!processedDetails.Add(typeSet)) return;
        var baseType = mapper.TryGetBaseClass(typeSet);
        if (baseType != null
            && mapper.TryGetTypeSet(baseType, out var baseSet))
        {
            GetRelatedObjects(mapper, baseSet, processedDetails, cancel);
        }

        var inheriting = mapper.TryGetInheritingClasses(typeSet);
        foreach (var inherit in inheriting)
        {
            cancel.ThrowIfCancellationRequested();
            GetRelatedObjects(mapper, inherit, processedDetails, cancel);
        }
        foreach (var memb in details.GetMembers())
        {
            cancel.ThrowIfCancellationRequested();
            if (memb is not IPropertySymbol prop) continue;
            if (prop.IsStatic) continue;

            foreach (var type in TransformSymbol(prop.Type))
            {
                if (!mapper.TryGetTypeSet(type, out var transformedTypeSet)) continue;
                GetRelatedObjects(mapper, transformedTypeSet, processedDetails, cancel);
            }
        }
    }

    private ITypeSymbol PeelList(ITypeSymbol typeSymbol)
    {
        if (_listFieldGenerator.Applicable(typeSymbol)
            && typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.TypeArguments[0];
        }

        if (_arrayFieldGenerator.Applicable(typeSymbol))
        {
            return _arrayFieldGenerator.GetSubtype(typeSymbol);
        }

        return typeSymbol;
    }

    private IEnumerable<ITypeSymbol> TransformSymbol(ITypeSymbol typeSymbol)
    {
        if (_isGroupTester.IsGroup(typeSymbol))
        {
            yield return PeelList(typeSymbol);
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
            typeSymbol = InternalTransformSymbol(typeSymbol);
            if (ReferenceEquals(old, typeSymbol)) break;
        }

        yield return typeSymbol;
    }

    private ITypeSymbol InternalTransformSymbol(ITypeSymbol typeSymbol)
    {
        if (TryGetAsGroup(typeSymbol, out var replacement))
        {
            return replacement;
        }
        
        if (TryGetAsGenderedItem(typeSymbol, out replacement))
        {
            return replacement;
        }

        if (typeSymbol.NullableAnnotation != NullableAnnotation.None)
        {
            return typeSymbol.WithNullableAnnotation(NullableAnnotation.None);
        }

        typeSymbol = PeelList(typeSymbol);

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

    private bool TryGetAsGenderedItem(ITypeSymbol typeSymbol, out ITypeSymbol replacement)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && _genderedTypeFieldGenerator.Applicable(namedTypeSymbol))
        {
            replacement = namedTypeSymbol.TypeArguments[0];
            return true;
        }
        replacement = default!;
        return false;
    }
}