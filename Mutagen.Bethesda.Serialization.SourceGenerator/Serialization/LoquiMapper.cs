using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public record LoquiTypeSet(
    ITypeSymbol? Direct,
    ITypeSymbol Getter,
    ITypeSymbol Setter)
{
    public virtual bool Equals(LoquiTypeSet? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SymbolEqualityComparer.Default.Equals(Setter, other.Setter);
    }

    public override int GetHashCode()
    {
        return SymbolEqualityComparer.Default.GetHashCode(Setter);
    }
}

public class LoquiMapping
{
    private readonly IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> _inheritingClassMapping;
    private readonly IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> _deepInheritingClassMapping;
    private readonly IReadOnlyCollection<LoquiTypeSet> _isAnInheritor;
    private readonly IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> _typeSetMapping;
    private readonly IsLoquiObjectTester _isLoquiObjectTester;

    public LoquiMapping(
        IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> typeSetMapping,
        IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> inheritingClassMapping, 
        IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> deepInheritingClassMapping, 
        ImmutableHashSet<LoquiTypeSet> isAnInheritor,
        IsLoquiObjectTester isLoquiObjectTester)
    {
        _inheritingClassMapping = inheritingClassMapping;
        _deepInheritingClassMapping = deepInheritingClassMapping;
        _isAnInheritor = isAnInheritor;
        _isLoquiObjectTester = isLoquiObjectTester;
        _typeSetMapping = typeSetMapping;
        
    }

    public ITypeSymbol? TryGetBaseClass(LoquiTypeSet? typeSymbol)
    {
        if (typeSymbol?.Direct?.BaseType != null
            && _isLoquiObjectTester.IsLoqui(typeSymbol.Direct.BaseType))
        {
            return typeSymbol.Direct.BaseType;
        }

        return default;
    }

    public IReadOnlyCollection<LoquiTypeSet> TryGetInheritingClasses(LoquiTypeSet typeSymbol)
    {
        if (_inheritingClassMapping.TryGetValue(typeSymbol, out var inheriting)) return inheriting;
        return Array.Empty<LoquiTypeSet>();
    }

    public IReadOnlyCollection<LoquiTypeSet> TryGetDeepInheritingClasses(LoquiTypeSet typeSymbol)
    {
        if (_deepInheritingClassMapping.TryGetValue(typeSymbol, out var inheriting)) return inheriting;
        return Array.Empty<LoquiTypeSet>();
    }

    public bool HasInheritingClasses(LoquiTypeSet typeSymbol) => TryGetInheritingClasses(typeSymbol).Count > 0;

    public bool TryGetTypeSet(ITypeSymbol typeSymbol, out LoquiTypeSet typeSet) => _typeSetMapping.TryGetValue(typeSymbol.OriginalDefinition, out typeSet);

    public bool IsInheritor(LoquiTypeSet type)
    {
        return _isAnInheritor.Contains(type);
    }
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
        INamedTypeSymbol[] mutagenSymbols = IterateAllMutagenSymbols(compilation, cancel)
            .WhereCastable<ITypeSymbol, INamedTypeSymbol>()
            .Where(x => _isLoquiObjectTester.IsLoqui(x))
            .ToArray();

        var typeSets = GetTypeSets(compilation, mutagenSymbols, cancel);

        var inheritingClassMapping = GetInheritanceSets(mutagenSymbols, typeSets, cancel);

        var inheritors = inheritingClassMapping
            .Where(kv => kv.Value.Count > 1)
            .Select(kv => (Key: typeSets[kv.Key.Getter], Value: kv.Value))
            .Where(kv => kv.Key.Direct != null)
            .Where(kv => !kv.Key.Direct!.Name.EndsWith("MajorRecord"))
            .SelectMany(x => x.Value)
            .ToImmutableHashSet();

        var deepInheritance = AnalyzeDeepInheritance(typeSets, inheritingClassMapping);

