using Newtonsoft.Json.Linq;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class NewtonsoftJsonSerializationWriterKernel : ISerializationWriterKernel<JTokenWriter>
{
    public void WriteString(JTokenWriter writer, string item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt8(JTokenWriter writer, sbyte item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt16(JTokenWriter writer, short item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt32(JTokenWriter writer, int item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt64(JTokenWriter writer, long item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt8(JTokenWriter writer, byte item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt16(JTokenWriter writer, ushort item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt32(JTokenWriter writer, uint item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt64(JTokenWriter writer, ulong item)
    {
        throw new NotImplementedException();
    }
}