using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

[UsesVerify]
public class MemberTests : ATestsBase
{
    private string GetPrimitiveTest(params string[] nicknames)
    {
        return GetObjWithMember(sb =>
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
            foreach (var nickname in nicknames)
            {
                sb.AppendLine($"public {nickname} SomeMember{i} {{ get; set; }}");
                sb.AppendLine($"public static readonly {nickname} SomeMember{i++}Default = default;");
            }
            foreach (var nickname in nicknames)
            {
                sb.AppendLine($"public {nickname}? SomeMember{i} {{ get; set; }}");
                sb.AppendLine($"public static readonly {nickname}? SomeMember{i++}Default = default;");
            }
            foreach (var nickname in nicknames)
            {
                sb.AppendLine($"public Nullable<{nickname}> SomeMember{i} {{ get; set; }}");
                sb.AppendLine($"public static readonly Nullable<{nickname}> SomeMember{i++}Default = default;");
            }
        });
    }
    
    [Fact]
    public Task ModNoGeneration()
    {
        var source = GetModWithMember(sb => { });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task ObjNoGeneration()
    {
        var source = GetObjWithMember(sb => { });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task UnknownType()
    {
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
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
    public Task FormVersion()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public ushort FormVersion { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Loqui()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public SomeLoqui MyLoqui { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task LoquiWithBaseClass()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public SomeLoquiWithBase MyLoqui { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task LoquiWithMultipleBaseClasses()
    {
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public List<string> SomeList { get; set; }");
            sb.AppendLine("public IReadOnlyList<string> SomeList2 { get; set; }");
            sb.AppendLine("public ExtendedList<string> SomeList3 { get; set; }");
            sb.AppendLine("public List<string>? SomeList4 { get; set; }");
            sb.AppendLine("public IReadOnlyList<string>? SomeList5 { get; set; }");
            sb.AppendLine("public ExtendedList<string>? SomeList6 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task LoquiList()
    {
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public AssetLink<INpcGetter> SomeAssetLink { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter> SomeAssetLink2 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter> SomeAssetLink3 { get; set; }");
            sb.AppendLine("public AssetLinkGetter<INpcGetter> SomeAssetLink4 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<INpcGetter> SomeAssetLink5 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<Mutagen.Bethesda.Skyrim.INpcGetter> SomeAssetLink6 { get; set; }");
            sb.AppendLine("public IAssetLink<INpcGetter> SomeAssetLink7 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter> SomeAssetLink8 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter> SomeAssetLink9 { get; set; }");
            sb.AppendLine("public IAssetLinkGetter<INpcGetter> SomeAssetLink10 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter> SomeAssetLink11 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter> SomeAssetLink12 { get; set; }"); 
            sb.AppendLine("public AssetLink<INpcGetter>? SomeAssetLink13 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter>? SomeAssetLink14 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>? SomeAssetLink15 { get; set; }");
            sb.AppendLine("public IAssetLink<INpcGetter>? SomeAssetLink16 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>? SomeAssetLink17 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>? SomeAssetLink18 { get; set; }");
            sb.AppendLine("public IAssetLinkGetter<INpcGetter>? SomeAssetLink19 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>? SomeAssetLink20 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>? SomeAssetLink21 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Array()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public string[] SomeArray { get; set; }");
            sb.AppendLine("public string[]? SomeArray2 { get; set; }");
        });
       
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Array2d()
    {
        var source = GetObjWithMember(sb =>
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
        var source = GetObjWithMember(sb =>
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
        var source = GetModWithMember(
            sb =>
            {
                sb.AppendLine("public Group<TestMajorRecord> SomeGroup { get; set; }");
                sb.AppendLine("public IGroup<TestMajorRecord> SomeGroup2 { get; set; }");
                sb.AppendLine("public IGroupGetter<TestMajorRecord> SomeGroup3 { get; set; }");
            },
            sb =>
            {
                sb.AppendLine("public interface IGroupGetter<T> : ILoquiObjectGetter, IGroupGetter");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine("where T : class, IMajorRecordInternal");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("public int SomeInt { get; set; }");
                    sb.AppendLine("public IReadOnlyCache<T, int> Items { get; set; }");
                    sb.AppendLine("public int SomeInt2 { get; set; }");
                }
                
                sb.AppendLine("public interface IGroup<T> : ILoquiObject, IGroupGetter<T>");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine("where T : class, IMajorRecordInternal");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("public int SomeInt { get; set; }");
                    sb.AppendLine("public IReadOnlyCache<T, int> Items { get; set; }");
                    sb.AppendLine("public int SomeInt2 { get; set; }");
                }
                
                sb.AppendLine("public class Group<T> : SomeBaseClass, IGroup<T>");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine("where T : class, IMajorRecordInternal");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("public int SomeInt { get; set; }");
                    sb.AppendLine("public IReadOnlyCache<T, int> Items { get; set; }");
                    sb.AppendLine("public int SomeInt2 { get; set; }");
                }
            });
       
        return TestHelper.Verify(source);
    }
}