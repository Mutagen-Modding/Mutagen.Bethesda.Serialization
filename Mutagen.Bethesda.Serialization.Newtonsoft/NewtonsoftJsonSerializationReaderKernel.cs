using System.Drawing;
using System.Numerics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Newtonsoft.Json;
using Noggog;

namespace Mutagen.Bethesda.Serialization.Newtonsoft;

public class NewtonsoftJsonSerializationReaderKernel : ISerializationReaderKernel<JsonTextReader>
{
    public JsonTextReader GetNewObject(Stream stream)
    {
        var ret = new JsonTextReader(new StreamReader(stream));
        if (!ret.Read() || ret.TokenType != JsonToken.StartObject)
        {
            throw new DataMisalignedException("Did not start with a JSON start object character as expected");
        }

        return ret;
    }

    public bool TryGetNextField(JsonTextReader reader, out string name)
    {
        if (!reader.Read())
        {
            name = default!;
            return false;
        }

        if (reader.TokenType == JsonToken.EndObject)
        {
            name = default!;
            return false;
        }

        if (reader.TokenType == JsonToken.PropertyName
            || reader.ValueType != typeof(string))
        {
            name = reader.Value?.ToString()!;
            reader.Read();
            return name != null;
        }
        
        throw new DataMisalignedException($"Did not have EndObject or PropertyName token as expected. Line: {reader.LineNumber} Pos: {reader.LinePosition}");
    }

    public Type GetNextType(JsonTextReader reader, string namespaceString)
    {
        if (reader.TokenType != JsonToken.StartObject || !reader.Read())
        {
            throw new DataMisalignedException("Did not start with a JSON start object character as expected");
        }

        SkipPropertyName(reader);
        var val = (string)reader.Value!;
        var typeStr = $"{namespaceString}.{val}, {namespaceString}";
        return Type.GetType(typeStr)!;
    }

    public FormKey ExtractFormKey(JsonTextReader reader)
    {
        if (reader.TokenType != JsonToken.StartObject || !reader.Read())
        {
            throw new DataMisalignedException("Did not start with a JSON start object character as expected");
        }
        return ReadFormKey(reader) ?? throw new NullReferenceException("Required FormKey for MajorRecord was null");
    }

    public void Skip(JsonTextReader reader)
    {
        if (reader.TokenType == JsonToken.StartObject)
        {
            SkipObject(reader);
        }
        if (reader.TokenType == JsonToken.StartArray)
        {
            SkipArray(reader);
        }
    }

