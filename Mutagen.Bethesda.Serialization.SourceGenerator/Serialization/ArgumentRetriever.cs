using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class ArgumentRetriever
{
    public ISymbol? Get(
        GeneratorSyntaxContext context,
        SeparatedSyntaxList<ArgumentSyntax> args, 
        int number, 
        string argName)
    {
        foreach (var arg in args)
        {
            if (arg.NameColon?.Name.ToString() == argName)
            {
                return context.SemanticModel.GetSymbolInfo(arg.Expression).Symbol;
            }
        }

        if (args.Count > number)
        {
            var arg = args[number];
            if (arg.Expression is CastExpressionSyntax cast)
            {
                var symbInfo = context.SemanticModel.GetSymbolInfo(cast.Type);
                return symbInfo.Symbol;
            }
            else
            {
                var symbInfo = context.SemanticModel.GetSymbolInfo(arg.Expression);
                return symbInfo.Symbol;
            }
        }

        return null;
    }
}