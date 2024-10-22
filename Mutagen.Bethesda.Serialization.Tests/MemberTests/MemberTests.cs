using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.MemberTests;

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
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task ObjNoGeneration()
    {
        var source = GetObjWithMember(sb => { });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task UnknownType()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public Unknown UnknownThing { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task String()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("string", "String"));
    }
    
    [Fact]
    public Task UInt8()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("byte", "Byte", "UInt8"));
    }
    
    [Fact]
    public Task Int8()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("sbyte", "SByte", "Int8"));
    }
    
    [Fact]
    public Task UInt16()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("ushort", "UInt16"));
    }
    
    [Fact]
    public Task Int16()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("short", "Int16"));
    }
    
    [Fact]
    public Task UInt32()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("uint", "UInt32"));
    }
    
    [Fact]
    public Task Int32()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("int", "Int32"));
    }
    
    [Fact]
    public Task UInt64()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("ulong", "UInt64"));
    }
    
    [Fact]
    public Task Int64()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("long", "Int64"));
    }
    
    [Fact]
    public Task P3Float()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("P3Float"));
    }
    
    [Fact]
    public Task P2Float()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("P2Float"));
    }
    
    [Fact]
    public Task P3UInt8()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("P3UInt8"));
    }
    
    [Fact]
    public Task P3Int16()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("P3Int16"));
    }
    
    [Fact]
    public Task P3UInt16()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("P3UInt16"));
    }
    
    [Fact]
    public Task P2Int()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("P2Int"));
    }
    
    [Fact]
    public Task P2Int16()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("P2Int16"));
    }
    
    [Fact]
    public Task Percent()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("Percent"));
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
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task FormVersion()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public ushort FormVersion { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task Loqui()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public SomeLoqui MyLoqui { get; set; }");
            sb.AppendLine("public SomeLoqui? MyLoqui2 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task LoquiWithBaseClass()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public SomeLoquiWithBase MyLoqui { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task LoquiWithMultipleBaseClasses()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public AbstractBaseLoqui MyLoqui { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task Bool()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("bool", "Boolean"));
    }
    
    [Fact]
    public Task Bytes()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public byte[] SomeBytes { get; set; }");
            sb.AppendLine("public ReadOnlyMemorySlice<byte> SomeBytes2 { get; set; }");
            sb.AppendLine("public MemorySlice<byte> SomeBytes3 { get; set; } = new byte[5];");
            sb.AppendLine("public byte[]? SomeBytes4 { get; set; }");
            sb.AppendLine("public ReadOnlyMemorySlice<byte>? SomeBytes5 { get; set; }");
            sb.AppendLine("public Nullable<ReadOnlyMemorySlice<byte>> SomeBytes6 { get; set; }");
            sb.AppendLine("public Nullable<MemorySlice<byte>> SomeBytes7 { get; set; }");
            sb.AppendLine("public byte[] SomeBytes8 { get; } = new byte[5];");
            sb.AppendLine("public ReadOnlyMemorySlice<byte> SomeBytes9 { get; } = new byte[5];");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task SliceList()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public IReadOnlyList<ReadOnlyMemorySlice<Byte>> SomeBytes { get; set; }");
            sb.AppendLine("public SliceList<byte> SomeBytes2 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task Char()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("char", "Char"));
    }
    
    [Fact]
    public Task Float()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("float", "Single"));
    }
    
    [Fact]
    public Task Color()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("System.Drawing.Color", "Color"));
    }
    
    [Fact]
    public Task RecordType()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("RecordType", "Mutagen.Bethesda.Plugins.RecordType"));
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
       
        return TestHelper.VerifySerialization(source);
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
       
        return TestHelper.VerifySerialization(source);
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
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task MajorRecordList()
    {
        var source = GetObjWithMember(sb =>
            {
                sb.AppendLine("public List<TestMajorRecord> SomeList { get; set; }");
                sb.AppendLine("public IReadOnlyList<TestMajorRecord> SomeList2 { get; set; }");
                sb.AppendLine("public ExtendedList<TestMajorRecord> SomeList3 { get; set; }");
                sb.AppendLine("public TestMajorRecord[] SomeList4 { get; set; }");
            },
            outsideBuilder: sb =>
            {
                CustomizeForFolderRecord(sb);
            });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task MajorRecordWithNestedMajorRecordsList()
    {
        var source = GetObjWithMember(sb =>
            {
                sb.AppendLine("public List<TestMajorRecordWithNested> SomeList { get; set; }");
                sb.AppendLine("public IReadOnlyList<TestMajorRecordWithNested> SomeList2 { get; set; }");
                sb.AppendLine("public ExtendedList<TestMajorRecordWithNested> SomeList3 { get; set; }");
                sb.AppendLine("public TestMajorRecordWithNested[] SomeList4 { get; set; }");
            },
            outsideBuilder: sb =>
            {
                CustomizeForFolderRecord(sb);
            });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task FormLinkList()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public ExtendedList<IFormLinkGetter<ITestMajorRecordGetter>> SomeFormKeys { get; set; }");
            sb.AppendLine("public IReadOnlyList<IFormLinkGetter<ITestMajorRecordGetter>> SomeFormKeys2 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
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
       
        return TestHelper.VerifySerialization(source);
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
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task GenderedItemNullable()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public GenderedItem<ExtendedList<int>?> SomeGendered { get; set; }");
            sb.AppendLine("public IGenderedItem<ExtendedList<int>?> SomeGendered2 { get; set; }");
            sb.AppendLine("public IGenderedItemGetter<ExtendedList<int>?> SomeGendered3 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task ModKey()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("ModKey", "Mutagen.Bethesda.Plugins.ModKey"));
    }
    
    [Fact]
    public Task FormKey()
    {
        return TestHelper.VerifySerialization(GetPrimitiveTest("FormKey", "Mutagen.Bethesda.Plugins.FormKey"));
    }
    
    [Fact]
    public Task FormLink()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public FormLink<ITestMajorRecordGetter> SomeFormKey { get; set; }");
            sb.AppendLine("public FormLinkNullable<ITestMajorRecordGetter> SomeFormKey2 { get; set; }");
            sb.AppendLine("public IFormLink<ITestMajorRecordGetter> SomeFormKey3 { get; set; }");
            sb.AppendLine("public IFormLinkNullable<ITestMajorRecordGetter> SomeFormKey4 { get; set; }");
            sb.AppendLine("public IFormLinkGetter<ITestMajorRecordGetter> SomeFormKey5 { get; set; }");
            sb.AppendLine("public IFormLinkNullableGetter<ITestMajorRecordGetter> SomeFormKey6 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task FormLinkOrIndex()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public FormLinkOrIndex<ITestMajorRecordGetter> SomeFormKey { get; set; }");
            sb.AppendLine("public IFormLinkOrIndex<ITestMajorRecordGetter> SomeFormKey3 { get; set; }");
            sb.AppendLine("public IFormLinkOrIndexGetter<ITestMajorRecordGetter> SomeFormKey5 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task AssetLink()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public AssetLink<ITestMajorRecordGetter> SomeAssetLink { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<ITestMajorRecordGetter> SomeAssetLink2 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter> SomeAssetLink3 { get; set; }");
            sb.AppendLine("public AssetLinkGetter<ITestMajorRecordGetter> SomeAssetLink4 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<ITestMajorRecordGetter> SomeAssetLink5 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter> SomeAssetLink6 { get; set; }");
            sb.AppendLine("public IAssetLink<ITestMajorRecordGetter> SomeAssetLink7 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<ITestMajorRecordGetter> SomeAssetLink8 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter> SomeAssetLink9 { get; set; }");
            sb.AppendLine("public IAssetLinkGetter<ITestMajorRecordGetter> SomeAssetLink10 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<ITestMajorRecordGetter> SomeAssetLink11 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter> SomeAssetLink12 { get; set; }"); 
            sb.AppendLine("public AssetLink<ITestMajorRecordGetter>? SomeAssetLink13 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<ITestMajorRecordGetter>? SomeAssetLink14 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>? SomeAssetLink15 { get; set; }");
            sb.AppendLine("public IAssetLink<ITestMajorRecordGetter>? SomeAssetLink16 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<ITestMajorRecordGetter>? SomeAssetLink17 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>? SomeAssetLink18 { get; set; }");
            sb.AppendLine("public IAssetLinkGetter<ITestMajorRecordGetter>? SomeAssetLink19 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<ITestMajorRecordGetter>? SomeAssetLink20 { get; set; }");
            sb.AppendLine("public Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>? SomeAssetLink21 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task Array()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public string[] SomeArray { get; set; }");
            sb.AppendLine("public string[]? SomeArray2 { get; set; }");
            sb.AppendLine("public string[] SomeArray3 { get; } = new string[4];");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task ArrayWithListGetters()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public CloudLayer[] SomeArray { get; set; }");
            sb.AppendLine("public CloudLayer[]? SomeArray2 { get; set; }");
            sb.AppendLine("public CloudLayer[] SomeArray3 { get; } = new string[4];");
        },
        getterMemberBuilder: sb =>
        {
            sb.AppendLine("public IReadOnlyList<CloudLayer> SomeArray { get; set; }");
            sb.AppendLine("public IReadOnlyList<CloudLayer>? SomeArray2 { get; set; }");
            sb.AppendLine("public IReadOnlyList<CloudLayer> SomeArray3 { get; } = new string[4];");
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task ArraySliceGetter()
    {
        var source = GetObjWithMember(sb =>
            {
                sb.AppendLine("public float[] SomeArray { get; set; }");
                sb.AppendLine("public float[]? SomeArray2 { get; set; }");
                sb.AppendLine("public float[] SomeArray3 { get; } = new string[4];");
            },
            getterMemberBuilder: sb =>
            {
                sb.AppendLine("public ReadOnlyMemorySlice<float> SomeArray { get; set; }");
                sb.AppendLine("public ReadOnlyMemorySlice<float>? SomeArray2 { get; set; }");
                sb.AppendLine("public ReadOnlyMemorySlice<float> SomeArray3 { get; } = new string[4];");
            });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task Array2d()
    {
        var source = GetObjWithMember(sb =>
        {
            sb.AppendLine("public Array2d<string> SomeArray { get; set; }");
            sb.AppendLine("public IArray2d<string> SomeArray2 { get; set; }");
            sb.AppendLine("public IReadOnlyArray2d<string> SomeArray3 { get; set; }");
            sb.AppendLine("public Array2d<string>? SomeArray4 { get; set; }");
            sb.AppendLine("public IArray2d<string>? SomeArray5 { get; set; }");
            sb.AppendLine("public IReadOnlyArray2d<string>? SomeArray6 { get; set; }");
        });
       
        return TestHelper.VerifySerialization(source);
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
       
        return TestHelper.VerifySerialization(source);
    }

    private void CustomizeForFolderRecord(StructuredStringBuilder sb)
    {
        sb.AppendLine("public class Customization : ICustomize");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public void Customize(ICustomizationBuilder builder)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder.FilePerRecord();");
            }
        }
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
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task ListBlockGroup()
    {
        var source = GetModWithMember(
            sb =>
            {
                sb.AppendLine("public ListGroup<CellBlock> SomeGroup { get; set; }");
                sb.AppendLine("public IListGroup<CellBlock> SomeGroup2 { get; set; }");
                sb.AppendLine("public IListGroupGetter<CellBlock> SomeGroup3 { get; set; }");
            },
            sb =>
            {
                sb.AppendLine("public interface IListGroupGetter<T> : ILoquiObjectGetter, IListGroupGetter");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine("where T : class, IMajorRecordInternal");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("public int SomeInt { get; set; }");
                    sb.AppendLine("public IReadOnlyList<T> Items { get; set; }");
                    sb.AppendLine("public int SomeInt2 { get; set; }");
                }
                
                sb.AppendLine("public interface IListGroup<T> : ILoquiObject, IListGroupGetter<T>");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine("where T : class, IMajorRecordInternal");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("public int SomeInt { get; set; }");
                    sb.AppendLine("public IReadOnlyList<T> Items { get; set; }");
                    sb.AppendLine("public int SomeInt2 { get; set; }");
                }
                
                sb.AppendLine("public class ListGroup<T> : SomeBaseClass, IListGroup<T>");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine("where T : class, IMajorRecordInternal");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("public int SomeInt { get; set; }");
                    sb.AppendLine("public IExtendedList<T> Items { get; set; }");
                    sb.AppendLine("public int SomeInt2 { get; set; }");
                }

                sb.AppendLine("public interface ICellBlockGetter : ILoquiObjectGetter");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("int BlockNumber { get; }");
                    sb.AppendLine("IReadOnlyList<CellSubBlock> SubBlocks { get; }");
                }
                
                sb.AppendLine("public interface ICellBlock : ILoquiObject, ICellBlockGetter");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("int BlockNumber { get; set; }");
                    sb.AppendLine("IExtendedList<CellSubBlock> SubBlocks { get; }");
                }
                
                sb.AppendLine("public class CellBlock : ICellBlock");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("int BlockNumber { get; set; }");
                    sb.AppendLine("public IExtendedList<CellSubBlock> SubBlocks { get; }");
                }
                
                sb.AppendLine("public interface ICellSubBlockGetter : ILoquiObjectGetter");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("int BlockNumber { get; }");
                    sb.AppendLine("IReadOnlyList<TestMajorRecord> Records { get; }");
                }
                
                sb.AppendLine("public interface ICellSubBlock : ILoquiObject, ICellSubBlockGetter");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("int BlockNumber { get; set; }");
                    sb.AppendLine("IExtendedList<TestMajorRecord> Records { get; }");
                }
                
                sb.AppendLine("public class CellSubBlock : ICellSubBlock");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("int BlockNumber { get; set; }");
                    sb.AppendLine("public IExtendedList<TestMajorRecord> Records { get; }");
                }

                CustomizeForFolderRecord(sb);
            });
       
        return TestHelper.VerifySerialization(source);
    }
}