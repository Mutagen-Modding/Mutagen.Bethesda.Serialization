using Shouldly;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class LoquiNameRetrieverTests
{
    [Theory]
    [DefaultAutoData]
    public void Direct(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("SomeClass");
        names.Direct.ShouldBe("SomeClass");
        names.Getter.ShouldBe("ISomeClassGetter");
        names.Setter.ShouldBe("ISomeClass");
    }
    
    [Theory]
    [DefaultAutoData]
    public void Getter(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("ISomeClassGetter");
        names.Direct.ShouldBe("SomeClass");
        names.Getter.ShouldBe("ISomeClassGetter");
        names.Setter.ShouldBe("ISomeClass");
    }
    
    [Theory]
    [DefaultAutoData]
    public void Setter(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("ISomeClass");
        names.Direct.ShouldBe("SomeClass");
        names.Getter.ShouldBe("ISomeClassGetter");
        names.Setter.ShouldBe("ISomeClass");
    }
    
    [Theory]
    [DefaultAutoData]
    public void InterfaceStartsWithI(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("IIngredient");
        names.Direct.ShouldBe("Ingredient");
        names.Getter.ShouldBe("IIngredientGetter");
        names.Setter.ShouldBe("IIngredient");
    }
    
    [Theory]
    [DefaultAutoData]
    public void ClassStartsWithI(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("Ingredient");
        names.Direct.ShouldBe("Ingredient");
        names.Getter.ShouldBe("IIngredientGetter");
        names.Setter.ShouldBe("IIngredient");
    }
    
    [Theory]
    [DefaultAutoData]
    public void Abstract(LoquiNameRetriever sut)
    {
        var names = sut.GetNames("IAIngredient");
        names.Direct.ShouldBe("AIngredient");
        names.Getter.ShouldBe("IAIngredientGetter");
        names.Setter.ShouldBe("IAIngredient");
    }
}