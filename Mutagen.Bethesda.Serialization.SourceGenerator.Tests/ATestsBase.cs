using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public abstract class ATestsBase
{
    protected string GetModWithMember(
        Action<StructuredStringBuilder> memberBuilder,
        Action<StructuredStringBuilder> outsideBuilder = null)
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine("using Noggog;");
        sb.AppendLine("using Loqui;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Newtonsoft;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Records;");
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
        sb.AppendLine();
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        outsideBuilder?.Invoke(sb);
        
        sb.AppendLine("public partial interface ITestModGetter : IModGetter, ILoquiObject");
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
        }
        
        sb.AppendLine("public partial interface ITestMod : ITestModGetter");
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
        }
        
        using (var c = sb.Class("TestMod"))
        {
            c.Partial = true;
            c.BaseClass = "AMod";
            c.Interfaces.Add("ITestMod");
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
                sb.AppendLine("MutagenTestConverter.Instance.Convert(theMod);");
            }
        }

        return sb.ToString();
    }

    protected string GetObjWithMember(
        Action<StructuredStringBuilder> memberBuilder,
        Action<StructuredStringBuilder>? outsideBuilder = null)
    {
        var sb = new StructuredStringBuilder();
        return GetObjWithMember(sb, memberBuilder, outsideBuilder);
    }

    protected string GetObjWithMember(
        StructuredStringBuilder sb,
        Action<StructuredStringBuilder> memberBuilder,
        Action<StructuredStringBuilder>? namespaceBuilder = null,
        Action<StructuredStringBuilder>? outsideBuilder = null,
        string objName = "SomeObject")
    {
        sb.AppendLine("using Noggog;");
        sb.AppendLine("using Loqui;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Mutagen.Bethesda.Serialization.Newtonsoft;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Records;");
        namespaceBuilder?.Invoke(sb);
        sb.AppendLine();
        
        using var ns = sb.Namespace("Mutagen.Bethesda.Serialization.SourceGenerator.Tests");
        
        outsideBuilder?.Invoke(sb);
        
        sb.AppendLine($"public partial interface I{objName} : I{objName}Getter");
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
        }
        
        sb.AppendLine($"public partial interface I{objName}Getter : ILoquiObject");
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
        }
        
        using (var c = sb.Class($"{objName}_Registration"))
        {
            c.Partial = true;
            c.BaseClass = "ARegistration";
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 15, 0);");
            sb.AppendLine($"public override Type ClassType => typeof({objName});");
            sb.AppendLine($"public override Type GetterType => typeof(I{objName}Getter);");
            sb.AppendLine($"public override Type SetterType => typeof(I{objName});");
            sb.AppendLine($"public override string Name => nameof({objName});");
        }

        using (var c = sb.Class(objName))
        {
            c.Partial = true;
            c.Interfaces.Add($"I{objName}");
        }
        using (sb.CurlyBrace())
        {
            memberBuilder(sb);
            sb.AppendLine($"public ILoquiRegistration Registration {{ get; }} = new {objName}_Registration();");
            sb.AppendLine();
            using (var f = sb.Function("public void SomeFunction"))
            {
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine($"var theObj = new {objName}();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(theObj);");
            }
        }

        return sb.ToString();
    }

    protected void GenerateGroup(StructuredStringBuilder sb)
    {
        using (var c = sb.Class("Group<T>"))
        {
            c.Interfaces.Add("IGroup<T>");
            c.Interfaces.Add("IGroupGetter<T>");
        }
        using (sb.CurlyBrace())
        {}
        
    }
}