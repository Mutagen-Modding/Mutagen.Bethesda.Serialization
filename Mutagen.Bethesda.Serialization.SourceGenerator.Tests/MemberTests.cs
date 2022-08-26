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
        
        sb.AppendLine("using Noggog;");
        sb.AppendLine("using System.Collections.Generic;");
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

    private string GetPrimitiveTest(params string[] nicknames)
    {
        return GetModWithMember(sb =>
        {
            int i = 0;
            foreach (var nickname in nicknames)
            {
                sb.AppendLine($"public {nickname} SomeMember{i++} {{ get; set; }}");
            }
            foreach (var nickname in nicknames)
            {
                sb.AppendLine($"public {nickname}? SomeMember{i++} {{ get; set; }}");
            }
            foreach (var nickname in nicknames)
            {
                sb.AppendLine($"public Nullable<{nickname}> SomeMember{i++} {{ get; set; }}");
            }
        });
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
        return TestHelper.Verify(GetPrimitiveTest("string", "String"));
    }
    
    [Fact]
    public Task UInt8()
    {
        return TestHelper.Verify(GetPrimitiveTest("byte", "Byte", "UInt8"));
    }
    
    [Fact]
    public Task Int8()
    {
        return TestHelper.Verify(GetPrimitiveTest("sbyte", "SByte", "Int8"));
    }
    
    [Fact]
    public Task UInt16()
    {
        return TestHelper.Verify(GetPrimitiveTest("ushort", "UInt16"));
    }
    
    [Fact]
    public Task Int16()
    {
        return TestHelper.Verify(GetPrimitiveTest("short", "Int16"));
    }
    
    [Fact]
    public Task UInt32()
    {
        return TestHelper.Verify(GetPrimitiveTest("uint", "UInt32"));
    }
    
    [Fact]
    public Task Int32()
    {
        return TestHelper.Verify(GetPrimitiveTest("int", "Int32"));
    }
    
    [Fact]
    public Task UInt64()
    {
        return TestHelper.Verify(GetPrimitiveTest("ulong", "UInt64"));
    }
    
    [Fact]
    public Task Int64()
    {
        return TestHelper.Verify(GetPrimitiveTest("long", "Int64"));
    }
    
    [Fact]
    public Task Enum()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public enum MyEnum { }");
            sb.AppendLine("public MyEnum SomeEnum { get; set; }");
            
            sb.AppendLine("public enum MyEnum2 : uint { }");
            sb.AppendLine("public MyEnum2 SomeEnum2 { get; set; }");
            
            sb.AppendLine("[System.Flags]");
            sb.AppendLine("public enum MyEnum3 { }");
            sb.AppendLine("public MyEnum3 SomeEnum3 { get; set; }");
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
        return TestHelper.Verify(GetPrimitiveTest("bool", "Boolean"));
    }
    
    [Fact]
    public Task Float()
    {
        return TestHelper.Verify(GetPrimitiveTest("float", "Single"));
    }
    
    [Fact]
    public Task TranslatedString()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public TranslatedString TranslatedString { get; set; }");
            sb.AppendLine("public ITranslatedString TranslatedString2 { get; set; }");
            sb.AppendLine("public ITranslatedStringGetter TranslatedString3 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task List()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public List<string> SomeList { get; set; }");
            sb.AppendLine("public IReadOnlyList<string> SomeList2 { get; set; }");
            sb.AppendLine("public ExtendedList<string> SomeList3 { get; set; }");
            sb.AppendLine("public string[] SomeList4 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task FormLink()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public FormLink<INpcGetter> SomeFormKey { get; set; }");
            sb.AppendLine("public FormLinkNullable<INpcGetter> SomeFormKey2 { get; set; }");
            sb.AppendLine("public IFormLink<INpcGetter> SomeFormKey3 { get; set; }");
            sb.AppendLine("public IFormLinkNullable<INpcGetter> SomeFormKey4 { get; set; }");
            sb.AppendLine("public IFormLinkGetter<INpcGetter> SomeFormKey5 { get; set; }");
            sb.AppendLine("public IFormLinkNullableGetter<INpcGetter> SomeFormKey6 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task SkippedProperty()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public int StaticRegistration { get; set; }");
            sb.AppendLine("public int SomeInt { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
}