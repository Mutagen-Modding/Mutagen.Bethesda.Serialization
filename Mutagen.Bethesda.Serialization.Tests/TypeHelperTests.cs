using FluentAssertions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Serialization.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class TypeHelperTests
{
    [Fact]
    public void GetTypeFromStringTypical()
    {
        TypeHelper.GetTypeFromString(
                "Npc", 
                "Mutagen.Bethesda.Fallout4")
            .Should().Be(typeof(Npc));
    }
    
    [Fact]
    public void GetTypeFromStringGeneric()
    {
        var type = TypeHelper.GetTypeFromString(
            "ObjectModFormLinkIntProperty<Npc+Property>",
            "Mutagen.Bethesda.Fallout4");
        type.Should().Be(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
    }
    
    [Fact]
    public void GetNameWithDeclaringTypeTypical()
    {
        TypeHelper.GetNameWithDeclaringType(typeof(Npc))
            .Should().Be("Npc");
    }
    
    [Fact]
    public void GetNameWithDeclaringTypeGeneric()
    {
        TypeHelper.GetNameWithDeclaringType(typeof(ObjectModFormLinkIntProperty<Npc.Property>))
            .Should().Be("ObjectModFormLinkIntProperty<Npc+Property>");
    }
    
    [Fact]
    public void GetTypeToSerializeTypical()
    {
        TypeHelper.GetTypeToSerialize(typeof(Npc))
            .Should().Be(typeof(Npc));
        TypeHelper.GetTypeToSerialize(typeof(INpcGetter))
            .Should().Be(typeof(Npc));
        TypeHelper.GetTypeToSerialize(typeof(INpc))
            .Should().Be(typeof(Npc));
    }
    
    [Fact]
    public void GetTypeToSerializeGeneric()
    {
        TypeHelper.GetTypeToSerialize(typeof(ObjectModFormLinkIntProperty<Npc.Property>))
            .Should().Be(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
        TypeHelper.GetTypeToSerialize(typeof(IObjectModFormLinkIntPropertyGetter<Npc.Property>))
            .Should().Be(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
        TypeHelper.GetTypeToSerialize(typeof(IObjectModFormLinkIntProperty<Npc.Property>))
            .Should().Be(typeof(ObjectModFormLinkIntProperty<Npc.Property>));
    }
}