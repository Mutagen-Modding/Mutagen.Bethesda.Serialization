using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Serialization;

public interface ISerializationReaderKernel<TReaderObject>
{
    public TReaderObject GetNewObject();
    public string ReadString(TReaderObject reader);
    public sbyte ReadInt8(TReaderObject reader);
    public short ReadInt16(TReaderObject reader);
    public int ReadInt32(TReaderObject reader);
    public long ReadInt64(TReaderObject reader);
    public byte ReadUInt8(TReaderObject reader);
    public ushort ReadUInt16(TReaderObject reader);
    public uint ReadUInt32(TReaderObject reader);
    public ulong ReadUInt64(TReaderObject reader);
    public FormKey ReadFormKey(TReaderObject reader);
}

public interface ISerializationWriterKernel<TWriterObject>
{
    public TWriterObject GetNewObject();
    public void WriteString(TWriterObject writer, string fieldName, string? item);
    public void WriteInt8(TWriterObject writer, string fieldName, sbyte? item);
    public void WriteInt16(TWriterObject writer, string fieldName, short? item);
    public void WriteInt32(TWriterObject writer, string fieldName, int? item);
    public void WriteInt64(TWriterObject writer, string fieldName, long? item);
    public void WriteUInt8(TWriterObject writer, string fieldName, byte? item);
    public void WriteUInt16(TWriterObject writer, string fieldName, ushort? item);
    public void WriteUInt32(TWriterObject writer, string fieldName, uint? item);
    public void WriteUInt64(TWriterObject writer, string fieldName, ulong? item);
    public void WriteFormKey(TextWriter writer, string fieldName, FormKey? formKey);
}