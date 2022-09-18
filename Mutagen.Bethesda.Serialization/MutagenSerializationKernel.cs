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

    public void WriteChar(TWriterObject writer, string? fieldName, char? item, char? defaultVal)
    {
        if (EqualityComparer<char?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteChar(writer, fieldName, item);
    }
    
    public void WriteBool(TWriterObject writer, string? fieldName, bool? item, bool? defaultVal)
    {
        if (EqualityComparer<bool?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteBool(writer, fieldName, item);
    }
    
    public void WriteString(TWriterObject writer, string? fieldName, string? item, string? defaultVal)
    {
        if (EqualityComparer<string?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteString(writer, fieldName, item);
    }
    
    public void WriteInt8(TWriterObject writer, string? fieldName, sbyte? item, sbyte? defaultVal)
    {
        if (EqualityComparer<sbyte?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteInt8(writer, fieldName, item);
    }
    
    public void WriteInt16(TWriterObject writer, string? fieldName, short? item, short? defaultVal)
    {
        if (EqualityComparer<short?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteInt16(writer, fieldName, item);
    }
    
    public void WriteInt32(TWriterObject writer, string? fieldName, int? item, int? defaultVal)
    {
        if (EqualityComparer<int?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteInt32(writer, fieldName, item);
    }
    
    public void WriteInt64(TWriterObject writer, string? fieldName, long? item, long? defaultVal)
    {
        if (EqualityComparer<long?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteInt64(writer, fieldName, item);
    }
    
    public void WriteUInt8(TWriterObject writer, string? fieldName, byte? item, byte? defaultVal)
    {
        if (EqualityComparer<byte?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteUInt8(writer, fieldName, item);
    }
    
    public void WriteUInt16(TWriterObject writer, string? fieldName, ushort? item, ushort? defaultVal)
    {
        if (EqualityComparer<ushort?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteUInt16(writer, fieldName, item);
    }
    
    public void WriteUInt32(TWriterObject writer, string? fieldName, uint? item, uint? defaultVal)
    {
        if (EqualityComparer<uint?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteUInt32(writer, fieldName, item);
    }
    
    public void WriteUInt64(TWriterObject writer, string? fieldName, ulong? item, ulong? defaultVal)
    {
        if (EqualityComparer<ulong?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteUInt64(writer, fieldName, item);
    }
    
    public void WriteFloat(TWriterObject writer, string? fieldName, float? item, float? defaultVal)
    {
        if (EqualityComparer<float?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteFloat(writer, fieldName, item);
    }
    
    public void WriteModKey(TWriterObject writer, string? fieldName, ModKey? item, ModKey? defaultVal)
    {
        if (EqualityComparer<ModKey?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteModKey(writer, fieldName, item);
    }
    
    public void WriteFormKey(TWriterObject writer, string? fieldName, FormKey? item, FormKey? defaultVal)
    {
        if (EqualityComparer<FormKey?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteFormKey(writer, fieldName, item);
    }
    
    public void WriteRecordType(TWriterObject writer, string? fieldName, RecordType? item, RecordType? defaultVal)
    {
        if (EqualityComparer<RecordType?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteRecordType(writer, fieldName, item);
    }
    
    public void WriteP2Int(TWriterObject writer, string? fieldName, P2Int? item, P2Int? defaultVal)
    {
        if (EqualityComparer<P2Int?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteP2Int(writer, fieldName, item);
    }
    
    public void WriteP2Int16(TWriterObject writer, string? fieldName, P2Int16? item, P2Int16? defaultVal)
    {
        if (EqualityComparer<P2Int16?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteP2Int16(writer, fieldName, item);
    }
    
    public void WriteP2Float(TWriterObject writer, string? fieldName, P2Float? item, P2Float? defaultVal)
    {
        if (EqualityComparer<P2Float?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteP2Float(writer, fieldName, item);
    }
    
    public void WriteP3Float(TWriterObject writer, string? fieldName, P3Float? item, P3Float? defaultVal)
    {
        if (EqualityComparer<P3Float?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteP3Float(writer, fieldName, item);
    }
    
    public void WriteP3UInt8(TWriterObject writer, string? fieldName, P3UInt8? item, P3UInt8? defaultVal)
    {
        if (EqualityComparer<P3UInt8?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteP3UInt8(writer, fieldName, item);
    }
    
    public void WriteP3Int16(TWriterObject writer, string? fieldName, P3Int16? item, P3Int16? defaultVal)
    {
        if (EqualityComparer<P3Int16?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteP3Int16(writer, fieldName, item);
    }
    
    public void WriteP3UInt16(TWriterObject writer, string? fieldName, P3UInt16? item, P3UInt16? defaultVal)
    {
        if (EqualityComparer<P3UInt16?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteP3UInt16(writer, fieldName, item);
    }
    
    public void WritePercent(TWriterObject writer, string? fieldName, Percent? item, Percent? defaultVal)
    {
        if (EqualityComparer<Percent?>.Default.Equals(item, defaultVal)) return;
        _kernel.WritePercent(writer, fieldName, item);
    }
    
    public void WriteColor(TWriterObject writer, string? fieldName, Color? item, Color? defaultVal)
    {
        if (EqualityComparer<Color?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteColor(writer, fieldName, item);
    }
    
    public void WriteTranslatedString(TWriterObject writer, string? fieldName, ITranslatedStringGetter? item, ITranslatedStringGetter? defaultVal)
    {
        if (EqualityComparer<ITranslatedStringGetter?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteTranslatedString(writer, fieldName, item);
    }
    
    public void WriteBytes(TWriterObject writer, string? fieldName, ReadOnlyMemorySlice<byte>? item, ReadOnlyMemorySlice<byte>? defaultVal)
    {
        if (EqualityComparer<ReadOnlyMemorySlice<byte>?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteBytes(writer, fieldName, item);
    }
    
    public void WriteEnum<TEnum>(TWriterObject writer, string? fieldName, TEnum? item, TEnum? defaultVal)
        where TEnum : struct, Enum, IConvertible
    {
        if (EqualityComparer<TEnum?>.Default.Equals(item, defaultVal)) return;
        _kernel.WriteEnum<TEnum>(writer, fieldName, item);
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