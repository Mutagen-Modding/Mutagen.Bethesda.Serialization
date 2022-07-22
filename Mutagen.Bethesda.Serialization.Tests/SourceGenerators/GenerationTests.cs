using Xunit;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

[UsesVerify]
public class GenerationTests
{
    [Fact]
    public Task NoGeneration()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Newtonsoft;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesStub()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Newtonsoft;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenJsonConverter.
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesMultipleStubs()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Newtonsoft;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenJsonConverter.
    }

    [Fact]
    public void BasicPassthrough2()
    {
        MutagenYamlConverter.
    }
}";
       
        return TestHelper.Verify(source);
    }
}