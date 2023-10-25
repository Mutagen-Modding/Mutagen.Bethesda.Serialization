using System.Runtime.CompilerServices;
using Noggog.Verify.FileAbstractions;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        VerifyFileAbstractions.Enable();
        VerifyDiffPlex.Initialize();
    }
}