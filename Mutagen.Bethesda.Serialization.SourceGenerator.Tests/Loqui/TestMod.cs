using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public partial interface ITestMod : ITestModGetter, ILoquiObject, IMod
{
}

public partial interface ITestModGetter : ILoquiObjectGetter, IModGetter
{
}

public partial class TestMod : ITestMod
{
}

internal class TestMod_Registration : ARegistration
{
    public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 8, 0);
    public override Type ClassType => typeof(TestMod);
    public override Type GetterType => typeof(ITestModGetter);
    public override Type SetterType => typeof(ITestMod);
    public override string Name => nameof(TestMod);
}