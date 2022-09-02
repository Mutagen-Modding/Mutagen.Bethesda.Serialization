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
        sb.AppendLine("using Loqui;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Newtonsoft;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Records;");
        sb.AppendLine();
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        sb.AppendLine("public partial interface ITestModGetter : IModGetter, ILoquiObject");
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
    public Task P3Float()
    {
        return TestHelper.Verify(GetPrimitiveTest("P3Float"));
    }
    
    [Fact]
    public Task P2Float()
    {
        return TestHelper.Verify(GetPrimitiveTest("P2Float"));
    }
    
    [Fact]
    public Task P3UInt8()
    {
        return TestHelper.Verify(GetPrimitiveTest("P3UInt8"));
    }
    
    [Fact]
    public Task P3Int16()
    {
        return TestHelper.Verify(GetPrimitiveTest("P3Int16"));
    }
    
    [Fact]
    public Task P3UInt16()
    {
        return TestHelper.Verify(GetPrimitiveTest("P3UInt16"));
    }
    
    [Fact]
    public Task P2Int()
    {
        return TestHelper.Verify(GetPrimitiveTest("P2Int"));
    }
    
    [Fact]
    public Task P2Int16()
    {
        return TestHelper.Verify(GetPrimitiveTest("P2Int16"));
    }
    
    [Fact]
    public Task Percent()
    {
        return TestHelper.Verify(GetPrimitiveTest("Percent"));
    }
    
    [Fact]
    public Task Enum()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public enum MyEnum { }");
            sb.AppendLine("public MyEnum SomeEnum { get; set; }");
            sb.AppendLine("public System.Nullable<MyEnum> SomeEnum2 { get; set; }");
            
            sb.AppendLine("public enum MyEnum2 : uint { }");
            sb.AppendLine("public MyEnum2 SomeEnum3 { get; set; }");
            sb.AppendLine("public System.Nullable<MyEnum2> SomeEnum4 { get; set; }");
            
            sb.AppendLine("[System.Flags]");
            sb.AppendLine("public enum MyEnum3 { }");
            sb.AppendLine("public MyEnum3 SomeEnum5 { get; set; }");
            sb.AppendLine("public System.Nullable<MyEnum3> SomeEnum6 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Loqui()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public SomeLoqui MyLoqui { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task LoquiWithBaseClass()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public SomeLoquiWithBase MyLoqui { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task LoquiWithMultipleBaseClasses()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public AbstractBaseLoqui MyLoqui { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Bool()
    {
        return TestHelper.Verify(GetPrimitiveTest("bool", "Boolean"));
    }
    
    [Fact]
    public Task Bytes()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public byte[] SomeBytes { get; set; }");
            sb.AppendLine("public ReadOnlyMemorySlice<byte> SomeBytes2 { get; set; }");
            sb.AppendLine("public byte[]? SomeBytes3 { get; set; }");
            sb.AppendLine("public ReadOnlyMemorySlice<byte>? SomeBytes4 { get; set; }");
            sb.AppendLine("public Nullable<ReadOnlyMemorySlice<byte>> SomeBytes5 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Char()
    {
        return TestHelper.Verify(GetPrimitiveTest("char", "Char"));
    }
    
    [Fact]
    public Task Float()
    {
        return TestHelper.Verify(GetPrimitiveTest("float", "Single"));
    }
    
    [Fact]
    public Task Color()
    {
        return TestHelper.Verify(GetPrimitiveTest("System.Drawing.Color", "Color"));
    }
    
    [Fact]
    public Task RecordType()
    {
        return TestHelper.Verify(GetPrimitiveTest("RecordType", "Mutagen.Bethesda.Plugins.RecordType"));
    }
    
    [Fact]
    public Task TranslatedString()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public TranslatedString TranslatedString { get; set; }");
            sb.AppendLine("public ITranslatedString TranslatedString2 { get; set; }");
            sb.AppendLine("public ITranslatedStringGetter TranslatedString3 { get; set; }");
            sb.AppendLine("public TranslatedString? TranslatedString4 { get; set; }");
            sb.AppendLine("public ITranslatedString? TranslatedString5 { get; set; }");
            sb.AppendLine("public ITranslatedStringGetter? TranslatedString6 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Strings.TranslatedString TranslatedString7 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Strings.ITranslatedString TranslatedString8 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Strings.ITranslatedStringGetter TranslatedString9 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Strings.TranslatedString? TranslatedString10 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Strings.ITranslatedString? TranslatedString11 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Strings.ITranslatedStringGetter? TranslatedString12 { get; set; }");
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
    public Task LoquiList()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public List<SomeLoqui> SomeList { get; set; }");
            sb.AppendLine("public IReadOnlyList<SomeLoqui> SomeList2 { get; set; }");
            sb.AppendLine("public ExtendedList<SomeLoqui> SomeList3 { get; set; }");
            sb.AppendLine("public SomeLoqui[] SomeList4 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Dictionary()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public Dictionary<int, string> SomeDict { get; set; }");
            sb.AppendLine("public IReadOnlyDictionary<int, string> SomeDict1 { get; set; }");
            sb.AppendLine("public IDictionary<int, string> SomeDict2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GenderedItem()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public GenderedItem<string> SomeGenderedInt { get; set; }");
            sb.AppendLine("public IGenderedItem<string> SomeGenderedInt2 { get; set; }");
            sb.AppendLine("public IGenderedItemGetter<string> SomeGenderedInt3 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task ModKey()
    {
        return TestHelper.Verify(GetPrimitiveTest("ModKey", "Mutagen.Bethesda.Plugins.ModKey"));
    }
    
    [Fact]
    public Task FormKey()
    {
        return TestHelper.Verify(GetPrimitiveTest("FormKey", "Mutagen.Bethesda.Plugins.FormKey"));
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
    public Task AssetLink()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public AssetLink<INpcGetter> SomeFormKey { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter> SomeFormKey2 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter> SomeFormKey3 { get; set; }");
            sb.AppendLine("public AssetLinkGetter<INpcGetter> SomeFormKey4 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<INpcGetter> SomeFormKey5 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<Mutagen.Bethesda.Skyrim.INpcGetter> SomeFormKey6 { get; set; }");
            sb.AppendLine("public IAssetLink<INpcGetter> SomeFormKey7 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter> SomeFormKey8 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter> SomeFormKey9 { get; set; }");
            sb.AppendLine("public IAssetLinkGetter<INpcGetter> SomeFormKey10 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter> SomeFormKey11 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter> SomeFormKey12 { get; set; }");
            sb.AppendLine("public AssetLink<INpcGetter>? SomeFormKey13 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter>? SomeFormKey14 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>? SomeFormKey15 { get; set; }");
            sb.AppendLine("public IAssetLink<INpcGetter>? SomeFormKey16 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>? SomeFormKey17 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>? SomeFormKey18 { get; set; }");
            sb.AppendLine("public IAssetLinkGetter<INpcGetter>? SomeFormKey19 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>? SomeFormKey20 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>? SomeFormKey21 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Array()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public string[] SomeArray { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Array2d()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public Array2d<string> SomeArray { get; set; }");
            sb.AppendLine("public IArray2d<string> SomeArray2 { get; set; }");
            sb.AppendLine("public IReadOnlyArray2d<string> SomeArray3 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task SkippedProperty()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public ILoquiRegistration StaticRegistration { get; set; }");
            sb.AppendLine("public ILoquiRegistration Registration { get; set; }");
            sb.AppendLine("public string this[string str] => throw new NotImplementedException();");
            sb.AppendLine("public int SomeInt { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task TypicalGroup()
    {
        var source = GetModWithMember(sb =>
        {
            sb.AppendLine("public class Group<T> : SomeBaseClass, ILoquiObject, IGroupGetter");
            using (sb.IncreaseDepth())
            {
                sb.AppendLine("where T : class, IMajorRecordInternal");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("public int SomeInt { get; set; }");
                sb.AppendLine("public List<T> Items { get; set; }");
                sb.AppendLine("public int SomeInt2 { get; set; }");
            }
            
            sb.AppendLine("public class MajorRecord : SomeBaseClass, ILoquiObject");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("public int Item { get; set; }");
            }
            
            sb.AppendLine("public Group<MajorRecord> SomeGroup { get; set; }");
            sb.AppendLine("public IGroup<MajorRecord> SomeGroup2 { get; set; }");
            sb.AppendLine("public IGroupGetter<MajorRecord> SomeGroup3 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
}