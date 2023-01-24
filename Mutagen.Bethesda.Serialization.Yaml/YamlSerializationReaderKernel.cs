using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using DocumentStart = YamlDotNet.Core.Events.DocumentStart;
using Scalar = YamlDotNet.Core.Events.Scalar;
using StreamStart = YamlDotNet.Core.Events.StreamStart;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationReaderKernel : ISerializationReaderKernel<Parser>
{
    public Parser GetNewObject(Stream stream)
    {
        var reader = new Parser(new StreamReader(stream));

        reader.Consume<StreamStart>();
        reader.Consume<DocumentStart>();
        reader.Consume<MappingStart>();
        
        return reader;
    }

    public bool TryGetNextField(Parser reader, out string name)
    {
        reader.TryConsume<MappingStart>(out _);

        if (reader.Current is MappingEnd)
        {
            name = default!;
            return false;
        }
        
        if (!reader.TryConsume<Scalar>(out var scalar))
        {
            name = default!;
            return false;
        }

        name = scalar.Value;
        return true;
    }

    public Type GetNextType(Parser reader, string namespaceString)
    {
        reader.Consume<MappingStart>();
        reader.Consume<Scalar>();
        
        var scalar = reader.Consume<Scalar>();
        var val = scalar.Value;
        var typeStr = $"{namespaceString}.{val}, {namespaceString}";
        return Type.GetType(typeStr)!;
    }

    public FormKey ExtractFormKey(Parser reader)
    {
        if (!TryGetNextField(reader, out var name)
            && name != "FormKey")
        {
            throw new NullReferenceException("Required FormKey for MajorRecord was not first");
        }
        
        return ReadFormKey(reader) ?? throw new NullReferenceException("Required FormKey for MajorRecord was null");
    }

    public void Skip(Parser reader)
    {
        if (reader.Current is MappingStart)
        {
            SkipObject(reader);
        }
        else if (reader.Current is SequenceStart)
        {
            SkipArray(reader);
        }
        else
        {
            reader.MoveNext();
        }
    }

    private void SkipObject(Parser reader)
    {
        reader.Consume<MappingStart>();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.TryConsume<MappingStart>(out _))
            {
                objCount++;
            }
            else if (reader.TryConsume<MappingEnd>(out _))
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }
            else if (!reader.MoveNext())
            {
                break;
            }
        }
    }

    private void SkipArray(Parser reader)
    {
        reader.Consume<SequenceStart>();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.TryConsume<SequenceStart>(out _))
            {
                objCount++;
            }
            else if (reader.TryConsume<SequenceEnd>(out _))
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }

            if (!reader.MoveNext()) break;
        }
    }

    public char? ReadChar(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return str[0];
    }

    public bool? ReadBool(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return bool.Parse(str);
    }

    public TEnum? ReadEnum<TEnum>(Parser reader)
        where TEnum : struct, Enum, IConvertible
    {
        if (Enums<TEnum>.IsFlagsEnum)
        {
            if (reader.TryConsume<SequenceStart>(out _))
            {
                TEnum ret = default;
                while (reader.TryConsume<Scalar>(out var e))
                {
                    var strSpan = e.Value.AsSpan();
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
                    throw new ArgumentException($"Could not convert to {typeof(TEnum)}: {e.Value}");
                }

                reader.Consume<SequenceEnd>();
                return ret;
            }
            var str = ReadString(reader);
            if (str.IsNullOrWhitespace()) return null;
            throw new ArgumentException($"Could not convert to {typeof(TEnum)}: {str}");
        }
        else
        {
            var str = ReadString(reader);
            if (str.IsNullOrWhitespace()) return null;
            return Enum.Parse<TEnum>(str);
        }
    }

    public string? ReadString(Parser reader)
    {
        if (reader.TryConsume<MappingStart>(out _))
        {
            reader.Consume<MappingEnd>();
            return null;
        }
        var scalar = reader.Consume<Scalar>();
        return scalar.Value;
    }

    public sbyte? ReadInt8(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return sbyte.Parse(str);
    }

    public short? ReadInt16(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return short.Parse(str);
    }

    public int? ReadInt32(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return int.Parse(str);
    }

    public long? ReadInt64(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return long.Parse(str);
    }

    public byte? ReadUInt8(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return byte.Parse(str);
    }

    public ushort? ReadUInt16(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ushort.Parse(str);
    }

    public uint? ReadUInt32(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return uint.Parse(str);
    }

    public ulong? ReadUInt64(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ulong.Parse(str);
    }

    public float? ReadFloat(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return float.Parse(str);
    }

    public ModKey? ReadModKey(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ModKey.FromNameAndExtension(str);
    }

    public FormKey? ReadFormKey(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return FormKey.Factory(str);
    }

    public Color? ReadColor(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ColorExt.FromHexString(str);
    }

    public RecordType? ReadRecordType(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (str == "null")
        {
            return RecordType.Null;
        }
        return new RecordType(str);
    }

    public P2Int? ReadP2Int(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P2Int.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int)}: {str}");
    }

    public P2Int16? ReadP2Int16(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P2Int16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int16)}: {str}");
    }

    public P2Float? ReadP2Float(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P2Float.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Float)}: {str}");
    }

    public P3Float? ReadP3Float(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3Float.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Float)}: {str}");
    }

    public P3UInt8? ReadP3UInt8(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3UInt8.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt8)}: {str}");
    }

    public P3Int16? ReadP3Int16(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3Int16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Int16)}: {str}");
    }

    public P3UInt16? ReadP3UInt16(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3UInt16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt16)}: {str}");
    }

    public Percent? ReadPercent(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (double.TryParse(str, out var d))
        {
            return new Percent(d);
        }

        throw new ArgumentException($"Could not parse string into {nameof(Percent)}: {str}");
    }

    public TranslatedString? ReadTranslatedString(Parser reader)
    {
        Language? targetLanguage = null;
        string? mainString = null;
        List<KeyValuePair<Language, string?>>? pairs = null;
        Language? language = null;
        string? str = null;
        int objCount = 1;
        bool stop = false;
        bool inArray = false;

        reader.Consume<MappingStart>();
        
        while (!stop)
        {
            switch (reader.Current)
            {
                case MappingStart:
                    language = null;
                    str = null;
                    objCount++;
                    break;
                case Scalar s:
                    var propName = s.Value;
                    reader.Consume<Scalar>();
                    if (inArray)
                    {
                        if (propName == "Language"
                            && reader.Accept<Scalar>(out var lang))
                        {
                            language = Enum.Parse<Language>(lang.Value);
                        }
                        else if (propName == "String"
                                 && reader.Accept<Scalar>(out var strVal))
                        {
                            str = strVal.Value;
                        }
                    }
                    else if (propName == "TargetLanguage"
                             && reader.Accept<Scalar>(out var lang))
                    {
                        targetLanguage = Enum.Parse<Language>(lang.Value);
                    }
                    else if (propName == "Value"
                             && reader.Accept<Scalar>(out var val))
                    {
                        mainString = val.Value;
                    }
                    else if (propName == "Values"
                             && reader.Accept<SequenceStart>(out _))
                    {
                        inArray = true;
                    }
                    break;
                case MappingEnd:
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
                case SequenceEnd:
                    inArray = false;
                    break;
                default:
                    throw new DataMisalignedException();
            }

            reader.MoveNext();
        }

        if (pairs == null && mainString == null && targetLanguage == null) return null;

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

    public MemorySlice<byte>? ReadBytes(Parser reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (str == "[]") return Array.Empty<byte>();
        var span = str.AsSpan();
        if (span.StartsWith("0x"))
        {
            span = span[2..];
        }
        return Convert.FromHexString(span);
    }

    public TObject ReadLoqui<TObject>(
        Parser reader, 
        SerializationMetaData serializationMetaData,
        Read<ISerializationReaderKernel<Parser>, Parser, TObject> readCall)
    {
        var ret = readCall(reader, this, serializationMetaData);
        
        reader.Consume<MappingEnd>();

        return ret;
    }

    public void StartListSection(Parser reader)
    {
        reader.Consume<SequenceStart>();
    }

    public void EndListSection(Parser reader)
    {
        reader.Consume<SequenceEnd>();
    }

    public bool TryHasNextItem(Parser reader)
    {
        return reader.Current is not SequenceEnd;
    }

    public void StartDictionarySection(Parser reader)
    {
        reader.Consume<SequenceStart>();
    }

    public void EndDictionarySection(Parser reader)
    {
        reader.Consume<SequenceEnd>();
    }

    public bool TryHasNextDictionaryItem(Parser reader)
    {
        return reader.TryConsume<MappingStart>(out _);
    }

    public void StartDictionaryKey(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (scalar.Value != "Key")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryKey(Parser reader)
    {
    }

    public void StartDictionaryValue(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (scalar.Value != "Value")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryValue(Parser reader)
    {
    }

    public void EndDictionaryItem(Parser reader)
    {
        reader.Consume<MappingEnd>();
    }

    public void StartArray2dSection(Parser reader)
    {
        reader.Consume<SequenceStart>();
    }

    public void EndArray2dSection(Parser reader)
    {
        reader.Consume<SequenceEnd>();
    }

    public bool TryHasNextArray2dXItem(Parser reader)
    {
        return reader.Current is not SequenceEnd;
    }

    public void StartArray2dXItem(Parser reader)
    {
    }

    public void EndArray2dXItem(Parser reader)
    {
    }

    public bool TryHasNextArray2dYSection(Parser reader)
    {
        return reader.Current is SequenceStart;
    }

    public void StartArray2dYSection(Parser reader)
    {
        reader.Consume<SequenceStart>();
    }

    public void EndArray2dYSection(Parser reader)
    {
        reader.Consume<SequenceEnd>();
    }
}