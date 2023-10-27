using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using DocumentStart = YamlDotNet.Core.Events.DocumentStart;
using Scalar = YamlDotNet.Core.Events.Scalar;
using StreamStart = YamlDotNet.Core.Events.StreamStart;

namespace Mutagen.Bethesda.Serialization.Yaml;

public readonly struct YamlReadingUnit : IDisposable, IContainStreamPackage
{
    private readonly IDisposable _streamDispose;
    public Parser Parser { get; }
    public StreamPackage StreamPackage { get; }

    public YamlReadingUnit(StreamPackage stream)
    {
        var sw = new StreamReader(stream.Stream, leaveOpen: true);
        Parser = new Parser(sw);
        _streamDispose = sw;

        Parser.Consume<StreamStart>();
        Parser.Consume<DocumentStart>();
        Parser.Consume<MappingStart>();

        StreamPackage = stream;
    }

    public void Dispose()
    {
        _streamDispose.Dispose();
    }
}

public class YamlSerializationReaderKernel : ISerializationReaderKernel<YamlReadingUnit>
{
    public string ExpectedExtension => ".yaml";
    
    public YamlReadingUnit GetNewObject(StreamPackage stream)
    {
        return new YamlReadingUnit(stream);
    }

    public bool TryGetNextField(YamlReadingUnit reader, out string name)
    {
        reader.Parser.TryConsume<MappingStart>(out _);

        if (reader.Parser.TryConsume<MappingEnd>(out _))
        {
            name = default!;
            return false;
        }

        if (reader.Parser.TryConsume<StreamEnd>(out _))
        {
            name = default!;
            return false;
        }
        
        if (!reader.Parser.TryConsume<Scalar>(out var scalar))
        {
            name = default!;
            return false;
        }

        name = scalar.Value;
        return true;
    }

    public Type GetNextType(YamlReadingUnit reader, string namespaceString)
    {
        reader.Parser.TryConsume<MappingStart>(out _);
        reader.Parser.Consume<Scalar>();
        
        var scalar = reader.Parser.Consume<Scalar>();
        var val = scalar.Value;
        var typeStr = $"{namespaceString}.{val}, {namespaceString}";
        return Type.GetType(typeStr)!;
    }

    public FormKey ExtractFormKey(YamlReadingUnit reader)
    {
        if (!TryGetNextField(reader, out var name)
            && name != "FormKey")
        {
            throw new NullReferenceException("Required FormKey for MajorRecord was not first");
        }
        
        return ReadFormKey(reader) ?? throw new NullReferenceException("Required FormKey for MajorRecord was null");
    }

    public void Skip(YamlReadingUnit reader)
    {
        if (reader.Parser.Current is MappingStart)
        {
            SkipObject(reader);
        }
        else if (reader.Parser.Current is SequenceStart)
        {
            SkipArray(reader);
        }
        else
        {
            reader.Parser.MoveNext();
        }
    }

