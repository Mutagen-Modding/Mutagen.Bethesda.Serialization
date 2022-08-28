using Loqui;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public interface ISomeLoquiWithBase : ISomeLoquiWithBaseGetter, ILoquiObject
{
}

public interface ISomeLoquiWithBaseGetter : ILoquiObjectGetter
{
}

public class SomeLoquiWithBase : BaseLoqui, ISomeLoquiWithBase
{
    public override ILoquiRegistration Registration => new SomeLoquiWithBase_Registration();
}

internal class SomeLoquiWithBase_Registration : ARegistration
{
    public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 3, 0);
    public override Type ClassType => typeof(SomeLoquiWithBase);
    public override Type GetterType => typeof(ISomeLoquiWithBaseGetter);
    public override Type SetterType => typeof(ISomeLoquiWithBase);
    public override string Name => nameof(SomeLoquiWithBase);
}