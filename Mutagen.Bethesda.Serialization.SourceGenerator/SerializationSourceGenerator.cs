using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator;

[Generator]
public class SerializationSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var bootstraps = GetBootstraps()
            .Select(x => x.Name)
            .ToImmutableHashSet();
        if (bootstraps.Count == 0) return;
        
        IncrementalValuesProvider<MemberAccessExpressionSyntax> bootstrapInvocations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => IsBootstrapInvocation(s, bootstraps),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx));

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(bootstrapInvocations.Collect()),
            static (context, source) => GenerateStub(source.Item1, source.Item2, context));
    }

    private IEnumerable<Type> GetBootstraps()
    {
        foreach (var type in TypeExt.IterateTypes())
        {
            if (!type.IsClass) continue;
            foreach (var interf in type.GetInterfaces())
            {
                if (!interf.IsGenericType) continue;
                if (interf.GenericTypeArguments.Length != 2) continue;
                if (interf.Name != "IMutagenSerializationBootstrap`2") continue;
                yield return type;
                break;
            }
        }
    }

    private static bool IsBootstrapInvocation(SyntaxNode node, IReadOnlyCollection<string> bootstrapNames)
    {
        if (node is not MemberAccessExpressionSyntax memberExpression) return false;
        return bootstrapNames.Contains(memberExpression.Expression.ToString());
    }

    static MemberAccessExpressionSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        return (MemberAccessExpressionSyntax)context.Node;
    }
    
    static void GenerateStub(Compilation compilation, ImmutableArray<MemberAccessExpressionSyntax> bootstrapInvocations, SourceProductionContext context)
    {
        var distinctBootstraps = bootstrapInvocations.Select(x => x.Expression.ToString()).Distinct().ToArray();
        if (distinctBootstraps.Length == 0) return;
        StructuredStringBuilder sb = new();
        foreach (var bootstrapClass in distinctBootstraps)
        {
            using (var c = sb.Class(bootstrapClass))
            {
                c.AccessModifier = AccessModifier.Public;
                c.Partial = true;
            }
            using (sb.CurlyBrace())
            {
                using (var args = sb.Function($"public static string Convert"))
                {
                    args.Add("IModGetter mod");
                }

                using (sb.CurlyBrace())
                {
                    sb.AppendLine("throw new NotImplementedException();");
                }
            }
            sb.AppendLine();
        }
        context.AddSource("StubMixIns.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));

    }
}