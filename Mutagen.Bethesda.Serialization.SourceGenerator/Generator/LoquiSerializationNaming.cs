using Loqui;
using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class LoquiSerializationNaming
{
    public record SerializationItems(
        ILoquiRegistration LoquiRegistration,
        string TermName)
    {
        public string SerializationHousingClassName => $"{TermName}_Serialization";
        public string SerializationHousingFileName => $"{TermName}_Serializations.g.cs";

        public string SerializationCall(bool serialize, bool withCheck = false)
        {
            return $"{SerializationHousingClassName}.{(serialize ? "Serialize" : "Deserialize")}{(withCheck ? "WithCheck" : null)}";
        }
    }
    
    public bool TryGetSerializationItems(ITypeSymbol obj, out SerializationItems items)
    {
        if (!LoquiRegistration.TryGetRegisterByFullName($"{obj.ContainingNamespace}.{obj.Name}", out var regis))
        {
            items = default!;
            return false;
        }

        items = new(regis, regis.ClassType.Name);
        return true;
    }
    
    public bool TryGetSerializationItems(Type obj, out SerializationItems items)
    {
        if (!LoquiRegistration.TryGetRegister(obj, out var regis))
        {
            items = default!;
            return false;
        }

        items = new(regis, regis.ClassType.Name);
        return true;
    }
}