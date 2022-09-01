using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class RelatedObjectAccumulator
{
    private readonly ListFieldGenerator _listFieldGenerator;
    private readonly IsLoquiObjectTester _loquiObjectTester;

    public RelatedObjectAccumulator(
        ListFieldGenerator listFieldGenerator,
        IsLoquiObjectTester loquiObjectTester)
    {
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
        objs.Add(details);
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
        if (!_loquiObjectTester.IsLoqui(details)) return;
        if (!processedDetails.Add(details.OriginalDefinition)) return;
        var baseType = mapper.TryGetBaseClass(details);
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
            
            var type = TransformSymbol(prop.Type);
            
            GetRelatedObjects(mapper, type, processedDetails, cancel);
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

    private ITypeSymbol TransformSymbol(ITypeSymbol typeSymbol)
    {
        while (true)
        {
            var old = typeSymbol;
            typeSymbol = InternalTransformSymbol(typeSymbol);
            if (ReferenceEquals(old, typeSymbol)) break;
        }

        return typeSymbol;
    }

    private ITypeSymbol InternalTransformSymbol(ITypeSymbol typeSymbol)
    {
        if (TryGetAsGroup(typeSymbol, out var replacement))
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
            && namedTypeSymbol.IsGenericType
            && namedTypeSymbol.TypeArguments.Length == 1
            && typeSymbol.Interfaces.Any(x => x.Name == "IGroupGetter"))
        {
            replacement = namedTypeSymbol.TypeArguments[0];
            return true;
        }
        replacement = default!;
        return false;
    }
}