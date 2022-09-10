using System.Runtime.CompilerServices;
using VerifyTests;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        VerifyDiffPlex.Initialize();
    }
}