using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Serialization;

public interface ISerializationReaderKernel<TReaderObject>
{
    public TReaderObject GetNewObject(Stream stream);
    public bool ReadBool(TReaderObject reader);
    public TEnum ReadEnum<TEnum>(TReaderObject reader)
        where TEnum : struct, Enum, IConvertible;
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
    public Color ReadColor(TReaderObject reader);
    public RecordType ReadRecordType(TReaderObject reader);
    public TranslatedString ReadTranslatedString(TReaderObject reader);
    public ReadOnlyMemorySlice<byte> ReadBytes(TReaderObject reader);
}

public interface ISerializationWriterKernel<TWriterObject>
{
    public TWriterObject GetNewObject(Stream stream);
    public void Finalize(Stream stream, TWriterObject writer);
    public void WriteChar(TWriterObject writer, string? fieldName, char? item);
    public void WriteBool(TWriterObject writer, string? fieldName, bool? item);
    public void WriteString(TWriterObject writer, string? fieldName, string? item);
    public void WriteInt8(TWriterObject writer, string? fieldName, sbyte? item);
    public void WriteInt16(TWriterObject writer, string? fieldName, short? item);
    public void WriteInt32(TWriterObject writer, string? fieldName, int? item);
    public void WriteInt64(TWriterObject writer, string? fieldName, long? item);
    public void WriteUInt8(TWriterObject writer, string? fieldName, byte? item);
    public void WriteUInt16(TWriterObject writer, string? fieldName, ushort? item);
    public void WriteUInt32(TWriterObject writer, string? fieldName, uint? item);
    public void WriteUInt64(TWriterObject writer, string? fieldName, ulong? item);
    public void WriteFloat(TWriterObject writer, string? fieldName, float? item);
    public void WriteFormKey(TWriterObject writer, string? fieldName, FormKey? formKey);
    public void WriteRecordType(TWriterObject writer, string? fieldName, RecordType? recordType);
    public void WriteP2Int(TWriterObject writer, string? fieldName, P2Int? p2);
    public void WriteP2Int16(TWriterObject writer, string? fieldName, P2Int16? p2);
    public void WriteP2Float(TWriterObject writer, string? fieldName, P2Float? p3);
    public void WriteP3Float(TWriterObject writer, string? fieldName, P3Float? p3);
    public void WriteP3UInt8(TWriterObject writer, string? fieldName, P3UInt8? p3);
    public void WriteP3Int16(TWriterObject writer, string? fieldName, P3Int16? p3);
    public void WriteP3UInt16(TWriterObject writer, string? fieldName, P3UInt16? p3);
    public void WritePercent(TWriterObject writer, string? fieldName, Percent? percent);
    public void WriteColor(TWriterObject writer, string? fieldName, Color? color);
    public void WriteTranslatedString(TWriterObject writer, string? fieldName, ITranslatedStringGetter? translatedString);
    public void WriteBytes(TWriterObject writer, string? fieldName, ReadOnlyMemorySlice<byte>? bytes);
    public void WriteEnum<TEnum>(TWriterObject writer, string? fieldName, TEnum? item)
        where TEnum : struct, Enum, IConvertible;

    #region List
    
    public TWriterObject StartListSection(TWriterObject writer, string? fieldName);
    public void EndListSection();
    
    #endregion

    #region Dict

    public TWriterObject StartDictionarySection(TWriterObject writer, string? fieldName);
    public void StopDictionarySection();
    public TWriterObject StartDictionaryItem(TWriterObject writer);
    public void StopDictionaryItem();
    public TWriterObject StartDictionaryKey(TWriterObject writer);
    public void StopDictionaryKey();
    public TWriterObject StartDictionaryValue(TWriterObject writer);
    public void StopDictionaryValue();

    #endregion

    #region Array2d

    public TWriterObject StartArray2dSection(TWriterObject writer, string? fieldName);
    public void StopArray2dSectionSection();
    public TWriterObject StartArray2dItem(TWriterObject writer, int x, int y);
    public void StopArray2dItem();

    #endregion
}