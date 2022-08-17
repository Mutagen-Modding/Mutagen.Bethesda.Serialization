using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationReaderKernel : ISerializationReaderKernel<YamlNode>
{
    public string ReadString(YamlNode reader)
    {
        return reader.Anchor.Value;
    }

    public sbyte ReadInt8(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public short ReadInt16(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public int ReadInt32(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public long ReadInt64(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public byte ReadUInt8(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public ushort ReadUInt16(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public uint ReadUInt32(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public ulong ReadUInt64(YamlNode reader)
    {
        throw new NotImplementedException();
    }
}