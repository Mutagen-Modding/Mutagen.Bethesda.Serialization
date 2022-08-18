using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

[UsesVerify]
public class MemberTests
{
    private string GetModWithMember(Action<StructuredStringBuilder> memberBuilder)
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Newtonsoft;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Records;");
        sb.AppendLine();
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        sb.AppendLine("public interface ITestModGetter : IModGetter");
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
        }
        
        using (var c = sb.Class("TestMod"))
        {
            c.Partial = true;
            c.BaseClass = "AMod";
            c.Interfaces.Add("ITestModGetter");
        }

        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
            sb.AppendLine();
            using (var f = sb.Function("public void SomeFunction"))
            {
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine($"var theMod = new TestMod();");
                sb.AppendLine("MutagenJsonConverter.Instance.Convert(theMod);");
            }
        }

        return sb.ToString();
    }
    
    [Fact]
    public Task NoGeneration()
    {
        var source = GetModWithMember(sb => { });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task UnknownType()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public Unknown UnknownThing { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task String()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public string SomeString { get; set; }");
            sb.AppendLine("public String SomeString2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task UInt8()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public byte SomeInt8 { get; set; }");
            sb.AppendLine("public Byte SomeInt82 { get; set; }");
            sb.AppendLine("public UInt8 SomeInt83 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Int8()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public sbyte SomeUInt8 { get; set; }");
            sb.AppendLine("public SByte SomeUInt82 { get; set; }");
            sb.AppendLine("public Int8 SomeUInt83 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task UInt16()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public ushort SomeUShort { get; set; }");
            sb.AppendLine("public UInt16 SomeUShort2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Int16()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public short SomeShort { get; set; }");
            sb.AppendLine("public Int16 SomeShort2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task UInt32()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public uint SomeUInt { get; set; }");
            sb.AppendLine("public UInt32 SomeUInt2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Int32()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public int SomeInt { get; set; }");
            sb.AppendLine("public Int32 SomeInt2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task UInt64()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public ulong SomeULong { get; set; }");
            sb.AppendLine("public UInt64 SomeULong2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Int64()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public long SomeLong { get; set; }");
            sb.AppendLine("public Int64 SomeLong2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Enum()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public enum MyEnum { }");
            sb.AppendLine("public MyEnum SomeEnum { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Loqui()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public class MyLoqui : SomeBaseClass, ILoquiObject { }");
            sb.AppendLine("public MyLoqui SomeLoqui { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Bool()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public bool SomeBool { get; set; }");
            sb.AppendLine("public Boolean SomeBool2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Float()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public float SomeFloat { get; set; }");
            sb.AppendLine("public Single SomeFloat2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
}