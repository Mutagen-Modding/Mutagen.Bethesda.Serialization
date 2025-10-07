using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Customization;

public class OmitCustomizationTests : ATestsBase
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
    public Task OmitTimestamp()
    {
        var sb = new StructuredStringBuilder();
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
                sb.AppendLine("public int SomeMember3 { get; set; } = 3;");
                sb.AppendLine("public int Timestamp { get; set; } = 5;");
                sb.AppendLine("public int PersistentTimestamp { get; set; } = 5;");
                sb.AppendLine("public int TemporaryTimestamp { get; set; } = 5;");
                sb.AppendLine("public int SubCellsTimestamp { get; set; } = 5;");
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
                sb.AppendLine("builder.OmitTimestampData();");
            }
        }
        
        return TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public Task OmitUnusedConditionDataFields()
    {
        var sb = new StructuredStringBuilder();
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
                sb.AppendLine("public int Unused1 { get; set; } = 3;");
                sb.AppendLine("public int UnusedField { get; set; } = 4;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeConditionData");
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
                sb.AppendLine("builder.OmitUnusedConditionDataFields();");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public Task OmitUnusedConditionDataFields_OnlyOmitsFromConditionDataObjects()
    {
        var sb = new StructuredStringBuilder();
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public int SomeMember2 { get; set; } = 2;");
                sb.AppendLine("public int Unused1 { get; set; } = 3;");
                sb.AppendLine("public int UnusedField { get; set; } = 4;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SomeOtherObject");
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
                sb.AppendLine("builder.OmitUnusedConditionDataFields();");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
}