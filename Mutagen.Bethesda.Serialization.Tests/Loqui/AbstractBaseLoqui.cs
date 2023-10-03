using Loqui;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public interface IAbstractBaseLoqui : IAbstractBaseLoquiGetter, ILoquiObject
{
}

public interface IAbstractBaseLoquiGetter : ILoquiObjectGetter
{
}

public abstract class AbstractBaseLoqui : IAbstractBaseLoqui
{
    public virtual ILoquiRegistration Registration => new AbstractBaseLoqui_Registration();
    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }
}

internal class AbstractBaseLoqui_Registration : ARegistration
{
    public override Type ClassType => typeof(AbstractBaseLoqui);
    public override Type GetterType => typeof(IAbstractBaseLoquiGetter);
    public override Type SetterType => typeof(IAbstractBaseLoqui);
    public override string Name => nameof(AbstractBaseLoqui);
}