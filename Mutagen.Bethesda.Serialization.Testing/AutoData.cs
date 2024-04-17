using AutoFixture;
using AutoFixture.Xunit2;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.Testing;

public class TestAutoDataAttribute : AutoDataAttribute
{
    public TestAutoDataAttribute(
        GameRelease Release = GameRelease.Fallout4,
        bool ConfigureMembers = false, 
        TargetFileSystem FileSystem = TargetFileSystem.Fake)
        : base(() =>
        {
            return new Fixture()
                .Customize(new MutagenDefaultCustomization(
                    targetFileSystem: FileSystem,
                    configureMembers: ConfigureMembers,
                    release: Release))
                .Customize(
                    new MutagenConcreteModsCustomization(release: Release, configureMembers: ConfigureMembers));
        })
    {
    }
}