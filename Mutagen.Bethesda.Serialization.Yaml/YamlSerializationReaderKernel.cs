using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
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
        if (!reader.TryConsume<Scalar>(out var scalar))
        {
            name = default!;
            return false;
        }

        name = scalar.Value;
        return true;
    }

    public FormKey ExtractFormKey(Parser reader)
    {
        throw new NotImplementedException();
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

    public char ReadChar(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return scalar.Value[0];
    }

    public bool ReadBool(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return bool.Parse(scalar.Value);
    }

    public TEnum ReadEnum<TEnum>(Parser reader)
        where TEnum : struct, Enum, IConvertible
    {
        if (Enums<TEnum>.IsFlagsEnum)
        {
            reader.Consume<SequenceStart>();
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
        else
        {
            var scalar = reader.Consume<Scalar>();
            return Enum.Parse<TEnum>(scalar.Value);
        }
    }

    public string ReadString(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return scalar.Value;
    }

    public sbyte ReadInt8(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return sbyte.Parse(scalar.Value);
    }

    public short ReadInt16(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return short.Parse(scalar.Value);
    }

    public int ReadInt32(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return int.Parse(scalar.Value);
    }

    public long ReadInt64(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return long.Parse(scalar.Value);
    }

    public byte ReadUInt8(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return byte.Parse(scalar.Value);
    }

    public ushort ReadUInt16(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return ushort.Parse(scalar.Value);
    }

    public uint ReadUInt32(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return uint.Parse(scalar.Value);
    }

    public ulong ReadUInt64(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return ulong.Parse(scalar.Value);
    }

    public float ReadFloat(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return float.Parse(scalar.Value);
    }

    public ModKey ReadModKey(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return ModKey.FromNameAndExtension(scalar.Value);
    }

    public FormKey ReadFormKey(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return FormKey.Factory(scalar.Value);
    }

    public Color ReadColor(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return ColorExt.ConvertFromCommaString(scalar.Value);
    }

    public RecordType ReadRecordType(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        var str = scalar.Value;
        if (str == "null")
        {
            return RecordType.Null;
        }
        return new RecordType(str);
    }

    public P2Int ReadP2Int(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (P2Int.TryParse(scalar.Value, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int)}: {scalar.Value}");
    }

    public P2Int16 ReadP2Int16(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (P2Int16.TryParse(scalar.Value, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Int16)}: {scalar.Value}");
    }

    public P2Float ReadP2Float(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (P2Float.TryParse(scalar.Value, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P2Float)}: {scalar.Value}");
    }

    public P3Float ReadP3Float(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (P3Float.TryParse(scalar.Value, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Float)}: {scalar.Value}");
    }

    public P3UInt8 ReadP3UInt8(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (P3UInt8.TryParse(scalar.Value, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt8)}: {scalar.Value}");
    }

    public P3Int16 ReadP3Int16(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (P3Int16.TryParse(scalar.Value, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3Int16)}: {scalar.Value}");
    }

    public P3UInt16 ReadP3UInt16(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (P3UInt16.TryParse(scalar.Value, out var pt))
        {
            return pt;
        }

        throw new ArgumentException($"Could not parse string into {nameof(P3UInt16)}: {scalar.Value}");
    }

    public Percent ReadPercent(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        if (double.TryParse(scalar.Value, out var d))
        {
            return new Percent(d);
        }

        throw new ArgumentException($"Could not parse string into {nameof(Percent)}: {scalar.Value}");
    }

    public TranslatedString ReadTranslatedString(Parser reader)
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

    public MemorySlice<byte> ReadBytes(Parser reader)
    {
        var scalar = reader.Consume<Scalar>();
        return Convert.FromHexString(scalar.Value);
    }

    public TObject ReadLoqui<TObject>(
        Parser reader, 
        SerializationMetaData serializationMetaData,
        Read<ISerializationReaderKernel<Parser>, Parser, TObject> readCall)
    {
        reader.Consume<MappingStart>();

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

    public void StartArray2dXSection(Parser reader)
    {
    }

    public void EndArray2dXSection(Parser reader)
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