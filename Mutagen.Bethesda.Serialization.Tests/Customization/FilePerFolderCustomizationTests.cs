using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Customization;

public class FilePerFolderCustomizationTests : ATestsBase
{
    [Fact]
    public Task GroupWithoutFilePerRecord()
    {
        var sb = new StructuredStringBuilder();
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
                sb.AppendLine("public int SomeMember3 { get; set; } = 3;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeMajorRecord");
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public Group<SomeMajorRecord> SomeGroup1 { get; set; } = null!;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeMod");
        
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
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task NothingApplicable()
    {
        var sb = new StructuredStringBuilder();
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
                sb.AppendLine("public int SomeMember3 { get; set; } = 3;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeMajorRecord");
        
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

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task Group()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public Group<SomeMajorRecord> SomeGroup1 { get; set; } = null!;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeMajorRecord");
        
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

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task List()
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

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task MajorRecord()
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

        return TestHelper.VerifySerialization(sb.ToString());
    }
}