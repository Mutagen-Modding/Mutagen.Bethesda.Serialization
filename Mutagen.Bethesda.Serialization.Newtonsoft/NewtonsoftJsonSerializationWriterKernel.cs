using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Newtonsoft.Json.Linq;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class NewtonsoftJsonSerializationWriterKernel : ISerializationWriterKernel<JTokenWriter>
{
    public JTokenWriter GetNewObject()
    {
        throw new NotImplementedException();
    }

    public void WriteChar(JTokenWriter writer, string? fieldName, char? item)
    {
        throw new NotImplementedException();
    }

    public void WriteBool(JTokenWriter writer, string? fieldName, bool? item)
    {
        throw new NotImplementedException();
    }

    public void WriteString(JTokenWriter writer, string? fieldName, string? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt8(JTokenWriter writer, string? fieldName, sbyte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt16(JTokenWriter writer, string? fieldName, short? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt32(JTokenWriter writer, string? fieldName, int? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt64(JTokenWriter writer, string? fieldName, long? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt8(JTokenWriter writer, string? fieldName, byte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt16(JTokenWriter writer, string? fieldName, ushort? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt32(JTokenWriter writer, string? fieldName, uint? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt64(JTokenWriter writer, string? fieldName, ulong? item)
    {
        throw new NotImplementedException();
    }

    public void WriteFloat(JTokenWriter writer, string? fieldName, float? item)
    {
        throw new NotImplementedException();
    }

    public void WriteFormKey(JTokenWriter writer, string? fieldName, FormKey? formKey)
    {
        throw new NotImplementedException();
    }

    public void WriteRecordType(JTokenWriter writer, string? fieldName, RecordType? recordType)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int(JTokenWriter writer, string? fieldName, P2Int? p2)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int16(JTokenWriter writer, string? fieldName, P2Int16? p2)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Float(JTokenWriter writer, string? fieldName, P2Float? p3)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Float(JTokenWriter writer, string? fieldName, P3Float? p3Float)
    {
        throw new NotImplementedException();
    }

    public void WriteP3UInt8(JTokenWriter writer, string? fieldName, P3UInt8? p3)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Int16(JTokenWriter writer, string? fieldName, P3Int16? p3)
    {
        throw new NotImplementedException();
    }

    public void WriteP3UInt16(JTokenWriter writer, string? fieldName, P3UInt16? p3)
    {
        throw new NotImplementedException();
    }

    public void WritePercent(JTokenWriter writer, string? fieldName, Percent? percent)
    {
        throw new NotImplementedException();
    }

    public void WriteColor(JTokenWriter writer, string? fieldName, Color? color)
    {
        throw new NotImplementedException();
    }

    public void WriteTranslatedString(JTokenWriter writer, string? fieldName, ITranslatedStringGetter? translatedString)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(JTokenWriter writer, string? fieldName, ReadOnlyMemorySlice<byte>? bytes)
    {
        throw new NotImplementedException();
    }

    public void WriteEnum<TEnum>(JTokenWriter writer, string? fieldName, TEnum? item) where TEnum : struct, Enum, IConvertible
    {
        throw new NotImplementedException();
    }

    public JTokenWriter StartListSection(JTokenWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void EndListSection()
    {
        throw new NotImplementedException();
    }

    public JTokenWriter StartDictionarySection(JTokenWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void StopDictionarySection()
    {
        throw new NotImplementedException();
    }

    public JTokenWriter StartDictionaryItem(JTokenWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryItem()
    {
        throw new NotImplementedException();
    }

    public JTokenWriter StartDictionaryKey(JTokenWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryKey()
    {
        throw new NotImplementedException();
    }

    public JTokenWriter StartDictionaryValue(JTokenWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryValue()
    {
        throw new NotImplementedException();
    }

    public JTokenWriter StartArray2dSection(JTokenWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void StopArray2dSectionSection()
    {
        throw new NotImplementedException();
    }

    public void StopDictionarySectionSection()
    {
        throw new NotImplementedException();
    }

    public JTokenWriter StartArray2dItem(JTokenWriter writer, int x, int y)
    {
        throw new NotImplementedException();
    }

    public void StopArray2dItem()
    {
        throw new NotImplementedException();
    }
}