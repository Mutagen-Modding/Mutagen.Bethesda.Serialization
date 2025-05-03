using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Customization;

public class SkipErrorsTests : ATestsBase
{
    [Fact]
    public Task Off()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public string SomeMember2 { get; set; } = null!;");
                sb.AppendLine("public int SomeMember3 { get; set; } = 2;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "MajorRecord");
        
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
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task On()
    {
        var sb = new StructuredStringBuilder();
        
        GetObjWithMember(sb, sb =>
            {
                sb.AppendLine("public int SomeMember1 { get; set; } = 1;");
                sb.AppendLine("public string SomeMember2 { get; set; } = null!;");
                sb.AppendLine("public int SomeMember3 { get; set; } = 2;");
            },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "MajorRecord");
        
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
                    sb.AppendLine(".SkipRecordsWithErrors();");
                }
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
}