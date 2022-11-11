using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public record SerializationItems(
    string Namespace,
    string TermName)
{
    public string SerializationHousingClassName => $"{TermName}_Serialization";
    public string SerializationHousingFileName => $"{TermName}_Serializations.g.cs";

    public string SerializationCall(bool withCheck = false)
    {
        return $"{Namespace}.{SerializationHousingClassName}.Serialize{(withCheck ? "WithCheck" : null)}";
    }

    public string DeserializationCall(bool withCheck = false)
    {
        return $"{Namespace}.{SerializationHousingClassName}.Deserialize{(withCheck ? "WithCheck" : null)}";
    }

    public string DeserializationIntoCall()
    {
        return $"{Namespace}.{SerializationHousingClassName}.DeserializeInto";
    }

    public string DeserializationSingleFieldIntoCall()
    {
        return $"{Namespace}.{SerializationHousingClassName}.DeserializeSingleFieldInto";
    }

    public string HasSerializationCall(bool withCheck = false)
    {
        return $"{Namespace}.{SerializationHousingClassName}.HasSerializationItems{(withCheck ? "WithCheck" : null)}";
    }
}

public class LoquiSerializationNaming
{
    private readonly LoquiNameRetriever _nameRetriever;

    public LoquiSerializationNaming(
        LoquiNameRetriever nameRetriever)
    {
        _nameRetriever = nameRetriever;
    }
    
    public bool TryGetSerializationItems(ITypeSymbol obj, out SerializationItems items)
    {
        var names = _nameRetriever.GetNames(obj);

        items = new(obj.ContainingNamespace.ToString(), names.Direct);
        return true;
    }
}