        return new LoquiMapping(
            typeSets,
            inheritingClassMapping,
            deepInheritance,
            inheritors,
            _isLoquiObjectTester);
    }

    private IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> AnalyzeDeepInheritance(
        IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> mapping,
        IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> rawInheritance)
    {
        var ret = new Dictionary<LoquiTypeSet, HashSet<LoquiTypeSet>>();
        foreach (var item in mapping.Values)
        {
            var set = new HashSet<LoquiTypeSet>();
            AnalyzeDeepInheritance(item, rawInheritance, set);
            set.Remove(item);
            ret[item] = set;
        }

        return ret;
    }

    private void AnalyzeDeepInheritance(
        LoquiTypeSet typeSet,
        IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> raw,
        HashSet<LoquiTypeSet> tracker)
    {
        if (!tracker.Add(typeSet)) return;
        if (raw.TryGetValue(typeSet, out var inheriting))
        {
            foreach (var item in inheriting)
            {
                AnalyzeDeepInheritance(item, raw, tracker);
            }
        }
    }
    
    private IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> GetTypeSets(
        Compilation compilation,
        INamedTypeSymbol[] mutagenSymbols,
        CancellationToken cancel)
    {
        var typeSets = new Dictionary<ITypeSymbol, LoquiTypeSet>(SymbolEqualityComparer.Default);
        foreach (var symb in mutagenSymbols)
        {
            cancel.ThrowIfCancellationRequested();
            var names = _nameRetriever.GetNames(symb);
            var generic = symb.TypeArguments.Length > 0 ? $"`{symb.TypeArguments.Length}" : string.Empty;
            var direct = compilation.GetTypeByMetadataName($"{symb.ContainingNamespace}.{names.Direct}{generic}");
            var getter = compilation.GetTypeByMetadataName($"{symb.ContainingNamespace}.{names.Getter}{generic}");
            var setter = compilation.GetTypeByMetadataName($"{symb.ContainingNamespace}.{names.Setter}{generic}");
            if (getter != null
                && setter != null)
            {
                typeSets[symb] = new(direct, getter, setter);
            }
        }

        return typeSets;
    }

    private IReadOnlyDictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> GetInheritanceSets(
        INamedTypeSymbol[] mutagenSymbols,
        IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> typeSets,
        CancellationToken cancel)
    {
        var inheritingClassMapping = new Dictionary<LoquiTypeSet, HashSet<LoquiTypeSet>>();
        
        foreach (var symb in mutagenSymbols)
        {
            cancel.ThrowIfCancellationRequested();

            if (symb.TypeKind != TypeKind.Interface) continue;
            if (symb.Name.EndsWith("Internal")) continue;
            
            if (!typeSets.TryGetValue(symb, out var set)) continue;
            
            ProcessInheritance(set, typeSets, inheritingClassMapping);
        }

        return inheritingClassMapping;
    }

    private void ProcessInheritance(
        LoquiTypeSet type,
        IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> typeSets,
        Dictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> mapping)
    {
        ProcessBaseTypeInheritance(type, typeSets, mapping);
        if (!typeSets.TryGetValue(type.Getter, out var typeSet)) return;
        HashSet<ITypeSymbol> passed = new(SymbolEqualityComparer.Default);
        passed.Add(typeSet.Getter);
        passed.Add(typeSet.Setter);
        foreach (var interf in type.Setter.AllInterfaces)
        {
            if (!typeSets.TryGetValue(interf, out var otherTypeSet)) continue;
            ProcessInterfaceTypeInheritance(type, otherTypeSet.Setter, passed, typeSets, mapping);
        }
    }

    private void ProcessBaseTypeInheritance(
        LoquiTypeSet type,
        IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> typeSets,
        Dictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> mapping)
    {
        var baseType = type.Direct?.BaseType;
        while (baseType != null
               && _isLoquiObjectTester.IsLoqui(baseType)
               && typeSets.TryGetValue(baseType, out var baseTypeSet))
        {
            mapping
                .GetOrAdd(baseTypeSet)
                .Add(type);
            baseType = baseType.BaseType;
        }
    }

    private void ProcessInterfaceTypeInheritance(
        LoquiTypeSet type,
        ITypeSymbol interf,
        HashSet<ITypeSymbol> passed,
        IReadOnlyDictionary<ITypeSymbol, LoquiTypeSet> typeSets,
        Dictionary<LoquiTypeSet, HashSet<LoquiTypeSet>> mapping)
    {
        if (_isLoquiObjectTester.IsLoqui(interf)
            && typeSets.TryGetValue(interf, out var interfTypeSet)
            && passed.Add(interfTypeSet.Getter)
            && passed.Add(interfTypeSet.Setter))
        {
            mapping
                .GetOrAdd(interfTypeSet)
                .Add(type);
            foreach (var otherInterface in interf.AllInterfaces)
            {
                if (!typeSets.TryGetValue(otherInterface, out var otherTypeSet)) continue;
                ProcessInterfaceTypeInheritance(otherTypeSet, otherInterface, passed, typeSets, mapping);
            }
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