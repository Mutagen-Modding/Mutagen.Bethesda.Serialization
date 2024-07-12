using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class CustomizationTests : ATestsBase
{
    private string GetBasicTest(
        Action<StructuredStringBuilder> customizationBuilder)
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
            });
        
        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<ISomeObjectGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<ISomeObjectGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                customizationBuilder(sb);
            }
        }

        return sb.ToString();
    }
    
    [Fact]
    public Task Omit()
    {
        var source = GetBasicTest(sb =>
        {
            sb.AppendLine("builder");
            using (sb.IncreaseDepth())
            {
                sb.AppendLine(".Omit(x => x.SomeMember1)");
                sb.AppendLine(".Omit(x => x.SomeMember3);");
            }
        });
       
        return TestHelper.VerifySerialization(source);
    }
    
    [Fact]
    public Task OmitLastModified()
    {
        var sb = new StructuredStringBuilder();
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
                sb.AppendLine("public int SomeMember3 { get; set; } = 3;");
                sb.AppendLine("public int LastModified { get; set; } = 5;");
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
                sb.AppendLine("builder.OmitLastModifiedData();");
            }
        }
        
        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task NormalGroup()
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
    public Task FilePerRecordNothingApplicable()
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
    public Task FilePerRecordWithGroup()
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
}