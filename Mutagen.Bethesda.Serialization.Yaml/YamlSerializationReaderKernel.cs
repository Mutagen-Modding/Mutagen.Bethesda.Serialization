using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationReaderKernel : ISerializationReaderKernel<YamlNode>
{
    public YamlNode GetNewObject(Stream stream)
    {
        throw new NotImplementedException();
    }

    public bool TryGetNextField(YamlNode reader, out string name)
    {
        throw new NotImplementedException();
    }

    public char ReadChar(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public bool ReadBool(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public TEnum ReadEnum<TEnum>(YamlNode reader) where TEnum : struct, Enum, IConvertible
    {
        throw new NotImplementedException();
    }

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

    public FormKey ReadFormKey(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public Color ReadColor(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public RecordType ReadRecordType(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public TranslatedString ReadTranslatedString(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemorySlice<byte> ReadBytes(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public void StartListSection(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public void EndListSection(YamlNode reader)
    {
        throw new NotImplementedException();
    }

    public bool TryHasNextItem(YamlNode reader)
    {
        throw new NotImplementedException();
    }
}