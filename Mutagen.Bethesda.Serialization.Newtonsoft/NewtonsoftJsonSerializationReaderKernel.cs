using System.Drawing;
using System.Globalization;
using System.Numerics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Strings;
using Newtonsoft.Json;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class JsonReadingUnit : IDisposable, IContainStreamPackage
{
    private readonly IDisposable _streamDispose;
    public JsonTextReader Reader { get; }
    public StreamPackage StreamPackage { get; }

    public JsonReadingUnit(StreamPackage stream)
    {
        var sw = new StreamReader(stream.Stream, leaveOpen: true);
        _streamDispose = sw;
        Reader = new JsonTextReader(sw);
        if (!Reader.Read() || Reader.TokenType != JsonToken.StartObject)
        {
            throw new DataMisalignedException("Did not start with a JSON start object character as expected");
        }

        StreamPackage = stream;
    }

    public void Dispose()
    {
        _streamDispose.Dispose();
    }
}

public class NewtonsoftJsonSerializationReaderKernel : ISerializationReaderKernel<JsonReadingUnit>
{
    public string ExpectedExtension => ".json";

    public JsonReadingUnit GetNewObject(StreamPackage stream)
    {
        return new JsonReadingUnit(stream);
    }

    public bool TryGetNextField(JsonReadingUnit reader, out string name)
    {
        if (!reader.Reader.Read())
        {
            name = default!;
            return false;
        }

        if (reader.Reader.TokenType == JsonToken.EndObject)
        {
            name = default!;
            return false;
        }

        if (reader.Reader.TokenType == JsonToken.PropertyName
            || reader.Reader.ValueType != typeof(string))
        {
            name = reader.Reader.Value?.ToString()!;
            reader.Reader.Read();
            return name != null;
        }
        
        throw new DataMisalignedException($"Did not have EndObject or PropertyName token as expected. Line: {reader.Reader.LineNumber} Pos: {reader.Reader.LinePosition}");
    }

    public Type GetNextType(JsonReadingUnit reader, string namespaceString)
    {
        if (reader.Reader.TokenType != JsonToken.StartObject || !reader.Reader.Read())
        {
            throw new DataMisalignedException("Did not start with a JSON start object character as expected");
        }

        SkipPropertyName(reader);
        return TypeHelper.GetTypeFromString((string)reader.Reader.Value!, namespaceString);
    }

    public FormKey ExtractFormKey(JsonReadingUnit reader)
    {
        if (!reader.Reader.Read())
        {
            throw new DataMisalignedException("Did not start with a JSON start object character as expected");
        }
        return ReadFormKey(reader) ?? throw new NullReferenceException("Required FormKey for MajorRecord was null");
    }

    public void Skip(JsonReadingUnit reader)
    {
        if (reader.Reader.TokenType == JsonToken.StartObject)
        {
            SkipObject(reader);
        }
        if (reader.Reader.TokenType == JsonToken.StartArray)
        {
            SkipArray(reader);
        }
    }

