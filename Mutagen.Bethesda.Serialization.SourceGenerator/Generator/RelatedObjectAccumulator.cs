using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class RelatedObjectAccumulator
{
    private readonly IsGroupTester _isGroupTester;
    private readonly GenderedTypeFieldGenerator _genderedTypeFieldGenerator;
    private readonly ListFieldGenerator _listFieldGenerator;
    private readonly IsLoquiObjectTester _loquiObjectTester;

    public RelatedObjectAccumulator(
        IsGroupTester isGroupTester,
        GenderedTypeFieldGenerator genderedTypeFieldGenerator,
        ListFieldGenerator listFieldGenerator,
        IsLoquiObjectTester loquiObjectTester)
    {
        _isGroupTester = isGroupTester;
        _genderedTypeFieldGenerator = genderedTypeFieldGenerator;
        _listFieldGenerator = listFieldGenerator;
        _loquiObjectTester = loquiObjectTester;
    }
    
    public ImmutableHashSet<ITypeSymbol> GetRelatedObjects(
        LoquiMapping mapper,
        ITypeSymbol details, 
        CancellationToken cancel)
    {
        var objs = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        GetRelatedObjects(mapper, details, objs, cancel);
        if (mapper.TryGetTypeSet(details, out var typeSet))
        {
            objs.Add(typeSet.Getter);
        }

        return objs.ToImmutableHashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
    }

    private void GetRelatedObjects(
        LoquiMapping mapper,
        ITypeSymbol details, 
        HashSet<ITypeSymbol> processedDetails,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        details = PeelList(details);
        if (!mapper.TryGetTypeSet(details.OriginalDefinition, out var typeSet)) return;
        details = typeSet.Getter;
        if (!_loquiObjectTester.IsLoqui(details)) return;
        if (!processedDetails.Add(details.OriginalDefinition)) return;
        var baseType = mapper.TryGetBaseClass(typeSet.Direct);
        if (baseType != null)
        {
            GetRelatedObjects(mapper, baseType, processedDetails, cancel);
        }

        var inheriting = mapper.TryGetInheritingClasses(details);
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
                GetRelatedObjects(mapper, type, processedDetails, cancel);
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

        return typeSymbol;
    }

    private IEnumerable<ITypeSymbol> TransformSymbol(ITypeSymbol typeSymbol)
    {
        if (_isGroupTester.IsGroup(typeSymbol))
        {
            yield return typeSymbol;
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