using FluentAssertions;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class LoquiNameRetrieverTests
{
    [Theory]
    [DefaultAutoData]
    public void Direct(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("SomeClass");
        names.Direct.Should().Be("SomeClass");
        names.Getter.Should().Be("ISomeClassGetter");
        names.Setter.Should().Be("ISomeClass");
    }
    
    [Theory]
    [DefaultAutoData]
    public void Getter(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("ISomeClassGetter");
        names.Direct.Should().Be("SomeClass");
        names.Getter.Should().Be("ISomeClassGetter");
        names.Setter.Should().Be("ISomeClass");
    }
    
    [Theory]
    [DefaultAutoData]
    public void Setter(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("ISomeClass");
        names.Direct.Should().Be("SomeClass");
        names.Getter.Should().Be("ISomeClassGetter");
        names.Setter.Should().Be("ISomeClass");
    }
    
    [Theory]
    [DefaultAutoData]
    public void InterfaceStartsWithI(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("IIngredient");
        names.Direct.Should().Be("Ingredient");
        names.Getter.Should().Be("IIngredientGetter");
        names.Setter.Should().Be("IIngredient");
    }
    
    [Theory]
    [DefaultAutoData]
    public void ClassStartsWithI(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("Ingredient");
        names.Direct.Should().Be("Ingredient");
        names.Getter.Should().Be("IIngredientGetter");
        names.Setter.Should().Be("IIngredient");
    }
    
    [Theory]
    [DefaultAutoData]
    public void Abstract(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("IAIngredient");
        names.Direct.Should().Be("AIngredient");
        names.Getter.Should().Be("IAIngredientGetter");
        names.Setter.Should().Be("IAIngredient");
    }
}