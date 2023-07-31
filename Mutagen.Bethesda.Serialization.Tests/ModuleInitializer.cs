using System.Runtime.CompilerServices;
using Noggog.Verify.FileAbstractions;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

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