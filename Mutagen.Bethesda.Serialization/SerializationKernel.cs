namespace Mutagen.Bethesda.Serialization;

public interface ISerializationReaderKernel<TReaderObject>
{
    public string ReadString(TReaderObject reader);
    public sbyte ReadInt8(TReaderObject reader);
    public short ReadInt16(TReaderObject reader);
    public int ReadInt32(TReaderObject reader);
    public long ReadInt64(TReaderObject reader);
    public byte ReadUInt8(TReaderObject reader);
    public ushort ReadUInt16(TReaderObject reader);
    public uint ReadUInt32(TReaderObject reader);
    public ulong ReadUInt64(TReaderObject reader);
}

public interface ISerializationWriterKernel<TWriterObject>
{
    public void WriteString(TWriterObject writer, string item);
    public void WriteInt8(TWriterObject writer, sbyte item);
    public void WriteInt16(TWriterObject writer, short item);
    public void WriteInt32(TWriterObject writer, int item);
    public void WriteInt64(TWriterObject writer, long item);
    public void WriteUInt8(TWriterObject writer, byte item);
    public void WriteUInt16(TWriterObject writer, ushort item);
    public void WriteUInt32(TWriterObject writer, uint item);
    public void WriteUInt64(TWriterObject writer, ulong item);
}