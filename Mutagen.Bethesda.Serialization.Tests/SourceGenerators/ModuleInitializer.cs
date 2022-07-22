using System.Runtime.CompilerServices;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}