using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class IsMajorRecordTester
{
    public bool IsMajorRecord(ITypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces.Any(x => x.Name.Equals("IMajorRecordGetter"));
    }
}