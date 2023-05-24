using System.Runtime.CompilerServices;
using Noggog.Verify.FileAbstractions;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        VerifyFileAbstractions.Enable();
        VerifyDiffPlex.Initialize();
    }
}