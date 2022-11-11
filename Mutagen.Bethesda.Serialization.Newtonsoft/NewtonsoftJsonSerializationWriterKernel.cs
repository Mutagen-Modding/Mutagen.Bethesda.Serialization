using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Newtonsoft.Json;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class JsonWritingUnit : IDisposable
{
    private IDisposable _streamDispose;
    public JsonTextWriter Writer { get; }
    
    public JsonWritingUnit(
        Stream stream)
    {
        var sw = new StreamWriter(stream, leaveOpen: true);
        _streamDispose = sw;
        Writer = new JsonTextWriter(sw)
        {
            Formatting = Formatting.Indented
        };
    }

    public void Dispose()
    {
        _streamDispose.Dispose();
    }

    public void WriteName(string? fieldName)
    {
        if (fieldName == null)
        {
            return;
        }
        Writer.WritePropertyName(fieldName);
    }
}

public class NewtonsoftJsonSerializationWriterKernel : ISerializationWriterKernel<JsonWritingUnit>
{
    public JsonWritingUnit GetNewObject(Stream stream)
    {
        var ret = new JsonWritingUnit(stream);
        ret.Writer.WriteStartObject();
        return ret;
    }

    public void Finalize(Stream stream, JsonWritingUnit writer)
    {
        writer.Writer.WriteEndObject();
        writer.Dispose();
    }

    public void WriteType(JsonWritingUnit writer, Type type)
    {
        WriteString(writer, "MutagenObjectType", type.Name);
    }

