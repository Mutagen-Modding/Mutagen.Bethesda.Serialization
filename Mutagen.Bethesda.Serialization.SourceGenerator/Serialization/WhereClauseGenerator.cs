using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class WhereClauseGenerator
{
    public IEnumerable<string> GetWheres(INamedTypeSymbol obj)
    {
        return obj.TypeParameters
            .Select(GetWheres)
            .Where(x => x != string.Empty)
            .ToArray();
    }

    private string GetWheres(ITypeParameterSymbol parameterSymbol)
    {
        List<string> ret = new();
        if (parameterSymbol.HasReferenceTypeConstraint)
        {
            ret.Add("class");
        }

        if (parameterSymbol.HasValueTypeConstraint)
        {
            ret.Add("struct");
        }

        if (parameterSymbol.HasNotNullConstraint)
        {
            ret.Add("notnull");
        }

        if (parameterSymbol.HasConstructorConstraint)
        {
            ret.Add("new()");
        }
        
        ret.AddRange(parameterSymbol.ConstraintTypes.Select(x => x.ToString()));

        if (ret.Count == 0) return string.Empty;
        return $"where {parameterSymbol} : {string.Join(", ", ret)}";
    }
}