    private void SkipObject(YamlReadingUnit reader)
    {
        reader.Parser.Consume<MappingStart>();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.Parser.TryConsume<MappingStart>(out _))
            {
                objCount++;
            }
            else if (reader.Parser.TryConsume<MappingEnd>(out _))
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }
            else if (!reader.Parser.MoveNext())
            {
                break;
            }
        }
    }

    private void SkipArray(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceStart>();
        int objCount = 1;
        while (objCount > 0)
        {
            if (reader.Parser.TryConsume<SequenceStart>(out _))
            {
                objCount++;
            }
            else if (reader.Parser.TryConsume<SequenceEnd>(out _))
            {
                objCount--;
                if (objCount == 0)
                {
                    break;
                }
            }

            if (!reader.Parser.MoveNext()) break;
        }
    }

    public char? ReadChar(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return str[0];
    }

    public bool? ReadBool(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return bool.Parse(str);
    }

    public TEnum? ReadEnum<TEnum>(YamlReadingUnit reader)
        where TEnum : struct, Enum, IConvertible
    {
        if (Enums<TEnum>.IsFlagsEnum)
        {
            if (reader.Parser.TryConsume<SequenceStart>(out _))
            {
                TEnum ret = default;
                while (reader.Parser.TryConsume<Scalar>(out var e))
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

                reader.Parser.Consume<SequenceEnd>();
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

    public string? ReadString(YamlReadingUnit reader)
    {
        if (reader.Parser.TryConsume<MappingStart>(out _))
        {
            reader.Parser.Consume<MappingEnd>();
            return null;
        }
        var scalar = reader.Parser.Consume<Scalar>();
        return scalar.Value.ReplaceLineEndings("\r\n");
    }

    public sbyte? ReadInt8(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return sbyte.Parse(str);
    }

    public short? ReadInt16(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return short.Parse(str);
    }

    public int? ReadInt32(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return int.Parse(str);
    }

    public long? ReadInt64(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return long.Parse(str);
    }

    public byte? ReadUInt8(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return byte.Parse(str);
    }

    public ushort? ReadUInt16(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ushort.Parse(str);
    }

    public uint? ReadUInt32(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return uint.Parse(str);
    }

    public ulong? ReadUInt64(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ulong.Parse(str);
    }

    public float? ReadFloat(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return float.Parse(str);
    }

    public ModKey? ReadModKey(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ModKey.FromNameAndExtension(str);
    }

    public FormKey? ReadFormKey(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return FormKey.Factory(str);
    }

    public Color? ReadColor(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        return ColorExt.FromHexString(str);
    }

    public RecordType? ReadRecordType(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (str == "null")
        {
            return RecordType.Null;
        }
        return new RecordType(str);
    }

    public P2Int? ReadP2Int(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P2Int.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int)}: {str}");
    }

    public P2Int16? ReadP2Int16(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P2Int16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int16)}: {str}");
    }

    public P2UInt8? ReadP2UInt8(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P2UInt8.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2UInt8)}: {str}");
    }

    public P2Float? ReadP2Float(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P2Float.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Float)}: {str}");
    }

    public P3Float? ReadP3Float(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3Float.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Float)}: {str}");
    }

    public P3UInt8? ReadP3UInt8(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3UInt8.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt8)}: {str}");
    }

    public P3Int16? ReadP3Int16(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3Int16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Int16)}: {str}");
    }

    public P3UInt16? ReadP3UInt16(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (P3UInt16.TryParse(str, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt16)}: {str}");
    }

    public Percent? ReadPercent(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (double.TryParse(str, out var d))
        {
            return new Percent(d);
        }

        throw new ArgumentException($"Could not parse string into {nameof(Percent)}: {str}");
    }

    public TimeOnly? ReadTimeOnly(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (TimeOnly.TryParse(str, out var d))
        {
            return d;
        }

        throw new ArgumentException($"Could not parse string into {nameof(TimeOnly)}: {str}");
    }

    public DateOnly? ReadDateOnly(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        if (str.IsNullOrWhitespace()) return null;
        if (DateOnly.TryParse(str, out var d))
        {
            return d;
        }

        throw new ArgumentException($"Could not parse string into {nameof(DateOnly)}: {str}");
    }

    public Guid? ReadGuid(YamlReadingUnit reader)
    {
        var str = ReadString(reader);
        return str == null ? null : new Guid(str);
    }

    public TranslatedString? ReadTranslatedString(YamlReadingUnit reader)
    {
        Language? targetLanguage = null;
        string? mainString = null;
        List<KeyValuePair<Language, string?>>? pairs = null;
        Language? language = null;
        string? str = null;
        int objCount = 1;
        bool stop = false;
        bool inArray = false;

        reader.Parser.Consume<MappingStart>();
        
        while (!stop)
        {
            switch (reader.Parser.Current)
            {
                case MappingStart:
                    language = null;
                    str = null;
                    objCount++;
                    break;
                case Scalar s:
                    var propName = s.Value;
                    reader.Parser.Consume<Scalar>();
                    if (inArray)
                    {
                        if (propName == "Language"
                            && reader.Parser.Accept<Scalar>(out var lang))
                        {
                            language = Enum.Parse<Language>(lang.Value);
                        }
                        else if (propName == "String"
                                 && reader.Parser.Accept<Scalar>(out var strVal))
                        {
                            str = strVal.Value.ReplaceLineEndings("\r\n");
                        }
                    }
                    else if (propName == "TargetLanguage"
                             && reader.Parser.Accept<Scalar>(out var lang))
                    {
                        targetLanguage = Enum.Parse<Language>(lang.Value);
                    }
                    else if (propName == "Value"
                             && reader.Parser.Accept<Scalar>(out var val))
                    {
                        mainString = val.Value.ReplaceLineEndings("\r\n");
                    }
                    else if (propName == "Values"
                             && reader.Parser.Accept<SequenceStart>(out _))
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

            reader.Parser.MoveNext();
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

    public MemorySlice<byte>? ReadBytes(YamlReadingUnit reader)
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

    public async Task<TObject?> ReadLoqui<TObject>(
        YamlReadingUnit reader, 
        SerializationMetaData serializationMetaData,
        ReadAsync<ISerializationReaderKernel<YamlReadingUnit>, YamlReadingUnit, TObject> readCall)
    {
        return await readCall(reader, this, serializationMetaData);
    }

    public void StartListSection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceStart>();
    }

    public void EndListSection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceEnd>();
    }

    public bool TryHasNextItem(YamlReadingUnit reader)
    {
        switch (reader.Parser.Current)
        {
            case SequenceEnd:
            case DocumentEnd:
            case StreamEnd:
                return false;
        }

        return true;
    }

    public void StartDictionarySection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceStart>();
    }

    public void EndDictionarySection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceEnd>();
    }

    public bool TryHasNextDictionaryItem(YamlReadingUnit reader)
    {
        return reader.Parser.TryConsume<MappingStart>(out _);
    }

    public void StartDictionaryKey(YamlReadingUnit reader)
    {
        var scalar = reader.Parser.Consume<Scalar>();
        if (scalar.Value != "Key")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryKey(YamlReadingUnit reader)
    {
    }

    public void StartDictionaryValue(YamlReadingUnit reader)
    {
        var scalar = reader.Parser.Consume<Scalar>();
        if (scalar.Value != "Value")
        {
            throw new DataMisalignedException();
        }
    }

    public void EndDictionaryValue(YamlReadingUnit reader)
    {
    }

    public void EndDictionaryItem(YamlReadingUnit reader)
    {
        reader.Parser.Consume<MappingEnd>();
    }

    public void StartArray2dSection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceStart>();
    }

    public void EndArray2dSection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceEnd>();
    }

    public bool TryHasNextArray2dXItem(YamlReadingUnit reader)
    {
        return reader.Parser.Current is not SequenceEnd;
    }

    public void StartArray2dXItem(YamlReadingUnit reader)
    {
    }

    public void EndArray2dXItem(YamlReadingUnit reader)
    {
    }

    public bool TryHasNextArray2dYSection(YamlReadingUnit reader)
    {
        return reader.Parser.Current is SequenceStart;
    }

    public void StartArray2dYSection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceStart>();
    }

    public void EndArray2dYSection(YamlReadingUnit reader)
    {
        reader.Parser.Consume<SequenceEnd>();
    }
}