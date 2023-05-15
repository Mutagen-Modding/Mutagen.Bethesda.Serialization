using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class SerializationGenerics
{
    public static readonly SerializationGenerics Instance = new();
    
    public List<string>? ExtraReaderGenerics;
    public List<string>? ExtraWriterGenerics;
    public List<string>? ExtraReaderWheres;
    public List<string>? ExtraWriterWheres;

    public string ReaderGenericsString(bool isMod)
    {
        IEnumerable<string> join = new string[] { "TReadObject" };
        if (isMod)
        {
            join = join.And("TMeta");
        }
        if (ExtraReaderGenerics != null)
        {
            join = join.And(ExtraReaderGenerics);
        }
        if (!join.Any()) return string.Empty;
        return $"<{(string.Join(", ", join))}>";
    }

    public string WriterGenericsString(bool forHas)
    {
        var join = Enumerable.Empty<string>();
        if (!forHas)
        {
            join = join.And("TKernel").And("TWriteObject");
        }
        if (ExtraWriterGenerics != null)
        {
            join = join.And(ExtraWriterGenerics);
        }

        if (!join.Any()) return string.Empty;
        return $"<{(string.Join(", ", join))}>";
    }

    public IEnumerable<string> WriterWheres(bool forHas)
    {
        if (ExtraWriterWheres != null)
        {
            foreach (var extraReaderWhere in ExtraWriterWheres)
            {
                yield return extraReaderWhere;
            }
        }
        if (!forHas)
        {
            yield return "where TKernel : ISerializationWriterKernel<TWriteObject>, new()";
        }
    }

    public IEnumerable<string> ReaderWheres()
    {
        if (ExtraReaderWheres != null)
        {
            foreach (var extraReaderWhere in ExtraReaderWheres)
            {
                yield return extraReaderWhere;
            }
        }
    }
}