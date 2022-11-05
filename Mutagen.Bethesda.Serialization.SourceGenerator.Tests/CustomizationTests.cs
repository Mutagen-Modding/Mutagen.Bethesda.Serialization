using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

[UsesVerify]
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
}