using AutoFixture;
using AutoFixture.Xunit2;
using Mutagen.Bethesda.Testing.AutoData;

namespace Mutagen.Bethesda.Serialization.Tests;

public class TestAutoDataAttribute : AutoDataAttribute
{
    public TestAutoDataAttribute(
        GameRelease Release = GameRelease.SkyrimSE,
        bool ConfigureMembers = false, 
        bool UseMockFileSystem = true)
        : base(() =>
        {
            return new Fixture()
                .Customize(new MutagenDefaultCustomization(
                    useMockFileSystem: UseMockFileSystem,
                    configureMembers: ConfigureMembers,
                    release: Release));
        })
    {
    }
}