    public void WriteChar(JsonWritingUnit writer, string? fieldName, char? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteBool(JsonWritingUnit writer, string? fieldName, bool? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteString(JsonWritingUnit writer, string? fieldName, string? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteStartObject();
            writer.Writer.WriteEndObject();
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteInt8(JsonWritingUnit writer, string? fieldName, sbyte? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteInt16(JsonWritingUnit writer, string? fieldName, short? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteInt32(JsonWritingUnit writer, string? fieldName, int? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteInt64(JsonWritingUnit writer, string? fieldName, long? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteUInt8(JsonWritingUnit writer, string? fieldName, byte? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteUInt16(JsonWritingUnit writer, string? fieldName, ushort? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteUInt32(JsonWritingUnit writer, string? fieldName, uint? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteUInt64(JsonWritingUnit writer, string? fieldName, ulong? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteFloat(JsonWritingUnit writer, string? fieldName, float? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item);
        }
    }

    public void WriteModKey(JsonWritingUnit writer, string? fieldName, ModKey? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item.Value.FileName);
        }
    }

    public void WriteFormKey(JsonWritingUnit writer, string? fieldName, FormKey? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item.ToString());
        }
    }

    public void WriteRecordType(JsonWritingUnit writer, string? fieldName, RecordType? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else if (item.Value.TypeInt == RecordType.Null.TypeInt)
        {
            writer.Writer.WriteValue("null");
        }
        else
        {
            writer.Writer.WriteValue(item.Value.Type);
        }
    }

    public void WriteP2Int(JsonWritingUnit writer, string? fieldName, P2Int? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue($"{item.Value.X}, {item.Value.Y}");
        }
    }

    public void WriteP2Int16(JsonWritingUnit writer, string? fieldName, P2Int16? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue($"{item.Value.X}, {item.Value.Y}");
        }
    }

    public void WriteP2Float(JsonWritingUnit writer, string? fieldName, P2Float? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue($"{item.Value.X}, {item.Value.Y}");
        }
    }

    public void WriteP3Float(JsonWritingUnit writer, string? fieldName, P3Float? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue($"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
        }
    }

    public void WriteP3UInt8(JsonWritingUnit writer, string? fieldName, P3UInt8? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue($"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
        }
    }

    public void WriteP3Int16(JsonWritingUnit writer, string? fieldName, P3Int16? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue($"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
        }
    }

    public void WriteP3UInt16(JsonWritingUnit writer, string? fieldName, P3UInt16? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue($"{item.Value.X}, {item.Value.Y}, {item.Value.Z}");
        }
    }

    public void WritePercent(JsonWritingUnit writer, string? fieldName, Percent? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item.Value);
        }
    }

    public void WriteColor(JsonWritingUnit writer, string? fieldName, Color? item)
    {
        writer.WriteName(fieldName);
        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item.Value.CommaString(alpha: ColorExt.IncludeAlpha.Never));
        }
    }

    public void WriteTranslatedString(JsonWritingUnit writer, string? fieldName, ITranslatedStringGetter? item)
    {
        writer.WriteName(fieldName);

        if (item == null)
        {
            writer.Writer.WriteStartObject();
            writer.Writer.WriteEndObject();
            return;
        }
        
        writer.Writer.WriteStartObject();
        writer.Writer.WritePropertyName("TargetLanguage");
        writer.Writer.WriteValue(item.TargetLanguage.ToStringFast());

        if (item.NumLanguages <= 1)
        {
            if (item.String is { } str)
            {
                writer.Writer.WritePropertyName("Value");
                writer.Writer.WriteValue($"{str}");
            }
        }
        else
        {
            writer.Writer.WritePropertyName("Values");
            writer.Writer.WriteStartArray();
            foreach (var entry in item)
            {
                writer.Writer.WriteStartObject();
                writer.Writer.WritePropertyName("Language");
                writer.Writer.WriteValue(entry.Key.ToStringFast());
                writer.Writer.WritePropertyName("String");
                writer.Writer.WriteValue(entry.Value);
                writer.Writer.WriteEndObject();
            }
            writer.Writer.WriteEndArray();
        }
        writer.Writer.WriteEndObject();
    }

    public void WriteBytes(JsonWritingUnit writer, string? fieldName, ReadOnlyMemorySlice<byte>? item)
    {
        writer.WriteName(fieldName);
        if (item == null)
        {
            writer.Writer.WriteValue(string.Empty);
        }
        else
        {
            writer.Writer.WriteValue(item.Value.Length == 0 ? "[]" : Convert.ToHexString(item.Value));
        }
    }

    public void WriteEnum<TEnum>(JsonWritingUnit writer, string? fieldName, TEnum? item)
        where TEnum : struct, Enum, IConvertible
    {
        writer.WriteName(fieldName);
        if (item == null)
        {
            writer.Writer.WriteValue("");
        }
        else if (!Enums<TEnum>.IsFlagsEnum)
        {
            writer.Writer.WriteValue(item.Value.ToStringFast());
        }
        else
        {
            writer.Writer.WriteStartArray();
            foreach (var flag in item.Value.EnumerateContainedFlags(includeUndefined: true))
            {
                writer.Writer.WriteValue(flag.ToStringFast());
            }
            writer.Writer.WriteEndArray();
        }
    }

    public void WriteWithName<TKernel, TObject>(
        MutagenSerializationWriterKernel<TKernel, JsonWritingUnit> kernel, 
        JsonWritingUnit writer,
        string? fieldName,
        TObject item, 
        SerializationMetaData serializationMetaData,
        Write<TKernel, JsonWritingUnit, TObject> writeCall) 
        where TKernel : ISerializationWriterKernel<JsonWritingUnit>, new()
    {
        writer.WriteName(fieldName);
        writeCall(writer, item, kernel, serializationMetaData);
    }

    public void WriteLoqui<TKernel, TObject>(
        MutagenSerializationWriterKernel<TKernel, JsonWritingUnit> kernel,
        JsonWritingUnit writer, 
        string? fieldName,
        TObject item,
        SerializationMetaData serializationMetaData,
        Write<TKernel, JsonWritingUnit, TObject> writeCall) 
        where TKernel : ISerializationWriterKernel<JsonWritingUnit>, new()
    {
        writer.WriteName(fieldName);
        writer.Writer.WriteStartObject();
        writeCall(writer, item, kernel, serializationMetaData);
        writer.Writer.WriteEndObject();
    }

    public void StartListSection(JsonWritingUnit writer, string? fieldName)
    {
        writer.WriteName(fieldName);
        writer.Writer.WriteStartArray();
    }

    public void EndListSection(JsonWritingUnit writer)
    {
        writer.Writer.WriteEndArray();
    }

    public void StartDictionarySection(JsonWritingUnit writer, string? fieldName)
    {
        writer.WriteName(fieldName);
        writer.Writer.WriteStartArray();
    }

    public void EndDictionarySection(JsonWritingUnit writer)
    {
        writer.Writer.WriteEndArray();
    }

    public void StartDictionaryItem(JsonWritingUnit writer)
    {
        writer.Writer.WriteStartObject();
    }

    public void EndDictionaryItem(JsonWritingUnit writer)
    {
        writer.Writer.WriteEndObject();
    }

    public void StartDictionaryKey(JsonWritingUnit writer)
    {
        writer.WriteName("Key");
    }

    public void EndDictionaryKey(JsonWritingUnit writer)
    {
    }

    public void StartDictionaryValue(JsonWritingUnit writer)
    {
        writer.WriteName("Value");
    }

    public void EndDictionaryValue(JsonWritingUnit writer)
    {
    }

    public void StartArray2dSection(JsonWritingUnit writer, string? fieldName)
    {
        writer.WriteName(fieldName);
        writer.Writer.WriteStartArray();
    }

    public void EndArray2dSection(JsonWritingUnit writer)
    {
        writer.Writer.WriteEndArray();
    }

    public void StartArray2dXSection(JsonWritingUnit writer)
    {
    }

    public void EndArray2dXSection(JsonWritingUnit writer)
    {
    }

    public void StartArray2dYSection(JsonWritingUnit writer)
    {
        writer.Writer.WriteStartArray();
        writer.Writer.Formatting = Formatting.None;
    }

    public void EndArray2dYSection(JsonWritingUnit writer)
    {
        writer.Writer.WriteEndArray();
        writer.Writer.Formatting = Formatting.Indented;
    }
}