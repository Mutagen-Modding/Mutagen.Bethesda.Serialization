using Loqui;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public interface ISubclassLoquiA : ISubclassLoquiAGetter, IAbstractBaseLoqui
{
}

public interface ISubclassLoquiAGetter : IAbstractBaseLoquiGetter
{
}

public class SubclassLoquiA : AbstractBaseLoqui, ISubclassLoquiA
{
    public override ILoquiRegistration Registration => new SubclassLoquiA_Registration();
}

internal class SubclassLoquiA_Registration : ARegistration
{
    public override Type ClassType => typeof(SubclassLoquiA);
    public override Type GetterType => typeof(ISubclassLoquiAGetter);
    public override Type SetterType => typeof(ISubclassLoquiA);
    public override string Name => nameof(SubclassLoquiA);
}