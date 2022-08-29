using Loqui;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class ProtocolRegistration : IProtocolRegistration
{
    public void Register()
    {
        LoquiRegistration.Register(
            new SomeLoqui_Registration(),
            new BaseLoqui_Registration(),
            new SomeLoquiWithBase_Registration(),
            new AbstractBaseLoqui_Registration(),
            new SubclassLoquiA_Registration(),
            new SubclassLoquiB_Registration());
    }
}