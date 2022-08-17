using Newtonsoft.Json.Linq;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class NewtonsoftJsonSerializationReaderKernel : ISerializationReaderKernel<JTokenReader>
{
    public string ReadString(JTokenReader reader)
    {
        return reader.CurrentToken?.ToString() ?? String.Empty;
    }

    public sbyte ReadInt8(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public short ReadInt16(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public int ReadInt32(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public long ReadInt64(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public byte ReadUInt8(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public ushort ReadUInt16(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public uint ReadUInt32(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public ulong ReadUInt64(JTokenReader reader)
    {
        throw new NotImplementedException();
    }
}