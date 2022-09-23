using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Newtonsoft.Json.Linq;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class NewtonsoftJsonSerializationReaderKernel : ISerializationReaderKernel<JTokenReader>
{
    public JTokenReader GetNewObject(Stream stream)
    {
        throw new NotImplementedException();
    }

    public bool TryGetNextField(out string name)
    {
        throw new NotImplementedException();
    }

    public char ReadChar(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public bool ReadBool(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public TEnum ReadEnum<TEnum>(JTokenReader reader) where TEnum : struct, Enum, IConvertible
    {
        throw new NotImplementedException();
    }

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

    public FormKey ReadFormKey(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public Color ReadColor(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public RecordType ReadRecordType(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public TranslatedString ReadTranslatedString(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemorySlice<byte> ReadBytes(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public void StartListSection(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public void EndListSection(JTokenReader reader)
    {
        throw new NotImplementedException();
    }

    public bool TryHasNextItem(JTokenReader reader)
    {
        throw new NotImplementedException();
    }
}