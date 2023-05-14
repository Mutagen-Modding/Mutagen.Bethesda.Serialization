using Mutagen.Bethesda.Serialization.Testing.Passthrough;

namespace Mutagen.Bethesda.Serialization.Tester.NoCustomization;

public class PassthroughTestBatteryImpl : PassthroughTestBattery
{
    protected override IPassthroughTest GetTest()
    {
        return new Test();
    }
}