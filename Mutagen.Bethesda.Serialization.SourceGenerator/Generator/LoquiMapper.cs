using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class LoquiMapping
{
    private readonly Dictionary<ITypeSymbol, List<ITypeSymbol>> _inheritingClassMapping;
    private readonly Dictionary<ITypeSymbol, ITypeSymbol> _toDirectMapping;
    private readonly IsLoquiObjectTester _isLoquiObjectTester;

    public LoquiMapping(
        Dictionary<ITypeSymbol, ITypeSymbol> toDirectMapping,
        Dictionary<ITypeSymbol, List<ITypeSymbol>> inheritingClassMapping, 
        IsLoquiObjectTester isLoquiObjectTester)
    {
        _inheritingClassMapping = inheritingClassMapping;
        _isLoquiObjectTester = isLoquiObjectTester;
        _toDirectMapping = toDirectMapping;
    }

    public ITypeSymbol? TryGetBaseClass(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.BaseType != null
            && _isLoquiObjectTester.IsLoqui(typeSymbol.BaseType))
        {
            return typeSymbol.BaseType;
        }

        return default;
    }

    public IReadOnlyList<ITypeSymbol> TryGetInheritingClasses(ITypeSymbol typeSymbol)
    {
        if (_inheritingClassMapping.TryGetValue(typeSymbol, out var inheriting)) return inheriting;
        return Array.Empty<ITypeSymbol>();
    }

    public bool HasInheritingClasses(ITypeSymbol typeSymbol) => TryGetInheritingClasses(typeSymbol).Count > 0;

    public bool TryGetDirectClass(ITypeSymbol typeSymbol, out ITypeSymbol directClass) => _toDirectMapping.TryGetValue(typeSymbol, out directClass);
}

public class LoquiMapper
{
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly IsLoquiObjectTester _isLoquiObjectTester;

    public LoquiMapper(
        LoquiNameRetriever nameRetriever,
        IsLoquiObjectTester isLoquiObjectTester)
    {
        _nameRetriever = nameRetriever;
        _isLoquiObjectTester = isLoquiObjectTester;
    }
    
    public LoquiMapping GetMappings(Compilation compilation, CancellationToken cancel)
    {
        var inheritingClassMapping = new Dictionary<ITypeSymbol, List<ITypeSymbol>>(SymbolEqualityComparer.Default);
        var toDirectMapping = new Dictionary<ITypeSymbol, ITypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var symb in IterateAllMutagenSymbols(compilation, cancel)
                     .WhereCastable<ITypeSymbol, INamedTypeSymbol>())
        {
            cancel.ThrowIfCancellationRequested();
            if (!_isLoquiObjectTester.IsLoqui(symb)) continue;

            ProcessInheritance(symb, inheritingClassMapping);

            var names = _nameRetriever.GetNames(symb);
            var direct = compilation.GetTypeByMetadataName($"{symb.ContainingNamespace}.{names.Direct}");
            if (direct != null)
            {
                toDirectMapping[symb] = direct;
            }
        }

        return new LoquiMapping(
            toDirectMapping,
            inheritingClassMapping,
            _isLoquiObjectTester);
    }

    private void ProcessInheritance(
        ITypeSymbol type,
        Dictionary<ITypeSymbol, List<ITypeSymbol>> mapping)
    {
        var baseType = type.BaseType;
        while (baseType != null
               && _isLoquiObjectTester.IsLoqui(baseType))
        {
            mapping.GetOrAdd(baseType).Add(type);
            baseType = baseType.BaseType;
        }
    }
    
    private IEnumerable<INamedTypeSymbol> IterateAllMutagenSymbols(Compilation compilation, CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var stack = new Stack<INamespaceSymbol>();
        stack.Push(compilation.GlobalNamespace);

        while (stack.Count > 0)
        {
            cancel.ThrowIfCancellationRequested();
            var @namespace = stack.Pop();

            foreach (var member in @namespace.GetMembers())
            {
                cancel.ThrowIfCancellationRequested();
                if (member is INamespaceSymbol ns)
                {
                    if (ns.ToString().StartsWith("Mutagen"))
                    {
                        stack.Push(ns);
                    }
                }
                else if (member is INamedTypeSymbol named
                         && _isLoquiObjectTester.IsLoqui(named))
                {
                    yield return named;
                }
            }
        }
    }
}