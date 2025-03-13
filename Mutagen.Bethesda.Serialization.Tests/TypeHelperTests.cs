using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Serialization.Utility;
using Shouldly;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class TypeHelperTests
{
    [Fact]
    public void GetTypeFromStringTypical()
    {
        TypeHelper.GetTypeFromString(
                "Npc", 
                "Mutagen.Bethesda.Fallout4")
            .ShouldBe(typeof(Npc));
    }
    
    [Fact]
    public void GetTypeFromStringGeneric()
    {
        var type = TypeHelper.GetTypeFromString(
            "ObjectModFormLinkIntProperty<Npc+Property>",
            "Mutagen.Bethesda.Fallout4");
        type.ShouldBe(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
    }
    
    [Fact]
    public void GetNameWithDeclaringTypeTypical()
    {
        TypeHelper.GetNameWithDeclaringType(typeof(Npc))
            .ShouldBe("Npc");
    }
    
    [Fact]
    public void GetNameWithDeclaringTypeGeneric()
    {
        TypeHelper.GetNameWithDeclaringType(typeof(ObjectModFormLinkIntProperty<Npc.Property>))
            .ShouldBe("ObjectModFormLinkIntProperty<Npc+Property>");
    }
    
    [Fact]
    public void GetTypeToSerializeTypical()
    {
        TypeHelper.GetTypeToSerialize(typeof(Npc))
            .ShouldBe(typeof(Npc));
        TypeHelper.GetTypeToSerialize(typeof(INpcGetter))
            .ShouldBe(typeof(Npc));
        TypeHelper.GetTypeToSerialize(typeof(INpc))
            .ShouldBe(typeof(Npc));
    }
    
    [Fact]
    public void GetTypeToSerializeGeneric()
    {
        TypeHelper.GetTypeToSerialize(typeof(ObjectModFormLinkIntProperty<Npc.Property>))
            .ShouldBe(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
        TypeHelper.GetTypeToSerialize(typeof(IObjectModFormLinkIntPropertyGetter<Npc.Property>))
            .ShouldBe(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
        TypeHelper.GetTypeToSerialize(typeof(IObjectModFormLinkIntProperty<Npc.Property>))
            .ShouldBe(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
    }
}