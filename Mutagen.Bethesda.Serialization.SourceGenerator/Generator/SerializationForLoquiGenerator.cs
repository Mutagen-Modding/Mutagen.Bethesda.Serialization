using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationForLoquiGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<BootstrapInvocation> modBootstrapInvocations)
    {
        context.RegisterSourceOutput(
            modBootstrapInvocations.SelectMany(x =>
            {
                
            }),
            Generate);
    }
    
    public void Generate(SourceProductionContext context, ImmutableArray<BootstrapInvocation> bootstrap)
    {
        if (bootstrap.ModRegistration == null) return;
        var sb = new StructuredStringBuilder();

        sb.AppendLine($"using {bootstrap.ModRegistration.Namespace};");
        
        using (sb.Namespace(bootstrap.NamedTypeSymbol.Namespace))
        {
        }
        
        using (var c = sb.Class($"{bootstrap.NamedTypeSymbol.ClassName}{bootstrap.ModRegistration.ClassName}MixIns"))
        {
            c.AccessModifier = AccessModifier.Public;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function($"public static string Convert"))
            {
                args.Add($"this {bootstrap.NamedTypeSymbol} converterBootstrap");
                args.Add($"{bootstrap.ModRegistration} mod");
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("throw new NotImplementedException();");
            }
        }
        sb.AppendLine();

        context.AddSource($"{bootstrap.NamedTypeSymbol.ClassName}_{bootstrap.ModRegistration.ClassName}_MixIns.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}