using Loqui;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public interface IBaseLoqui : IBaseLoquiGetter, ILoquiObject
{
}

public interface IBaseLoquiGetter : ILoquiObjectGetter
{
}

public class BaseLoqui : IBaseLoqui
{
    public virtual ILoquiRegistration Registration => new BaseLoqui_Registration();
    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }
}

internal class BaseLoqui_Registration : ARegistration
{
    public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 2, 0);
    public override Type ClassType => typeof(BaseLoqui);
    public override Type GetterType => typeof(IBaseLoquiGetter);
    public override Type SetterType => typeof(IBaseLoqui);
    public override string Name => nameof(BaseLoqui);
}