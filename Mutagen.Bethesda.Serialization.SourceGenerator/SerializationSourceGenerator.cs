using System.Collections.Immutable;
using System.Text;
using Loqui;
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
        try
        {
            File.AppendAllText("C:\\Test\\Log.txt", $"{nameof(SerializationSourceGenerator)}\n");

            IncrementalValuesProvider<BootstrapInvocation> bootstrapSymbols = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is MemberAccessExpressionSyntax,
                    transform: GetBootstrapInvocation)
                .Where(x => x != null)!;

            context.RegisterSourceOutput(
                context.CompilationProvider.Combine(bootstrapSymbols.Collect()),
                static (context, source) => GenerateStub(source.Item1, source.Item2, context));
        }
        catch (Exception ex)
        {
            File.AppendAllText("C:\\Test\\Log.txt", $"{ex}\n");
        }
    }

    record BootstrapInvocation(INamedTypeSymbol NamedTypeSymbol, ILoquiRegistration? ModRegistration);
    
    static BootstrapInvocation? GetBootstrapInvocation(GeneratorSyntaxContext context, CancellationToken cancel)
    {
        var memberAccessSyntax = (MemberAccessExpressionSyntax)context.Node;
        var expressionSymbol = context.SemanticModel.GetSymbolInfo(memberAccessSyntax.Expression).Symbol;
        if (expressionSymbol is not INamedTypeSymbol namedTypeSymbol) return default;
        if (!namedTypeSymbol.AllInterfaces.Any(x => x.Name == "IMutagenSerializationBootstrap")) return default;
        if (memberAccessSyntax.Parent is InvocationExpressionSyntax invocationExpressionSyntax
            && invocationExpressionSyntax.ArgumentList.Arguments.Count == 1)
        {
            var symb = context.SemanticModel
                .GetSymbolInfo(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression).Symbol;
            var type = symb.TryGetTypeSymbol()?.Name;
            if (LoquiRegistration.TryGetRegisterByFullName($"{symb.ContainingNamespace}.{type}", out var regis))
            {
                return new(namedTypeSymbol, regis);
            }
        }
        return new(namedTypeSymbol, default);
    }

    static void GenerateStub(Compilation compilation, ImmutableArray<BootstrapInvocation> bootstraps, SourceProductionContext context)
    {
        File.AppendAllText("C:\\Test\\Log.txt", "Stub\n");
        var distinctBootstraps = bootstraps.Select(x => x.NamedTypeSymbol.Name).Distinct().ToArray();
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