    private void SkipObject(JsonReadingUnit reader)
    {
        reader.Reader.Read();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.Reader.TokenType == JsonToken.StartObject)
            {
                objCount++;
            }
            else if (reader.Reader.TokenType == JsonToken.EndObject)
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }

            if (!reader.Reader.Read()) break;
        }
    }

    private void SkipPropertyName(JsonReadingUnit reader)
    {
        if (reader.Reader.TokenType == JsonToken.PropertyName)
        {
            if (!reader.Reader.Read())
            {
                throw new DataMisalignedException();
            }
        }
    }

    private void SkipArray(JsonReadingUnit reader)
    {
        reader.Reader.Read();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.Reader.TokenType == JsonToken.StartArray)
            {
                objCount++;
            }
            else if (reader.Reader.TokenType == JsonToken.EndArray)
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }

            if (!reader.Reader.Read()) break;
        }
    }

    public char? ReadChar(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        return str[0];
    }

    public bool? ReadBool(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return (bool)reader.Reader.Value!;
    }

    public TEnum? ReadEnum<TEnum>(JsonReadingUnit reader) where TEnum : struct, Enum, IConvertible
    {
        if (Enums<TEnum>.IsFlagsEnum)
        {
            TEnum ret = default;

            SkipPropertyName(reader);
            if (reader.Reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Reader.Read())
                {
                    if (reader.Reader.TokenType == JsonToken.EndArray)
                    {
                        break;
                    }
                    var str = (string)reader.Reader.Value!;
                    var strSpan = str.AsSpan();
                    if (Enum.TryParse<TEnum>(strSpan, out var otherEnum))
                    {
                        ret = Enums<TEnum>.Or(ret, otherEnum);
                        continue;
                    }

                    if (Enums<TEnum>.TryParseFromNumber(strSpan, out otherEnum))
                    {
                        ret = Enums<TEnum>.Or(ret, otherEnum);
                        continue;
                    }
                    throw new ArgumentException($"Could not convert to {typeof(TEnum)}: {str}");
                }

                return ret;
            }
            if (reader.Reader.Value is null or "") return null;
            throw new ArgumentException($"Could not convert to {typeof(TEnum)}: {reader.Reader.Value}");
        }
        else
        {
            SkipPropertyName(reader);
            if (reader.Reader.Value is null or "") return null;
            var str = (string)reader.Reader.Value!;
            return Enum.Parse<TEnum>(str);
        }
    }

    public string? ReadString(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.TokenType == JsonToken.StartObject)
        {
            reader.Reader.Read();
            if (reader.Reader.TokenType != JsonToken.EndObject)
            {
                throw new DataMisalignedException("String with object start did not follow with object end.");
            }

            return null;
        }
        return (string?)reader.Reader.Value!;
    }

    public sbyte? ReadInt8(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return checked((sbyte)(long)reader.Reader.Value!);
    }

    public short? ReadInt16(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return checked((short)(long)reader.Reader.Value!);
    }

    public int? ReadInt32(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return checked((int)(long)reader.Reader.Value!);
    }

    public long? ReadInt64(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return (long)reader.Reader.Value!;
    }

    public byte? ReadUInt8(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return checked((byte)(long)reader.Reader.Value!);
    }

    public ushort? ReadUInt16(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return checked((ushort)(long)reader.Reader.Value!);
    }

    public uint? ReadUInt32(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return checked((uint)(long)reader.Reader.Value!);
    }

    public ulong? ReadUInt64(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        if (reader.Reader.ValueType == typeof(long))
        {
            return checked((ulong)(long)reader.Reader.Value!);
        }

        if (reader.Reader.ValueType == typeof(BigInteger))
        {
            var bigInt = (BigInteger)reader.Reader.Value!;
            return ulong.Parse(bigInt.ToString());
        }

        throw new NotImplementedException();
    }

    public float? ReadFloat(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return (float)(double)reader.Reader.Value!;
    }

    public double? ReadDouble(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return (double)reader.Reader.Value!;
    }

    public ModKey? ReadModKey(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return ModKey.FromNameAndExtension((string)reader.Reader.Value!);
    }

    public FormKey? ReadFormKey(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        return FormKey.Factory((string)reader.Reader.Value!);
    }

    public Color? ReadColor(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        return ColorExt.FromHexString(str);
    }

    public RecordType? ReadRecordType(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (str == "null")
        {
            return RecordType.Null;
        }
        return new RecordType(str);
    }

    public P2Int? ReadP2Int(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P2Int.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int)}: {str}");
    }

    public P2Int16? ReadP2Int16(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P2Int16.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int16)}: {str}");
    }

    public P2UInt8? ReadP2UInt8(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P2UInt8.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2UInt8)}: {str}");
    }

    public P2Float? ReadP2Float(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P2Float.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Float)}: {str}");
    }

    public P3Float? ReadP3Float(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3Float.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Float)}: {str}");
    }

    public P3UInt8? ReadP3UInt8(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3UInt8.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt8)}: {str}");
    }

    public P3Int16? ReadP3Int16(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3Int16.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Int16)}: {str}");
    }

    public P3UInt16? ReadP3UInt16(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3UInt16.TryParse(str, out var pt, TargetCulture.English))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt16)}: {str}");
    }

    public Percent? ReadPercent(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        if (reader.Reader.Value is null or "") return null;
        var d = (double)reader.Reader.Value!;
        return new Percent(d);
    }

    public TimeOnly? ReadTimeOnly(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        return TimeOnly.Parse(str);
    }

    public DateOnly? ReadDateOnly(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        return DateOnly.Parse(str);
    }

    public Guid? ReadGuid(JsonReadingUnit reader)
    {
        var str = ReadString(reader);
        return str == null ? null : new Guid(str);
    }

    public TranslatedString? ReadTranslatedString(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        
        Language? targetLanguage = null;
        string? mainString = null;
        List<KeyValuePair<Language, string?>>? pairs = null;
        Language? language = null;
        string? str = null;
        int objCount = 1;
        bool stop = false;
        bool inArray = false;
        bool startingValues = false;
        
        while (!stop && reader.Reader.Read())
        {
            switch (reader.Reader.TokenType)
            {
                case JsonToken.StartObject:
                    language = null;
                    str = null;
                    objCount++;
                    break;
                case JsonToken.PropertyName:
                    var propName = (string?)reader.Reader.Value;
                    if (inArray)
                    {
                        if (propName == "Language")
                        {
                            language = Enum.Parse<Language>(reader.Reader.ReadAsString()!);
                        }
                        else if (propName == "String")
                        {
                            str = reader.Reader.ReadAsString();
                        }
                    }
                    else if (propName == "TargetLanguage")
                    {
                        targetLanguage = Enum.Parse<Language>(reader.Reader.ReadAsString()!);
                    }
                    else if (propName == "Value")
                    {
                        mainString = reader.Reader.ReadAsString()!;
                    }
                    else if (propName == "Values")
                    {
                        startingValues = true;
                    }
                    break;
                case JsonToken.EndObject:
                    if (language != null)
                    {
                        pairs ??= new();
                        pairs.Add(new(language.Value, str));
                    }

                    language = null;
                    str = null;
                    objCount--;
                    if (objCount == 0)
                    {
                        stop = true;
                    }
                    break;
                case JsonToken.StartArray:
                    if (!startingValues)
                    {
                        throw new DataMisalignedException("Started an array without being inside the values section");
                    }
                    inArray = true;
                    break;
                case JsonToken.EndArray:
                    startingValues = false;
                    inArray = false;
                    break;
            }
        }

        if (targetLanguage == null && pairs == null && mainString == null)
        {
            return null;
        }

        var ret = new TranslatedString(targetLanguage ?? TranslatedString.DefaultLanguage);
        if (pairs != null)
        {
            foreach (var p in pairs)
            {
                ret.Set(p.Key, p.Value);
            }
        }
        if (mainString != null)
        {
            ret.String = mainString;
        }

        return ret;
    }

    public MemorySlice<byte>? ReadBytes(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (str == "[]") return Array.Empty<byte>();
        var span = str.AsSpan();
        if (span.StartsWith("0x"))
        {
            span = span[2..];
        }
        return Convert.FromHexString(span);
    }

    public async Task<TObject?> ReadLoqui<TObject>(
        JsonReadingUnit reader, 
        SerializationMetaData serializationMetaData,
        ReadAsync<ISerializationReaderKernel<JsonReadingUnit>, JsonReadingUnit, TObject> readCall)
    {
        SkipPropertyName(reader);
        if (reader.Reader.TokenType != JsonToken.StartObject)
        {
            throw new DataMisalignedException();
        }

        var ret = await readCall(reader, this, serializationMetaData);
        
        if (reader.Reader.TokenType != JsonToken.EndObject)
        {
            throw new DataMisalignedException();
        }

        return ret;
    }

    public void StartListSection(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
    }

    public void EndListSection(JsonReadingUnit reader)
    {
    }

    public bool TryHasNextItem(JsonReadingUnit reader)
    {
        if (!reader.Reader.Read())
        {
            throw new DataMisalignedException();
        }
        return reader.Reader.TokenType != JsonToken.EndArray;
    }

    public void StartDictionarySection(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);

        if (reader.Reader.TokenType != JsonToken.StartArray
            || !reader.Reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionarySection(JsonReadingUnit reader)
    {
    }

    public bool TryHasNextDictionaryItem(JsonReadingUnit reader)
    {
        if (reader.Reader.TokenType == JsonToken.EndArray)
        {
            return false;
        }

        reader.Reader.Read();
        return true;
    }

    public void StartDictionaryKey(JsonReadingUnit reader)
    {
        if (reader.Reader.TokenType != JsonToken.PropertyName
            || reader.Reader.ValueType != typeof(string)
            || reader.Reader.Value?.ToString() != "Key")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryKey(JsonReadingUnit reader)
    {
        reader.Reader.Read();
    }

    public void StartDictionaryValue(JsonReadingUnit reader)
    {
        if (reader.Reader.TokenType != JsonToken.PropertyName
            || reader.Reader.ValueType != typeof(string)
            || reader.Reader.Value?.ToString() != "Value")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryValue(JsonReadingUnit reader)
    {
        reader.Reader.Read();
    }

    public void EndDictionaryItem(JsonReadingUnit reader)
    {
        reader.Reader.Read();
    }

    public void StartArray2dSection(JsonReadingUnit reader)
    {
        SkipPropertyName(reader);
        
        if (reader.Reader.TokenType != JsonToken.StartArray
            || !reader.Reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public void EndArray2dSection(JsonReadingUnit reader)
    {
    }

    public bool TryHasNextArray2dXItem(JsonReadingUnit reader)
    {
        return reader.Reader.TokenType != JsonToken.EndArray;
    }

    public void StartArray2dXItem(JsonReadingUnit reader)
    {
    }

    public void EndArray2dXItem(JsonReadingUnit reader)
    {
        if (!reader.Reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public bool TryHasNextArray2dYSection(JsonReadingUnit reader)
    {
        return reader.Reader.TokenType == JsonToken.StartArray;
    }

    public void StartArray2dYSection(JsonReadingUnit reader)
    {
        if (reader.Reader.TokenType != JsonToken.StartArray
            || !reader.Reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public void EndArray2dYSection(JsonReadingUnit reader)
    {
        if (reader.Reader.TokenType != JsonToken.EndArray
            || !reader.Reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public Exception ConvertException(Exception ex)
    {
        return ex;
    }
}