using Mutagen.Bethesda.Serialization.Utility;
using Shouldly;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class StripNullTests
{
    [Fact]
    public void FloatStrip()
    {
        float? f1 = 2.4f;
        float f2 = SerializationHelper.StripNull(f1, "Test");
        f2.ShouldBe(f1.Value);
    }
}