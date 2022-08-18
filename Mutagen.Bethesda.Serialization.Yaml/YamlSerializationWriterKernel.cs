using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationWriterKernel : ISerializationWriterKernel<YamlNode>
{
    public YamlNode GetNewObject()
    {
        throw new NotImplementedException();
    }

    public void WriteString(YamlNode writer, string item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt8(YamlNode writer, sbyte item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt16(YamlNode writer, short item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt32(YamlNode writer, int item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt64(YamlNode writer, long item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt8(YamlNode writer, byte item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt16(YamlNode writer, ushort item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt32(YamlNode writer, uint item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt64(YamlNode writer, ulong item)
    {
        throw new NotImplementedException();
    }
}