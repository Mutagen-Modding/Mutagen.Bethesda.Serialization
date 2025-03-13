using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using Shouldly;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Bootstrap;

public class BootstrapTests
{
    [Fact]
    public Task NoGeneration()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
    }
}";
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task GeneratesStubForPeriod()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenTestConverter.
    }
}";
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task GeneratesStubForUnknownFunction()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        MutagenTestConverter.Unknown
    }
}";
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task GeneratesStubForUnknownParameter()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;

public class BasicPassthroughs
{
    [Fact]
    public void BasicPassthrough()
    {
        string someVariable = null!;
        MutagenTestConverter.Instance.Convert(someVariable);
    }
}";
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task GeneratesMultipleStubs()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
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
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public async Task SkyrimModGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptySkyrimMod()
    { 
        var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        var workEngine = new InlineWorkDropoff();

        MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine);
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task OblivionModGenerationBootstrapper()
    {
        var source = 
            """
            using Mutagen.Bethesda.Serialization.Tests;
            using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
            using Mutagen.Bethesda.Oblivion;
            using Noggog.WorkEngine;

            namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

            public class SerializationTests
            {
                public void EmptyOblivionMod()
                { 
                    var mod = new OblivionMod(ModKey.Null);
                    var stream = new MemoryStream();
                    var workEngine = new InlineWorkDropoff();

                    MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine);
                }
            }
            """;
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task OblivionFilePerRecordGenerationBootstrapper()
    {
        var source = 
            """
            using Mutagen.Bethesda.Serialization.Tests;
            using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
            using Mutagen.Bethesda.Oblivion;
            using Noggog.WorkEngine;

            namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

            public class SerializationTests
            {
                public void EmptyOblivionMod()
                { 
                    var mod = new OblivionMod(ModKey.Null);
                    var stream = new MemoryStream();
                    var workEngine = new InlineWorkDropoff();

                    MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine);
                }
            }
            
            public class Customization : ICustomize
            {
                public void Customize(ICustomizationBuilder builder)
                {
                    builder
                        .OmitLastModifiedData()
                        .OmitTimestampData()
                        .FilePerRecord();
                }
            }
            
            public class ModHeaderStatsCustomization : ICustomize<IModStatsGetter>
            {
                public void CustomizeFor(ICustomizationBuilder<IModStatsGetter> builder)
                {
                    builder.Omit(x => x.NextFormID);
                    builder.Omit(x => x.NumRecords);
                }
            }
            
            public class CellCustomization : ICustomize<ICellGetter>
            {
                public void CustomizeFor(ICustomizationBuilder<ICellGetter> builder)
                {
                    builder.EmbedRecordsInSameFile(x => x.Temporary)
                        .EmbedRecordsInSameFile(x => x.Persistent)
                        .EmbedRecordsInSameFile(x => x.Landscape);
                }
            }
            
            public class WorldspaceCustomization : ICustomize<IWorldspaceGetter>
            {
                public void CustomizeFor(ICustomizationBuilder<IWorldspaceGetter> builder)
                {
                    builder.EmbedRecordsInSameFile(x => x.TopCell);
                }
            }
            """;
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task Fallout4ModGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Fallout4;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptyFallout4Mod()
    { 
        var mod = new Fallout4Mod(ModKey.Null);
        var stream = new MemoryStream();
        var workEngine = new InlineWorkDropoff();

        MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine);
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task StarfieldModGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Starfield;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptyStarfieldMod()
    { 
        var mod = new StarfieldMod(ModKey.Null);
        var stream = new MemoryStream();
        var workEngine = new InlineWorkDropoff();

        MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine);
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task CastBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptySkyrimMod()
    { 
        var stream = new MemoryStream();
        var workEngine = new InlineWorkDropoff();

        MutagenTestConverter.Instance.Serialize((SkyrimMod)null, stream, workEngine: workEngine);
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task SkyrimModFilePerRecordGenerationBootstrapper()
    {
        var source =
            """
            using Mutagen.Bethesda.Serialization.Tests;
            using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
            using Mutagen.Bethesda.Skyrim;
            using Noggog.WorkEngine;

            namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

            public class SerializationTests
            {
                public void EmptySkyrimMod()
                {
                    var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
                    var stream = new MemoryStream();
                    var workEngine = new InlineWorkDropoff();
            
                    MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine);
                }
            }

            public class Customization : ICustomize
            {
                public void Customize(ICustomizationBuilder builder)
                {
                    builder.FilePerRecord();
                }
            }
            
            public class ModHeaderCustomization : ICustomize<ISkyrimModHeaderGetter>
            {
                public void CustomizeFor(ICustomizationBuilder<ISkyrimModHeaderGetter> builder)
                {
                    builder.Omit(x => x.OverriddenForms);
                }
            }
            
            public class ModHeaderStatsCustomization : ICustomize<IModStatsGetter>
            {
                public void CustomizeFor(ICustomizationBuilder<IModStatsGetter> builder)
                {
                    builder.Omit(x => x.NextFormID);
                    builder.Omit(x => x.NumRecords);
                }
            }
            
            public class ConditionCustomization : ICustomize<IConditionGetter>
            {
                public void CustomizeFor(ICustomizationBuilder<IConditionGetter> builder)
                {z
                    builder.Omit(x => x.Unknown1);
                }
            }
            
            public class CellCustomization : ICustomize<ICellGetter>
            {
                public void CustomizeFor(ICustomizationBuilder<ICellGetter> builder)
                {
                    builder.EmbedRecordsInSameFile(x => x.Temporary)
                        .EmbedRecordsInSameFile(x => x.Persistent)
                        .EmbedRecordsInSameFile(x => x.NavigationMeshes)
                        .EmbedRecordsInSameFile(x => x.Landscape);
                }
            }
            
            public class WorldspaceCustomization : ICustomize<IWorldspace>
            {
                public void CustomizeFor(ICustomizationBuilder<IWorldspace> builder)
                {
                    builder.EmbedRecordsInSameFile(x => x.TopCell);
                }
            }
            """;
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task StarfieldModFilePerRecordGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Starfield;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class SerializationTests
{
    public void EmptyStarfieldMod()
    { 
        var mod = new StarfieldMod(Constants.Starfield, StarfieldRelease.Starfield);
        var stream = new MemoryStream();
        var workEngine = new InlineWorkDropoff();

        MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine);
    }
}

public class Customization : ICustomize
{
    public void Customize(ICustomizationBuilder builder)
    {
        builder.FilePerRecord();
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task SkyrimModWithMetaFileGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests;

public class MyMeta
{
    public int MyInt { get; set; }
    public bool MyBool { get; set; }
}

public class SerializationTests
{
    public void EmptySkyrimMod()
    { 
        var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        var workEngine = new InlineWorkDropoff();

        MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine, extraMeta: new MyMeta(true, 23));
    }
}";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task MetaFileInOtherNamespaceGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Skyrim;
using Noggog.WorkEngine;
using TestNamespace;

namespace TestNamespace
{
public class MyMeta
{
    public int MyInt { get; set; }
    public bool MyBool { get; set; }
}
}

namespace Mutagen.Bethesda.Serialization.Tests.SerializationTests
{
public class SerializationTests
{
    public void EmptySkyrimMod()
    { 
        var mod = new SkyrimMod(Constants.Skyrim, SkyrimRelease.SkyrimSE);
        var stream = new MemoryStream();
        var workEngine = new InlineWorkDropoff();

        MutagenTestConverter.Instance.Serialize(mod, stream, workEngine: workEngine, extraMeta: new MyMeta(true, 23));
    }
}
}

";
        var result = TestHelper.RunSourceGenerator(source);
        result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }
    
    [Fact]
    public async Task SkyrimModInterfaceGenerationBootstrapper()
    {
        var source = @"
using Mutagen.Bethesda.Serialization.Tests;
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
            .ShouldBeEmpty();
        result.Diagnostics
            .Where(
                d => d.Severity == DiagnosticSeverity.Warning && 
                     d.Id == "CS8785")
            .ShouldBeEmpty();
    }

    [Fact]
    public async Task TestMod()
    {
        await TestHelper.VerifySerialization(GetModWith((sb) =>
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
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Tests;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.Tests");
        
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

        await TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public async Task DoubleCall()
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Tests;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.Tests");
        
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

        await TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public async Task TwoDifferentCalls()
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Tests;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.Tests");
        
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

        await TestHelper.VerifySerialization(sb.ToString());
    }
    
    private string GetModWith(Action<StructuredStringBuilder> memberBuilder)
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Tests;");
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.Tests");
        
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