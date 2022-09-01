using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationWriterKernel : ISerializationWriterKernel<YamlNode>
{
    public YamlNode GetNewObject()
    {
        throw new NotImplementedException();
    }

    public void WriteBool(YamlNode writer, string? fieldName, bool? item)
    {
        throw new NotImplementedException();
    }

    public void WriteString(YamlNode writer, string? fieldName, string? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt8(YamlNode writer, string? fieldName, sbyte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt16(YamlNode writer, string? fieldName, short? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt32(YamlNode writer, string? fieldName, int? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt64(YamlNode writer, string? fieldName, long? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt8(YamlNode writer, string? fieldName, byte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt16(YamlNode writer, string? fieldName, ushort? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt32(YamlNode writer, string? fieldName, uint? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt64(YamlNode writer, string? fieldName, ulong? item)
    {
        throw new NotImplementedException();
    }

    public void WriteFloat(YamlNode writer, string? fieldName, float? item)
    {
        throw new NotImplementedException();
    }

    public void WriteFormKey(YamlNode writer, string? fieldName, FormKey? formKey)
    {
        throw new NotImplementedException();
    }

    public void WriteRecordType(YamlNode writer, string? fieldName, RecordType? recordType)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int(YamlNode writer, string? fieldName, P2Int? p2)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int16(YamlNode writer, string? fieldName, P2Int16? p2)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Float(YamlNode writer, string? fieldName, P3Float? p3Float)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Int16(YamlNode writer, string? fieldName, P3Int16? p3)
    {
        throw new NotImplementedException();
    }

    public void WritePercent(YamlNode writer, string? fieldName, Percent? percent)
    {
        throw new NotImplementedException();
    }

    public void WriteColor(YamlNode writer, string? fieldName, Color? color)
    {
        throw new NotImplementedException();
    }

    public void WriteTranslatedString(YamlNode writer, string? fieldName, ITranslatedStringGetter? translatedString)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(YamlNode writer, string? fieldName, ReadOnlyMemorySlice<byte>? bytes)
    {
        throw new NotImplementedException();
    }

    public void WriteEnum<TEnum>(YamlNode writer, string? fieldName, TEnum? item)
        where TEnum : struct, Enum, IConvertible
    {
        throw new NotImplementedException();
    }

    public YamlNode StartListSection(YamlNode writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void EndListSection()
    {
        throw new NotImplementedException();
    }

    public YamlNode StartDictionarySection(YamlNode writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public YamlNode StartDictionaryItem(YamlNode writer)
    {
        throw new NotImplementedException();
    }

    public YamlNode StartDictionaryKey(YamlNode writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryKey()
    {
        throw new NotImplementedException();
    }

    public YamlNode StartDictionaryValue(YamlNode writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryValue()
    {
        throw new NotImplementedException();
    }
}