    private void SkipObject(JsonTextReader reader)
    {
        reader.Read();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                objCount++;
            }
            else if (reader.TokenType == JsonToken.EndObject)
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }

            if (!reader.Read()) break;
        }
    }

    private void SkipPropertyName(JsonTextReader reader)
    {
        if (reader.TokenType == JsonToken.PropertyName)
        {
            if (!reader.Read())
            {
                throw new DataMisalignedException();
            }
        }
    }

    private void SkipArray(JsonTextReader reader)
    {
        reader.Read();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                objCount++;
            }
            else if (reader.TokenType == JsonToken.EndArray)
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }

            if (!reader.Read()) break;
        }
    }

    public char? ReadChar(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        return str[0];
    }

    public bool? ReadBool(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return (bool)reader.Value!;
    }

    public TEnum? ReadEnum<TEnum>(JsonTextReader reader) where TEnum : struct, Enum, IConvertible
    {
        if (Enums<TEnum>.IsFlagsEnum)
        {
            TEnum ret = default;

            SkipPropertyName(reader);
            if (reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        break;
                    }
                    var str = (string)reader.Value!;
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
            if (reader.Value is null or "") return null;
            throw new ArgumentException($"Could not convert to {typeof(TEnum)}: {reader.Value}");
        }
        else
        {
            SkipPropertyName(reader);
            if (reader.Value is null or "") return null;
            var str = (string)reader.Value!;
            return Enum.Parse<TEnum>(str);
        }
    }

    public string? ReadString(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        return (string?)reader.Value!;
    }

    public sbyte? ReadInt8(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return checked((sbyte)(long)reader.Value!);
    }

    public short? ReadInt16(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return checked((short)(long)reader.Value!);
    }

    public int? ReadInt32(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return checked((int)(long)reader.Value!);
    }

    public long? ReadInt64(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return (long)reader.Value!;
    }

    public byte? ReadUInt8(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return checked((byte)(long)reader.Value!);
    }

    public ushort? ReadUInt16(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return checked((ushort)(long)reader.Value!);
    }

    public uint? ReadUInt32(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return checked((uint)(long)reader.Value!);
    }

    public ulong? ReadUInt64(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        if (reader.ValueType == typeof(long))
        {
            return checked((ulong)(long)reader.Value!);
        }

        if (reader.ValueType == typeof(BigInteger))
        {
            var bigInt = (BigInteger)reader.Value!;
            return ulong.Parse(bigInt.ToString());
        }

        throw new NotImplementedException();
    }

    public float? ReadFloat(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return (float)(double)reader.Value!;
    }

    public ModKey? ReadModKey(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return ModKey.FromNameAndExtension((string)reader.Value!);
    }

    public FormKey? ReadFormKey(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        return FormKey.Factory((string)reader.Value!);
    }

    public Color? ReadColor(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        return ColorExt.ConvertFromCommaString(str.AsSpan());
    }

    public RecordType? ReadRecordType(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (str == "null")
        {
            return RecordType.Null;
        }
        return new RecordType(str);
    }

    public P2Int? ReadP2Int(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P2Int.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int)}: {str}");
    }

    public P2Int16? ReadP2Int16(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P2Int16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int16)}: {str}");
    }

    public P2Float? ReadP2Float(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P2Float.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Float)}: {str}");
    }

    public P3Float? ReadP3Float(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3Float.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Float)}: {str}");
    }

    public P3UInt8? ReadP3UInt8(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3UInt8.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt8)}: {str}");
    }

    public P3Int16? ReadP3Int16(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3Int16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Int16)}: {str}");
    }

    public P3UInt16? ReadP3UInt16(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (P3UInt16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt16)}: {str}");
    }

    public Percent? ReadPercent(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        if (reader.Value is null or "") return null;
        var d = (double)reader.Value!;
        return new Percent(d);
    }

    public TranslatedString? ReadTranslatedString(JsonTextReader reader)
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
        
        while (!stop && reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    language = null;
                    str = null;
                    objCount++;
                    break;
                case JsonToken.PropertyName:
                    var propName = (string?)reader.Value;
                    if (inArray)
                    {
                        if (propName == "Language")
                        {
                            language = Enum.Parse<Language>(reader.ReadAsString()!);
                        }
                        else if (propName == "String")
                        {
                            str = reader.ReadAsString();
                        }
                    }
                    else if (propName == "TargetLanguage")
                    {
                        targetLanguage = Enum.Parse<Language>(reader.ReadAsString()!);
                    }
                    else if (propName == "Value")
                    {
                        mainString = reader.ReadAsString()!;
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

    public MemorySlice<byte>? ReadBytes(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        var str = (string?)reader.Value!;
        if (str.IsNullOrEmpty()) return null;
        if (str == "[]") return Array.Empty<byte>();
        return Convert.FromHexString(str);
    }

    public TObject ReadLoqui<TObject>(
        JsonTextReader reader, 
        SerializationMetaData serializationMetaData,
        Read<ISerializationReaderKernel<JsonTextReader>, JsonTextReader, TObject> readCall)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new DataMisalignedException();
        }

        var ret = readCall(reader, this, serializationMetaData);
        
        if (reader.TokenType != JsonToken.EndObject)
        {
            throw new DataMisalignedException();
        }

        return ret;
    }

    public void StartListSection(JsonTextReader reader)
    {
        SkipPropertyName(reader);
    }

    public void EndListSection(JsonTextReader reader)
    {
    }

    public bool TryHasNextItem(JsonTextReader reader)
    {
        if (!reader.Read())
        {
            throw new DataMisalignedException();
        }
        return reader.TokenType != JsonToken.EndArray;
    }

    public void StartDictionarySection(JsonTextReader reader)
    {
        SkipPropertyName(reader);

        if (reader.TokenType != JsonToken.StartArray
            || !reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionarySection(JsonTextReader reader)
    {
    }

    public bool TryHasNextDictionaryItem(JsonTextReader reader)
    {
        if (reader.TokenType == JsonToken.EndArray)
        {
            return false;
        }

        reader.Read();
        return true;
    }

    public void StartDictionaryKey(JsonTextReader reader)
    {
        if (reader.TokenType != JsonToken.PropertyName
            || reader.ValueType != typeof(string)
            || reader.Value?.ToString() != "Key")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryKey(JsonTextReader reader)
    {
        reader.Read();
    }

    public void StartDictionaryValue(JsonTextReader reader)
    {
        if (reader.TokenType != JsonToken.PropertyName
            || reader.ValueType != typeof(string)
            || reader.Value?.ToString() != "Value")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryValue(JsonTextReader reader)
    {
        reader.Read();
    }

    public void EndDictionaryItem(JsonTextReader reader)
    {
        reader.Read();
    }

    public void StartArray2dSection(JsonTextReader reader)
    {
        SkipPropertyName(reader);
        
        if (reader.TokenType != JsonToken.StartArray
            || !reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public void EndArray2dSection(JsonTextReader reader)
    {
        if (reader.TokenType != JsonToken.EndArray
            || !reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public bool TryHasNextArray2dXItem(JsonTextReader reader)
    {
        return reader.TokenType != JsonToken.EndArray;
    }

    public void StartArray2dXSection(JsonTextReader reader)
    {
    }

    public void EndArray2dXSection(JsonTextReader reader)
    {
        if (!reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public bool TryHasNextArray2dYSection(JsonTextReader reader)
    {
        return reader.TokenType == JsonToken.StartArray;
    }

    public void StartArray2dYSection(JsonTextReader reader)
    {
        if (reader.TokenType != JsonToken.StartArray
            || !reader.Read())
        {
            throw new DataMisalignedException();
        }
    }

    public void EndArray2dYSection(JsonTextReader reader)
    {
        if (reader.TokenType != JsonToken.EndArray
            || !reader.Read())
        {
            throw new DataMisalignedException();
        }
    }
}