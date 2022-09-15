using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Skyrim;
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
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

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
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenTestConverter.
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesStubForUnknownFunction()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenTestConverter.Unknown
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesStubForUnknownParameter()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        string someVariable = null!;
        MutagenTestConverter.Instance.Convert(someVariable);
    }
}";
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesMultipleStubs()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Yaml;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenTestConverter.
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
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptySkyrimMod()
    { 
        var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        MutagenTestConverter.Instance.Serialize(mod, stream);
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Should().BeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .Should().BeEmpty();
    }
    
    [Fact]
    public async Task SkyrimModInterfaceGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptySkyrimMod()
    { 
        ISkyrimModGetter mod = null!;
        var stream = new MemoryStream();
        MutagenTestConverter.Instance.Serialize(mod, stream);
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Should().BeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .Should().BeEmpty();
    }

    [Fact]
    public async Task TestMod()
    {
        await TestHelper.Verify(GetModWith((sb) =>
        {
            sb.AppendLine("int SomeInt { get; set; }");
            sb.AppendLine("List<int> SomeList { get; set; }");
            sb.AppendLine("SomeLoqui SomeObject { get; set; }");
        }));
    }

    [Fact]
    public async Task SomeLoqui()
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        using (var c = sb.Class("SerializationTests"))
        {
            c.Static = true;
        }

        using (sb.CurlyBrace())
        {
            sb.AppendLine("public static void TestCall()");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("var obj = new SomeLoqui();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(obj);");
            }
        }

        await TestHelper.Verify(sb.ToString());
    }

    [Fact]
    public async Task DoubleCall()
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        using (var c = sb.Class("SerializationTests"))
        {
            c.Static = true;
        }

        using (sb.CurlyBrace())
        {
            sb.AppendLine("public static void TestCall()");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("var obj = new SomeLoqui();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(obj);");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(obj);");
            }
        }

        await TestHelper.Verify(sb.ToString());
    }

    [Fact]
    public async Task TwoDifferentCalls()
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        sb.AppendLine("public partial interface ITestModGetter : IModGetter, ILoquiObject");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("SomeLoqui SomeObject { get; set; }");
        }
        
        using (var c = sb.Class("TestMod"))
        {
            c.Partial = true;
            c.BaseClass = "AMod";
            c.Interfaces.Add("ITestModGetter");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("SomeLoqui SomeObject { get; set; }");
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
                sb.AppendLine("var mod = new TestMod();");
                sb.AppendLine("var obj = new SomeLoqui();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(mod);");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(obj);");
            }
        }

        await TestHelper.Verify(sb.ToString());
    }
    
    private string GetModWith(Action<StructuredStringBuilder> memberBuilder)
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;");
        
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
                sb.AppendLine("MutagenTestConverter.Instance.Convert(mod);");
            }
        }

        return sb.ToString();
    }
}