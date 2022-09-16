using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Serialization;

public class MutagenSerializationWriterKernel<TKernel, TWriterObject>
    where TKernel : ISerializationWriterKernel<TWriterObject>, new()
{
    private readonly TKernel _kernel = new();
    public static readonly MutagenSerializationWriterKernel<TKernel, TWriterObject> Instance = new();

    public TWriterObject GetNewObject(Stream stream) => _kernel.GetNewObject(stream);
    public void Finalize(Stream stream, TWriterObject writer) => _kernel.Finalize(stream, writer);

    public void WriteChar(TWriterObject writer, string? fieldName, char? item)
    {
        if (item == null) return;
        _kernel.WriteChar(writer, fieldName, item.Value);
    }
    
    public void WriteBool(TWriterObject writer, string? fieldName, bool? item)
    {
        if (item == null) return;
        _kernel.WriteBool(writer, fieldName, item.Value);
    }
    
    public void WriteString(TWriterObject writer, string? fieldName, string? item)
    {
        if (item == null) return;
        _kernel.WriteString(writer, fieldName, item);
    }
    
    public void WriteInt8(TWriterObject writer, string? fieldName, sbyte? item)
    {
        if (item == null) return;
        _kernel.WriteInt8(writer, fieldName, item.Value);
    }
    
    public void WriteInt16(TWriterObject writer, string? fieldName, short? item)
    {
        if (item == null) return;
        _kernel.WriteInt16(writer, fieldName, item.Value);
    }
    
    public void WriteInt32(TWriterObject writer, string? fieldName, int? item)
    {
        if (item == null) return;
        _kernel.WriteInt32(writer, fieldName, item.Value);
    }
    
    public void WriteInt64(TWriterObject writer, string? fieldName, long? item)
    {
        if (item == null) return;
        _kernel.WriteInt64(writer, fieldName, item.Value);
    }
    
    public void WriteUInt8(TWriterObject writer, string? fieldName, byte? item)
    {
        if (item == null) return;
        _kernel.WriteUInt8(writer, fieldName, item.Value);
    }
    
    public void WriteUInt16(TWriterObject writer, string? fieldName, ushort? item)
    {
        if (item == null) return;
        _kernel.WriteUInt16(writer, fieldName, item.Value);
    }
    
    public void WriteUInt32(TWriterObject writer, string? fieldName, uint? item)
    {
        if (item == null) return;
        _kernel.WriteUInt32(writer, fieldName, item.Value);
    }
    
    public void WriteUInt64(TWriterObject writer, string? fieldName, ulong? item)
    {
        if (item == null) return;
        _kernel.WriteUInt64(writer, fieldName, item.Value);
    }
    
    public void WriteFloat(TWriterObject writer, string? fieldName, float? item)
    {
        if (item == null) return;
        _kernel.WriteFloat(writer, fieldName, item.Value);
    }
    
    public void WriteModKey(TWriterObject writer, string? fieldName, ModKey? item)
    {
        if (item == null) return;
        _kernel.WriteModKey(writer, fieldName, item.Value);
    }
    
    public void WriteFormKey(TWriterObject writer, string? fieldName, FormKey? item)
    {
        if (item == null) return;
        _kernel.WriteFormKey(writer, fieldName, item.Value);
    }
    
    public void WriteRecordType(TWriterObject writer, string? fieldName, RecordType? item)
    {
        if (item == null) return;
        _kernel.WriteRecordType(writer, fieldName, item.Value);
    }
    
    public void WriteP2Int(TWriterObject writer, string? fieldName, P2Int? item)
    {
        if (item == null) return;
        _kernel.WriteP2Int(writer, fieldName, item.Value);
    }
    
    public void WriteP2Int16(TWriterObject writer, string? fieldName, P2Int16? item)
    {
        if (item == null) return;
        _kernel.WriteP2Int16(writer, fieldName, item.Value);
    }
    
    public void WriteP2Float(TWriterObject writer, string? fieldName, P2Float? item)
    {
        if (item == null) return;
        _kernel.WriteP2Float(writer, fieldName, item.Value);
    }
    
    public void WriteP3Float(TWriterObject writer, string? fieldName, P3Float? item)
    {
        if (item == null) return;
        _kernel.WriteP3Float(writer, fieldName, item.Value);
    }
    
    public void WriteP3UInt8(TWriterObject writer, string? fieldName, P3UInt8? item)
    {
        if (item == null) return;
        _kernel.WriteP3UInt8(writer, fieldName, item.Value);
    }
    
    public void WriteP3Int16(TWriterObject writer, string? fieldName, P3Int16? item)
    {
        if (item == null) return;
        _kernel.WriteP3Int16(writer, fieldName, item.Value);
    }
    
    public void WriteP3UInt16(TWriterObject writer, string? fieldName, P3UInt16? item)
    {
        if (item == null) return;
        _kernel.WriteP3UInt16(writer, fieldName, item.Value);
    }
    
    public void WritePercent(TWriterObject writer, string? fieldName, Percent? item)
    {
        if (item == null) return;
        _kernel.WritePercent(writer, fieldName, item.Value);
    }
    
    public void WriteColor(TWriterObject writer, string? fieldName, Color? item)
    {
        if (item == null) return;
        _kernel.WriteColor(writer, fieldName, item.Value);
    }
    
    
    public void WriteTranslatedString(TWriterObject writer, string? fieldName, ITranslatedStringGetter? item)
    {
        if (item == null) return;
        _kernel.WriteTranslatedString(writer, fieldName, item);
    }
    
    public void WriteBytes(TWriterObject writer, string? fieldName, ReadOnlyMemorySlice<byte>? item)
    {
        if (item == null) return;
        _kernel.WriteBytes(writer, fieldName, item.Value);
    }
    
    public void WriteEnum<TEnum>(TWriterObject writer, string? fieldName, TEnum? item)
        where TEnum : struct, Enum, IConvertible
    {
        if (item == null) return;
        _kernel.WriteEnum<TEnum>(writer, fieldName, item.Value);
    }
    
    public void WriteWithName<TObject>(
        TWriterObject writer, 
        string? fieldName, 
        TObject? item,
        Write<TKernel, TWriterObject, TObject> writeCall)
    {
        if (item == null) return;
        _kernel.WriteWithName(this, writer, fieldName, item, writeCall);
    }
    
    public void WriteLoqui<TObject>(
        TWriterObject writer,
        string? fieldName,
        TObject? item,
        Write<TKernel, TWriterObject, TObject> writeCall)
    {
        if (item == null) return;
        _kernel.WriteLoqui(this, writer, fieldName, item, writeCall);
    }

    public void StartListSection(TWriterObject writer, string? fieldName) => _kernel.StartListSection(writer, fieldName);
    
    public void EndListSection(TWriterObject writer) => _kernel.EndListSection(writer);

    public void StartDictionarySection(TWriterObject writer, string? fieldName) => _kernel.StartDictionarySection(writer, fieldName);
    
    public void EndDictionarySection(TWriterObject writer) => _kernel.EndDictionarySection(writer);
    
    public void StartDictionaryItem(TWriterObject writer) => _kernel.StartDictionaryItem(writer);
    
    public void EndDictionaryItem(TWriterObject writer) => _kernel.EndDictionaryItem(writer);
    
    public void StartDictionaryKey(TWriterObject writer) => _kernel.StartDictionaryKey(writer);
    
    public void EndDictionaryKey(TWriterObject writer) => _kernel.EndDictionaryKey(writer);
    
    public void StartDictionaryValue(TWriterObject writer) => _kernel.StartDictionaryValue(writer);
    
    public void EndDictionaryValue(TWriterObject writer) => _kernel.EndDictionaryValue(writer);

    public void StartArray2dSection(TWriterObject writer, string? fieldName) => _kernel.StartArray2dSection(writer, fieldName);
    
    public void EndArray2dSection(TWriterObject writer) => _kernel.EndArray2dSection(writer);
    
    public void StartArray2dXSection(TWriterObject writer) => _kernel.StartArray2dXSection(writer);
    
    public void EndArray2dXSection(TWriterObject writer) => _kernel.EndArray2dXSection(writer);
    
    public void StartArray2dYSection(TWriterObject writer) => _kernel.StartArray2dYSection(writer);
    
    public void EndArray2dYSection(TWriterObject writer) => _kernel.EndArray2dYSection(writer);
    
}