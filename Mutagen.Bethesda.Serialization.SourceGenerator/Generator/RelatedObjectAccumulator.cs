using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class RelatedObjectAccumulator
{
    private readonly LoquiMapping _loquiMapping;
    private readonly IsLoquiObjectTester _loquiObjectTester;

    public RelatedObjectAccumulator(
        LoquiMapping loquiMapping,
        IsLoquiObjectTester loquiObjectTester)
    {
        _loquiMapping = loquiMapping;
        _loquiObjectTester = loquiObjectTester;
    }
    
    public ImmutableHashSet<ITypeSymbol> GetRelatedObjects(
        Compilation compilation,
        ITypeSymbol details, 
        CancellationToken cancel)
    {
        var objs = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        GetRelatedObjects(compilation, details, objs, cancel);
        objs.Add(details);
        return objs.ToImmutableHashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
    }

    private void GetRelatedObjects(
        Compilation compilation,
        ITypeSymbol details, 
        HashSet<ITypeSymbol> processedDetails,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (!_loquiObjectTester.IsLoqui(details)) return;
        if (!processedDetails.Add(details.OriginalDefinition)) return;
        var baseType = _loquiMapping.TryGetBaseClass(details);
        if (baseType != null)
        {
            var baseTypeSymbol = compilation.GetTypeByMetadataName(baseType.FullName!);
            if (baseTypeSymbol != null)
            {
                GetRelatedObjects(compilation, baseTypeSymbol, processedDetails, cancel);
            }
        }

        var inheriting = _loquiMapping.TryGetInheritingClasses(details);
        foreach (var inherit in inheriting)
        {
            var inheritTypeSymbol = compilation.GetTypeByMetadataName(inherit.FullName!);
            if (inheritTypeSymbol != null)
            {
                GetRelatedObjects(compilation, inheritTypeSymbol, processedDetails, cancel);
            }
        }
        foreach (var memb in details.GetMembers())
        {
            cancel.ThrowIfCancellationRequested();
            if (memb is not IPropertySymbol prop) continue;
            if (prop.IsStatic) continue;
            
            var type = TransformSymbol(prop.Type);
            
            GetRelatedObjects(compilation, type, processedDetails, cancel);
        }
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