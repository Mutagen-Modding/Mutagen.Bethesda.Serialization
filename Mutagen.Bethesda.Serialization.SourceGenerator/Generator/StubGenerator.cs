using System.Collections.Immutable;
using System.Text;
using Loqui;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class StubGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<BootstrapInvocation> bootstrapSymbols)
    {
        var allSymbols = bootstrapSymbols.Collect()
            .SelectMany((allSymbols, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                var dict = new Dictionary<ClassDetails, List<BootstrapInvocation>>();
                foreach (var bootstrap in allSymbols)
                {
                    dict.GetOrAdd(bootstrap.NamedTypeSymbol).Add(bootstrap);
                }

                cancel.ThrowIfCancellationRequested();
                return dict
                    .Where(kv => kv.Value.All(x => x.ModRegistration == null))
                    .Select(x => x.Key)
                    .ToImmutableHashSet();
            });
        
        context.RegisterSourceOutput(allSymbols, Generate);
    }
    
    public void Generate(SourceProductionContext context, ClassDetails bootstrap)
    {
        StructuredStringBuilder sb = new();
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Records;");
        sb.AppendLine();
        using (sb.Namespace(bootstrap.Namespace))
        {
        }
        
        using (var c = sb.Class($"{bootstrap.ClassName}MixIns"))
        {
            c.AccessModifier = AccessModifier.Public;
            c.Static = true;
        }

        using (sb.CurlyBrace())
        {
            using (var args = sb.Function($"public static string Convert"))
            {
                args.Add($"this {bootstrap} converterBootstrap");
                args.Add("IModGetter mod");
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("throw new NotImplementedException();");
            }
        }
        sb.AppendLine();

        context.AddSource($"{bootstrap.ClassName}StubMixIn.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}