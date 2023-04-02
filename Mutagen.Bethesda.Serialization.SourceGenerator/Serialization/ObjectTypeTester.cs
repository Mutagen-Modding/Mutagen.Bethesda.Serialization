using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class ObjectTypeTester
{
    public bool IsModObject(ITypeSymbol typeSymbol)
    {
        return typeSymbol.Name.Contains("Mod") 
               && typeSymbol.AllInterfaces
                   .Any(x => x.ToString().Contains("IModGetter"));
    }
    
    public bool IsMajorRecordObject(ITypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces
            .Any(x => x.Name.ToString() == "IMajorRecordGetter");
    }
    
    public bool IsGroup(ITypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces
            .Any(x =>
            {
                var str = x.ToString();
                return str.Contains("IGroupGetter") || str.Contains("IListGroupGetter");
            });
    }

    public bool IsConcrete(ITypeSymbol? typeSymbol)
    {
        return typeSymbol is INamedTypeSymbol { IsAbstract: false } named;
    }

    public bool IsModHeader(ITypeSymbol typeSymbol)
    {
        return typeSymbol.Name.Contains("ModHeader");
    }
}