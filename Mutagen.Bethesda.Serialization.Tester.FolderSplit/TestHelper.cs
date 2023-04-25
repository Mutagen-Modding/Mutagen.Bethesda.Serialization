using System.IO.Abstractions;

namespace Mutagen.Bethesda.Serialization.Tester.FolderSplit;

public static class TestHelper
{
    private static bool AutoVerify = false;

    private static VerifySettings GetVerifySettings()
    {
        var verifySettings = new VerifySettings();
#if DEBUG
        if (AutoVerify)
        {
            verifySettings.AutoVerify();
        }
#else
        verifySettings.DisableDiff();
#endif
        return verifySettings;
    }
    
    public static Task VerifyFileSystem(IFileSystem fileSystem)
    {
        return Verifier.Verify(fileSystem, GetVerifySettings());
    }
}