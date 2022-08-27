using System.Threading.Tasks;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using VerifyXunit;
using Xunit;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

[UsesVerify]
public class BootstrapTests
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
    public Task GeneratesStubForPeriod()
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
    public Task GeneratesStubForYaml()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Yaml;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenYamlConverter.
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesStubForUnknownFunction()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Newtonsoft;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenJsonConverter.Unknown
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesStubForUnknownParameter()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Newtonsoft;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        string someVariable = null!;
        MutagenJsonConverter.Instance.Convert(someVariable);
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesMultipleStubs()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.Yaml;

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
    
    [Fact]
    public async Task SkyrimModGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptySkyrimMod()
    {
        var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
        MutagenJsonConverter.Instance.Convert(mod);
    }
}";
        TestHelper.RunSourceGenerator(source);
    }
    
    [Fact]
    public async Task TestMod()
    {
        await TestHelper.Verify(GetModWith((sb) =>
        {
            using (var c = sb.Class("SubObject"))
            {
                c.Interfaces.Add("ILoquiObject");
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("public string SubField { get; set; }");
            }
            
            sb.AppendLine("int SomeInt { get; set; }");
            sb.AppendLine("List<int> SomeList { get; set; }");
            sb.AppendLine("SubObject SomeObject { get; set; }");
        }));
    }
    
    private string GetModWith(Action<StructuredStringBuilder> memberBuilder)
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Newtonsoft;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        using (var c = sb.Class("ISomeModGetter"))
        {
            c.Type = ObjectType.Interface;
            c.Interfaces.Add("IModGetter");
            c.Interfaces.Add("ILoquiObjectGetter");
        }
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
        }
        
        using (var c = sb.Class("SomeMod"))
        {
            c.BaseClass = "TestMod";
            c.Interfaces.Add("ISomeModGetter");
        }
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
        }

        using (var c = sb.Class("SerializationTests"))
        {
            c.Static = true;
        }

        using (sb.CurlyBrace())
        {
            sb.AppendLine("public static void TestCall()");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("var mod = new SomeMod();");
                sb.AppendLine("MutagenJsonConverter.Instance.Convert(mod);");
            }
        }

        return sb.ToString();
    }
}