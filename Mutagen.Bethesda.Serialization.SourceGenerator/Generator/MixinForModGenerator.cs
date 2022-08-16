using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class MixinForModGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<BootstrapInvocation> modBootstrapInvocations)
    {
        context.RegisterSourceOutput(
            modBootstrapInvocations,
            Generate);
    }
    
    public void Generate(SourceProductionContext context, BootstrapInvocation bootstrap)
    {
        if (bootstrap.ModRegistration == null) return;
        var sb = new StructuredStringBuilder();

        sb.AppendLine($"using {bootstrap.ModRegistration.ContainingNamespace};");
        
        using (sb.Namespace(bootstrap.Bootstrap.ContainingNamespace.ToString()))
        {
        }
        
        using (var c = sb.Class($"{bootstrap.Bootstrap.Name}{bootstrap.ModRegistration.Name}MixIns"))
        {
            c.AccessModifier = AccessModifier.Public;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function($"public static string Convert"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{bootstrap.ModRegistration} mod");
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("throw new NotImplementedException();");
            }
        }
        sb.AppendLine();

        context.AddSource($"{bootstrap.Bootstrap.Name}_{bootstrap.ModRegistration.Name}_MixIns.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}