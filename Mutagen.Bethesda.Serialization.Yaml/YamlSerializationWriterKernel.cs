using System.Drawing;
using System.Globalization;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Mutagen.Bethesda.Serialization.Yaml;

public readonly struct YamlWritingUnit
{
    public readonly IEmitter Emitter;
    public readonly StreamWriter StreamWriter;

    public YamlWritingUnit(Stream stream)
    {
        StreamWriter = new StreamWriter(stream, leaveOpen: true);
        Emitter = new Emitter(StreamWriter);
    }

    public void WriteName(string? name)
    {
        if (name != null)
        {
            WriteScalar(name);
        }
    }

    public void WriteScalar(string str)
    {
        Emitter.Emit(new Scalar(str));
    }
}

public class YamlSerializationWriterKernel : ISerializationWriterKernel<YamlWritingUnit>
{
    public YamlWritingUnit GetNewObject(Stream stream)
    {
        var ret = new YamlWritingUnit(stream);
        ret.Emitter.Emit(new StreamStart());
        ret.Emitter.Emit(new DocumentStart());
        ret.Emitter.Emit(new MappingStart());
        return ret;
    }

    public void Finalize(Stream stream, YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new MappingEnd());
        writer.Emitter.Emit(new DocumentEnd(true));
        writer.Emitter.Emit(new StreamEnd());
        writer.StreamWriter.Dispose();
    }

    public void WriteChar(YamlWritingUnit writer, string? fieldName, char? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteBool(YamlWritingUnit writer, string? fieldName, bool? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteString(YamlWritingUnit writer, string? fieldName, string? item)
    {
        writer.WriteName(fieldName);
        if (item == null)
        {
            writer.Emitter.Emit(new MappingStart());
            writer.Emitter.Emit(new MappingEnd());
        }
        else
        {
            writer.WriteScalar(item);
        }
    }

    public void WriteInt8(YamlWritingUnit writer, string? fieldName, sbyte? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteInt16(YamlWritingUnit writer, string? fieldName, short? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteInt32(YamlWritingUnit writer, string? fieldName, int? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteInt64(YamlWritingUnit writer, string? fieldName, long? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteUInt8(YamlWritingUnit writer, string? fieldName, byte? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteUInt16(YamlWritingUnit writer, string? fieldName, ushort? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteUInt32(YamlWritingUnit writer, string? fieldName, uint? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteUInt64(YamlWritingUnit writer, string? fieldName, ulong? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteFloat(YamlWritingUnit writer, string? fieldName, float? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteModKey(YamlWritingUnit writer, string? fieldName, ModKey? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.Value.FileName);
    }

    public void WriteFormKey(YamlWritingUnit writer, string? fieldName, FormKey? item)
    {
        WriteString(writer, fieldName, item == null ? string.Empty : item.ToString());
    }

    public void WriteRecordType(YamlWritingUnit writer, string? fieldName, RecordType? item)
    {
        writer.WriteName(fieldName);
        
        if (item == null)
        {
            writer.WriteScalar(string.Empty);
        }
        else if (item.Value.TypeInt == RecordType.Null.TypeInt)
        {
            writer.WriteScalar("null");
        }
        else
        {
            writer.WriteScalar(item.Value.Type);
        }
    }

    public void WriteP2Int(YamlWritingUnit writer, string? fieldName, P2Int? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.X}, {item.Value.Y}");
    }

    public void WriteP2Int16(YamlWritingUnit writer, string? fieldName, P2Int16? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.X}, {item.Value.Y}");
    }

    public void WriteP2Float(YamlWritingUnit writer, string? fieldName, P2Float? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.X}, {item.Value.Y}");
    }

    public void WriteP3Float(YamlWritingUnit writer, string? fieldName, P3Float? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
    }

    public void WriteP3UInt8(YamlWritingUnit writer, string? fieldName, P3UInt8? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
    }

    public void WriteP3Int16(YamlWritingUnit writer, string? fieldName, P3Int16? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
    }

    public void WriteP3UInt16(YamlWritingUnit writer, string? fieldName, P3UInt16? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
    }

    public void WritePercent(YamlWritingUnit writer, string? fieldName, Percent? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : item.Value.Value.ToString(CultureInfo.InvariantCulture));
    }

    public void WriteColor(YamlWritingUnit writer, string? fieldName, Color? item)
    {
        WriteString(writer, fieldName, item == null ? "null" : $"{item.Value.R}, {item.Value.G}, {item.Value.B}");
    }

    public void WriteTranslatedString(YamlWritingUnit writer, string? fieldName, ITranslatedStringGetter? item)
    {
        writer.WriteName(fieldName);
        if (item == null || (item.NumLanguages <= 1 && item.String == null))
        {
            writer.Emitter.Emit(new MappingStart());
            writer.Emitter.Emit(new MappingEnd());
            return;
        }
        
        if (item.NumLanguages <= 1)
        {
            writer.WriteScalar(item.String ?? string.Empty);
        }
        else
        {
            writer.Emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Any));
            foreach (var entry in item)
            {
                writer.Emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, false, MappingStyle.Any));
                writer.WriteScalar("Language");
                writer.WriteScalar(entry.Key.ToStringFast());
                writer.WriteScalar("String");
                writer.WriteScalar(entry.Value);
                writer.Emitter.Emit(new MappingEnd());
            }
            writer.Emitter.Emit(new SequenceEnd());
        }
    }

    public void WriteBytes(YamlWritingUnit writer, string? fieldName, ReadOnlyMemorySlice<byte>? item)
    {
        writer.WriteName(fieldName);
        writer.WriteScalar(item == null ? "" : Convert.ToHexString(item.Value));
    }

    public void WriteEnum<TEnum>(YamlWritingUnit writer, string? fieldName, TEnum? item)
        where TEnum : struct, Enum, IConvertible
    {
        writer.WriteName(fieldName);
        if (item == null)
        {
            writer.WriteScalar("");
        }
        else if (!Enums<TEnum>.IsFlagsEnum)
        {
            writer.WriteScalar(item.Value.ToStringFast());
        }
        else
        {
            writer.Emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Any));
            foreach (var flag in item.Value.EnumerateContainedFlags(includeUndefined: true))
            {
                writer.WriteScalar(flag.ToStringFast());
            }
            writer.Emitter.Emit(new SequenceEnd());
        }
    }

    public void WriteWithName<TKernel, TObject>(
        MutagenSerializationWriterKernel<TKernel, YamlWritingUnit> kernel, 
        YamlWritingUnit writer,
        string? fieldName,
        TObject item,
        SerializationMetaData serializationMetaData,
        Write<TKernel, YamlWritingUnit, TObject> writeCall)
        where TKernel : ISerializationWriterKernel<YamlWritingUnit>, new()
    {
        writer.WriteName(fieldName);
        writeCall(writer, item, kernel, serializationMetaData);
    }

    public void WriteLoqui<TKernel, TObject>(
        MutagenSerializationWriterKernel<TKernel, YamlWritingUnit> kernel,
        YamlWritingUnit writer, 
        string? fieldName,
        TObject item,
        SerializationMetaData serializationMetaData,
        Write<TKernel, YamlWritingUnit, TObject> writeCall)
        where TKernel : ISerializationWriterKernel<YamlWritingUnit>, new()
    {
        writer.WriteName(fieldName);
        writer.Emitter.Emit(new MappingStart());
        writeCall(writer, item, kernel, serializationMetaData);
        writer.Emitter.Emit(new MappingEnd());
    }

    public void StartListSection(YamlWritingUnit writer, string? fieldName)
    {
        writer.WriteName(fieldName);
        writer.Emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Any));
    }

    public void EndListSection(YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new SequenceEnd());
    }

    public void StartDictionarySection(YamlWritingUnit writer, string? fieldName)
    {
        writer.WriteName(fieldName);
        writer.Emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Any));
    }

    public void EndDictionarySection(YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new SequenceEnd());
    }

    public void StartDictionaryItem(YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new MappingStart());
    }

    public void EndDictionaryItem(YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new MappingEnd());
    }

    public void StartDictionaryKey(YamlWritingUnit writer)
    {
        writer.WriteScalar("Key");
    }

    public void EndDictionaryKey(YamlWritingUnit writer)
    {
    }

    public void StartDictionaryValue(YamlWritingUnit writer)
    {
        writer.WriteScalar("Value");
    }

    public void EndDictionaryValue(YamlWritingUnit writer)
    {
    }

    public void StartArray2dSection(YamlWritingUnit writer, string? fieldName)
    {
        writer.WriteName(fieldName);
        writer.Emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Block));
    }

    public void EndArray2dSection(YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new SequenceEnd());
    }

    public void StartArray2dXSection(YamlWritingUnit writer)
    {
    }

    public void EndArray2dXSection(YamlWritingUnit writer)
    {
    }

    public void StartArray2dYSection(YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Flow));
    }

    public void EndArray2dYSection(YamlWritingUnit writer)
    {
        writer.Emitter.Emit(new SequenceEnd());
    }
}