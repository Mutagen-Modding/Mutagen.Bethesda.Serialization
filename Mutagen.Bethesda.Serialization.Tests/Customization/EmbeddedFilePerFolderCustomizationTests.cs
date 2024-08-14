using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Customization;

public class EmbeddedFilePerFolderCustomizationTests : ATestsBase
{
    [Fact]
    public Task ListEmbedded()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public ExtendedList<TestMajorRecord> SomeList { get; set; } = null!;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeRecord");
        
        GenerateGroup(sb);

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void Customize"))
            {
                f.Add("ICustomizationBuilder builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".FilePerRecord();");
                }
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("EmbedCustomization"))
        {
            c.Interfaces.Add("ICustomize<SomeRecord>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<ISomeRecordGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".EmbedRecordsInSameFile(x => x.SomeList);");
                }
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task MajorRecordEmbedded()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public TestMajorRecord MajorRecord { get; set; } = null!;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeRecord");
        
        GenerateGroup(sb);

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void Customize"))
            {
                f.Add("ICustomizationBuilder builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".FilePerRecord();");
                }
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("EmbedCustomization"))
        {
            c.Interfaces.Add("ICustomize<SomeRecord>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<ISomeRecordGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".EmbedRecordsInSameFile(x => x.MajorRecord);");
                }
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task SubobjectHasEmbedded()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public TestMajorRecord MajorRecord { get; set; } = null!;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeRecord");
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public SomeRecord SomeRecord { get; set; } = null!;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeBaseRecord");
        
        GenerateGroup(sb);

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void Customize"))
            {
                f.Add("ICustomizationBuilder builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".FilePerRecord();");
                }
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("EmbedCustomization"))
        {
            c.Interfaces.Add("ICustomize<SomeRecord>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<ISomeRecordGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".EmbedRecordsInSameFile(x => x.MajorRecord);");
                }
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task CellSubBlock()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public List<TestMajorRecord> MajorRecord { get; set; } = null!;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "CellSubBlock");
        
        GenerateGroup(sb);

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void Customize"))
            {
                f.Add("ICustomizationBuilder builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".FilePerRecord();");
                }
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("EmbedCustomization"))
        {
            c.Interfaces.Add("ICustomize<SomeRecord>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<ISomeRecordGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".EmbedRecordsInSameFile(x => x.MajorRecord);");
                }
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task WorldspaceSubBlock()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public List<TestMajorRecord> MajorRecord { get; set; } = null!;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "WorldspaceSubBlock");
        
        GenerateGroup(sb);

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void Customize"))
            {
                f.Add("ICustomizationBuilder builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".FilePerRecord();");
                }
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("EmbedCustomization"))
        {
            c.Interfaces.Add("ICustomize<SomeRecord>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<ISomeRecordGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".EmbedRecordsInSameFile(x => x.MajorRecord);");
                }
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }}