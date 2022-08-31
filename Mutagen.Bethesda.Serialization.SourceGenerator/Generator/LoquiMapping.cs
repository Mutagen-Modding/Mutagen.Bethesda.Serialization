using Loqui;
using Microsoft.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class LoquiMapping
{
    private readonly IsLoquiObjectTester _isLoquiObjectTester;
    private readonly Lazy<Dictionary<ObjectKey, List<ILoquiRegistration>>> _inheritingClassMapping;

    public LoquiMapping(
        IsLoquiObjectTester isLoquiObjectTester)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
        _inheritingClassMapping = new Lazy<Dictionary<ObjectKey, List<ILoquiRegistration>>>(PopulateInheritingClassMapping);
    }

    public ITypeSymbol? TryGetBaseClass(ITypeSymbol typeSymbol, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (typeSymbol.BaseType != null
            && _isLoquiObjectTester.IsLoqui(typeSymbol.BaseType))
        {
            return typeSymbol.BaseType;
        }

        return default;
    }

    public IReadOnlyList<ILoquiRegistration> TryGetInheritingClasses(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (!LoquiRegistration.StaticRegister.TryGetRegisterByFullName(typeSymbol.ToString(), out var regis)) return Array.Empty<ILoquiRegistration>();
        if (_inheritingClassMapping.Value.TryGetValue(regis.ObjectKey, out var inheriting)) return inheriting;
        return Array.Empty<ILoquiRegistration>();
    }

    public bool HasInheritingClasses(ITypeSymbol typeSymbol, CancellationToken cancel) => TryGetInheritingClasses(typeSymbol, cancel).Count > 0;

    private Dictionary<ObjectKey, List<ILoquiRegistration>> PopulateInheritingClassMapping()
    {
        var ret = new Dictionary<ObjectKey, List<ILoquiRegistration>>();
        foreach (var registerRegistration in LoquiRegistration.StaticRegister.Registrations)
        {
            Process(registerRegistration, ret);
        }

        return ret;
    }

    private void Process(
        ILoquiRegistration registration,
        Dictionary<ObjectKey, List<ILoquiRegistration>> mapping)
    {
        Type? baseType = registration.ClassType.BaseType;
        while (baseType != null)
        {
            if (!LoquiRegistration.StaticRegister.TryGetRegister(baseType, out var baseRegis)) return;
            mapping.GetOrAdd(baseRegis.ObjectKey).Add(registration);
            baseType = baseRegis.ClassType.BaseType;
        }
    }
}