using Loqui;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public interface ISomeLoqui : ISomeLoquiGetter, ILoquiObject
{
}

public interface ISomeLoquiGetter : ILoquiObjectGetter
{
}

public class SomeLoqui : ISomeLoqui
{
    public ILoquiRegistration Registration => new SomeLoqui_Registration();
    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }
}

internal class SomeLoqui_Registration : ARegistration
{
    public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 1, 0);
    public override Type ClassType => typeof(SomeLoqui);
    public override Type GetterType => typeof(ISomeLoquiGetter);
    public override Type SetterType => typeof(ISomeLoqui);
    public override string Name => nameof(SomeLoqui);
}