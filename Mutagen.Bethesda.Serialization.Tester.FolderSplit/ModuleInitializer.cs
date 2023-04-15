using System.Runtime.CompilerServices;
using Noggog.Verify.FileAbstractions;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

public class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyFileAbstractions.Enable();
        VerifyDiffPlex.Initialize();
    }
}