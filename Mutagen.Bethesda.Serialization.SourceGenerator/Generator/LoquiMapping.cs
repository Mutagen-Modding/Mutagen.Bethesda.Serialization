using Loqui;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class LoquiMapping
{
    private readonly IsLoquiObjectTester _isLoquiObjectTester;

    public LoquiMapping(
        IsLoquiObjectTester isLoquiObjectTester)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
    }
    
    public Type? TryGetBaseClass(ITypeSymbol typeSymbol)
    {
        if (LoquiRegistration.StaticRegister.TryGetRegisterByFullName(typeSymbol.ToString(), out var regis)
            && regis.ClassType.BaseType != null
            && _isLoquiObjectTester.IsLoqui(regis.ClassType.BaseType))
        {
            return regis.ClassType.BaseType;
        }

        return default;
    }
}