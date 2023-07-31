using System.Drawing;
using System.IO.Abstractions;
using System.Text;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
using Noggog.IO;
using Noggog.Testing.AutoFixture;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.KernelTests;

public abstract class AKernelTest<TWriterKernel, TWriter, TReaderKernel, TReader> : ASerializationTest<TWriterKernel, TWriter, TReaderKernel, TReader>
    where TWriterKernel : ISerializationWriterKernel<TWriter>, new()
    where TReaderKernel : ISerializationReaderKernel<TReader>, new()
    where TWriter : IContainStreamPackage
{
    private async Task<string> GetResults(
        Func<MutagenSerializationWriterKernel<TWriterKernel, TWriter>, TWriter, Task> toDo)
    {
        using var memStream = new MemoryStream();
        var stream = GetStreamPackage(memStream);
        var writerObj = WriterKernel.GetNewObject(stream);
        await toDo(WriterKernel, writerObj);
        WriterKernel.Finalize(stream, writerObj);
        if (writerObj is IDisposable disp) disp.Dispose();
        memStream.Position = 0;
        StreamReader reader = new StreamReader(memStream);
        var ret = await reader.ReadToEndAsync();
        return ret;
    }

    private T? GetReadResults<T>(
        string str,
        string nickname,
        Func<TReaderKernel, TReader, T> toDo)
    {
        var stream = GetStreamPackage(new MemoryStream(Encoding.UTF8.GetBytes(str)));
        var readerObj = ReaderKernel.GetNewObject(stream);
        T? obj = default;
        bool found = false;
        while (ReaderKernel.TryGetNextField(readerObj, out var name))
        {
            if (name == nickname)
            {
                obj = toDo(ReaderKernel, readerObj);
                found = true;
                break;
            }
            else
            {
                ReaderKernel.Skip(readerObj);
            }
        }

        if (!found)
        {
            throw new Exception();
        }

        if (readerObj is IDisposable disp) disp.Dispose();
        return obj;
    }

    interface IReadResults
    {
        string Name { get; } 
        Task Check(TReaderKernel kernel, TReader reader);
    }

    record ReadResults<T>(string Name, Func<TReaderKernel, TReader, Task<T?>> Reader, T? Expected) : IReadResults
    {
        public async Task Check(TReaderKernel kernel, TReader reader)
        {
            var val = await Reader(kernel, reader);
            if (Expected is IReadOnlyList<object> e)
            {
                var list = (IReadOnlyList<object>)val!;
                list.Should().Equal(e);
            }
            else if (Expected is ReadOnlyMemorySlice<byte> bytes)
            {
                var slice = val as ReadOnlyMemorySlice<byte>?;
                ((IEnumerable<byte>)Expected).Should().Equal(slice);
            }
            else
            {
                val.Should().Be(Expected);
            }
        }
    }
    
    private async Task CheckReadResults(
        string str,
        params IReadResults[] results)
    {
        var kernel = new TReaderKernel();
        using var mem = new MemoryStream(Encoding.UTF8.GetBytes(str));
        var stream = GetStreamPackage(mem);
        var readerObj = kernel.GetNewObject(stream);
        using var disp = readerObj as IDisposable;
        var dict = results.ToDictionary(x => x.Name, x => x);
        while (kernel.TryGetNextField(readerObj, out var name))
        {
            if (!dict.TryGetValue(name, out var result))
            {
                throw new Exception();
            }
            await result.Check(kernel, readerObj);
            dict.Remove(name);
        }

        if (dict.Count > 0)
        {
            throw new Exception();
        }
    }

    private async Task<string> GetPrimitiveWriteTest<T>(
        string nickName,
        Action<MutagenSerializationWriterKernel<TWriterKernel, TWriter>, TWriter, string?, T?, T?> callback,
        T nonDefaultToTest,
        params T[] items)
    {
        return await GetResults(async (k, w) =>
        {
            int i = 0;
            callback(k, w, $"{nickName}{i++}", default, default);
            callback(k, w, $"{nickName}{i++}", default, nonDefaultToTest);
            foreach (var item in items)
            {
                callback(k, w, $"{nickName}{i++}", item, default);
            }
        });
    }

    private async Task DoPrimitiveTest<T>(
        string nickName,
        Action<MutagenSerializationWriterKernel<TWriterKernel, TWriter>, TWriter, string?, T?, T?> writeCallback,
        Func<TReaderKernel, TReader, T> readCallback,
        T nonDefaultToTest,
        params T[] items)
    {
        await DoPrimitiveTest(
            nickName,
            writeCallback,
            readCallback,
            equality: (l, r) => EqualityComparer<T>.Default.Equals(l, r),
            nonDefaultToTest,
            items);
    }

    private async Task DoPrimitiveTest<T>(
        string nickName,
        Action<MutagenSerializationWriterKernel<TWriterKernel, TWriter>, TWriter, string?, T?, T?> writeCallback,
        Func<TReaderKernel, TReader, T?> readCallback,
        Func<T?, T?, bool> equality,
        T nonDefaultToTest,
        params T[] items)
    {
        var str = await GetPrimitiveWriteTest(nickName, writeCallback, nonDefaultToTest, items);
        await TestHelper.VerifyString(str);
        await CheckReadResults(
            str,
            new ReadResults<T>[]
            {
                new ReadResults<T>($"{nickName}1", async (k, r) => readCallback(k, r), default(T)),
            }.Concat(items.Select((x, i) =>
            {
                var name = $"{nickName}{(2 + i)}";
                return new ReadResults<T>(name, async (k, r) => readCallback(k, r), x);
            })).ToArray());
    }

    [Fact]
    public async Task Nothing()
    {
        var str = await GetResults(async (k, w) => { });
        await TestHelper.VerifyString(str);
    }

    [Fact]
    public async Task Char()
    {
        await DoPrimitiveTest<char?>(
            "Char",
            (k, w, name, item, def) => k.WriteChar(w, name, item, def),
            (k, r) => k.ReadChar(r),
            'c',
            'c',
            (char)165);
    }

    [Fact]
    public async Task Bool()
    {
        await DoPrimitiveTest<bool?>(
            "Bool",
            (k, w, name, item, def) => k.WriteBool(w, name, item, def),
            (k, r) => k.ReadBool(r),
            true,
            true,
            false);
    }

    [Fact]
    public async Task String()
    {
        await DoPrimitiveTest<string?>(
            "String",
            (k, w, name, item, def) => k.WriteString(w, name, item, def),
            (k, r) => k.ReadString(r),
            "Hello",
            string.Empty,
            "Hello",
            "Hello\r\nWorld");
    }

    [Fact]
    public async Task Int8()
    {
        await DoPrimitiveTest<sbyte?>(
            "Int8",
            (k, w, name, item, def) => k.WriteInt8(w, name, item, def),
            (k, r) => k.ReadInt8(r),
            1,
            -4,
            5,
            0,
            sbyte.MinValue,
            sbyte.MaxValue);
    }

    [Fact]
    public async Task Int16()
    {
        await DoPrimitiveTest<short?>(
            "Int16",
            (k, w, name, item, def) => k.WriteInt16(w, name, item, def),
            (k, r) => k.ReadInt16(r),
            1,
            -4,
            5,
            0,
            short.MinValue,
            short.MaxValue);
    }

    [Fact]
    public async Task Int32()
    {
        await DoPrimitiveTest<int?>(
            "Int32",
            (k, w, name, item, def) => k.WriteInt32(w, name, item, def),
            (k, r) => k.ReadInt32(r),
            1,
            -4,
            5,
            0,
            int.MinValue,
            int.MaxValue);
    }

    [Fact]
    public async Task Int64()
    {
        await DoPrimitiveTest<long?>(
            "Int64",
            (k, w, name, item, def) => k.WriteInt64(w, name, item, def),
            (k, r) => k.ReadInt64(r),
            1,
            -4,
            5,
            0,
            long.MinValue,
            long.MaxValue);
    }

    [Fact]
    public async Task UInt8()
    {
        await DoPrimitiveTest<byte?>(
            "UInt8",
            (k, w, name, item, def) => k.WriteUInt8(w, name, item, def),
            (k, r) => k.ReadUInt8(r),
            1,
            5,
            byte.MinValue,
            byte.MaxValue);
    }

    [Fact]
    public async Task UInt16()
    {
        await DoPrimitiveTest<ushort?>(
            "UInt16",
            (k, w, name, item, def) => k.WriteUInt16(w, name, item, def),
            (k, r) => k.ReadUInt16(r),
            1,
            5,
            ushort.MinValue,
            ushort.MaxValue);
    }

    [Fact]
    public async Task UInt32()
    {
        await DoPrimitiveTest<uint?>(
            "UInt32",
            (k, w, name, item, def) => k.WriteUInt32(w, name, item, def),
            (k, r) => k.ReadUInt32(r),
            1,
            5,
            uint.MinValue,
            uint.MaxValue);
    }

    [Fact]
    public async Task UInt64()
    {
        await DoPrimitiveTest<ulong?>(
            "UInt64",
            (k, w, name, item, def) => k.WriteUInt64(w, name, item, def),
            (k, r) => k.ReadUInt64(r),
            1,
            5,
            ulong.MinValue,
            ulong.MaxValue);
    }

    [Fact]
    public async Task Float()
    {
        await DoPrimitiveTest<float?>(
            "Float",
            (k, w, name, item, def) => k.WriteFloat(w, name, item, def),
            (k, r) => k.ReadFloat(r),
            1,
            0,
            -5,
            5,
            float.MinValue,
            float.MaxValue);
    }

    [Fact]
    public async Task TimeOnly()
    {
        await DoPrimitiveTest<TimeOnly?>(
            "TimeOnly",
            (k, w, name, item, def) => k.WriteTimeOnly(w, name, item, def),
            (k, r) => k.ReadTimeOnly(r),
            new TimeOnly(),
            new TimeOnly(4, 14),
            new TimeOnly(4, 14, 14),
            new TimeOnly(17, 14, 14, 34, 12),
            System.TimeOnly.MinValue,
            System.TimeOnly.MaxValue);
    }

    [Fact]
    public async Task DateOnly()
    {
        await DoPrimitiveTest<DateOnly?>(
            "DateOnly",
            (k, w, name, item, def) => k.WriteDateOnly(w, name, item, def),
            (k, r) => k.ReadDateOnly(r),
            new DateOnly(),
            new DateOnly(2023, 5, 14),
            System.DateOnly.MinValue,
            System.DateOnly.MaxValue);
    }

    [Fact]
    public async Task ModKey()
    {
        await DoPrimitiveTest<ModKey?>(
            "ModKey",
            (k, w, name, item, def) => k.WriteModKey(w, name, item, def),
            (k, r) => k.ReadModKey(r),
            Plugins.ModKey.FromNameAndExtension("SomeMod.esp"),
            Plugins.ModKey.Null,
            Plugins.ModKey.FromNameAndExtension("SomeMod.esp"));
    }

    [Fact]
    public async Task FormKey()
    {
        await DoPrimitiveTest<FormKey?>(
            "FormKey",
            (k, w, name, item, def) => k.WriteFormKey(w, name, item, def),
            (k, r) => k.ReadFormKey(r),
            Plugins.FormKey.Factory("123456:SomeMod.esp"),
            Plugins.FormKey.Null,
            Plugins.FormKey.Factory("123456:SomeMod.esp"));
    }

    [Fact]
    public async Task ExtractFormKey()
    {
        var fk = Plugins.FormKey.Factory("000800:InputMod.esp");
        var str = await GetResults(async (k, w) => { k.WriteFormKey(w, "FormKey", fk, default); });

        var kernel = new TReaderKernel();
        var stream = GetStreamPackage(new MemoryStream(Encoding.UTF8.GetBytes(str)));
        var readerObj = kernel.GetNewObject(stream);
        kernel.ExtractFormKey(readerObj)
            .Should().Be(fk);
    }

    [Fact]
    public async Task RecordType()
    {
        await DoPrimitiveTest<RecordType?>(
            "RecordType",
            (k, w, name, item, def) => k.WriteRecordType(w, name, item, def),
            (k, r) => k.ReadRecordType(r),
            new RecordType("TEST"),
            Plugins.RecordType.Null,
            new RecordType("TEST"));
    }

    [Fact]
    public async Task P2Int()
    {
        await DoPrimitiveTest<P2Int?>(
            "P2Int",
            (k, w, name, item, def) => k.WriteP2Int(w, name, item, def),
            (k, r) => k.ReadP2Int(r),
            new P2Int(1, -3),
            new P2Int(),
            new P2Int(1, -3),
            new P2Int(int.MaxValue, int.MaxValue),
            new P2Int(int.MinValue, int.MinValue));
    }

    [Fact]
    public async Task P2Int16()
    {
        await DoPrimitiveTest<P2Int16?>(
            "P2Int16",
            (k, w, name, item, def) => k.WriteP2Int16(w, name, item, def),
            (k, r) => k.ReadP2Int16(r),
            new P2Int16(1, -3),
            new P2Int16(),
            new P2Int16(1, -3),
            new P2Int16(short.MaxValue, short.MaxValue),
            new P2Int16(short.MinValue, short.MinValue));
    }

    [Fact]
    public async Task P2Float()
    {
        await DoPrimitiveTest<P2Float?>(
            "P2Float",
            (k, w, name, item, def) => k.WriteP2Float(w, name, item, def),
            (k, r) => k.ReadP2Float(r),
            new P2Float(1, -3),
            new P2Float(),
            new P2Float(1, -3),
            new P2Float(float.MaxValue, float.MaxValue),
            new P2Float(float.MinValue, float.MinValue));
    }

    [Fact]
    public async Task P3Float()
    {
        await DoPrimitiveTest<P3Float?>(
            "P3Float",
            (k, w, name, item, def) => k.WriteP3Float(w, name, item, def),
            (k, r) => k.ReadP3Float(r),
            new P3Float(1, 3, 0),
            new P3Float(),
            new P3Float(1, -3, 0),
            new P3Float(float.MaxValue, float.MaxValue, float.MaxValue),
            new P3Float(float.MinValue, float.MinValue, float.MinValue));
    }

    [Fact]
    public async Task P3UInt8()
    {
        await DoPrimitiveTest<P3UInt8?>(
            "P3UInt8",
            (k, w, name, item, def) => k.WriteP3UInt8(w, name, item, def),
            (k, r) => k.ReadP3UInt8(r),
            new P3UInt8(1, 3, 0),
            new P3UInt8(),
            new P3UInt8(1, 3, 0),
            new P3UInt8(byte.MaxValue, byte.MaxValue, byte.MaxValue),
            new P3UInt8(byte.MinValue, byte.MinValue, byte.MinValue));
    }

    [Fact]
    public async Task P3Int16()
    {
        await DoPrimitiveTest<P3Int16?>(
            "P3Int16",
            (k, w, name, item, def) => k.WriteP3Int16(w, name, item, def),
            (k, r) => k.ReadP3Int16(r),
            new P3Int16(1, 3, 0),
            new P3Int16(),
            new P3Int16(1, 3, 0),
            new P3Int16(short.MaxValue, short.MaxValue, short.MaxValue),
            new P3Int16(short.MinValue, short.MinValue, short.MinValue));
    }

    [Fact]
    public async Task P3UInt16()
    {
        await DoPrimitiveTest<P3UInt16?>(
            "P3UInt16",
            (k, w, name, item, def) => k.WriteP3UInt16(w, name, item, def),
            (k, r) => k.ReadP3UInt16(r),
            new P3UInt16(1, 3, 0),
            new P3UInt16(),
            new P3UInt16(1, 3, 0),
            new P3UInt16(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue),
            new P3UInt16(ushort.MinValue, ushort.MinValue, ushort.MinValue));
    }

    [Fact]
    public async Task Percent()
    {
        await DoPrimitiveTest<Percent?>(
            "Percent",
            (k, w, name, item, def) => k.WritePercent(w, name, item, def),
            (k, r) => k.ReadPercent(r),
            new Percent(0d),
            new Percent(),
            new Percent(0d),
            new Percent(0.5d),
            new Percent(1d));
    }

    [Fact]
    public async Task Color()
    {
        await DoPrimitiveTest<Color?>(
            "Color",
            (k, w, name, item, def) => k.WriteColor(w, name, item, def),
            (k, r) => k.ReadColor(r),
            System.Drawing.Color.FromArgb(255, 2, 3, 4),
            System.Drawing.Color.FromArgb(255, 0, 0, 0),
            System.Drawing.Color.FromArgb(1, 2, 3));
    }

    [Fact]
    public async Task ObjectType()
    {
        var str = await GetResults(async (k, w) =>
        {
            await k.WriteLoqui<int>(w, "Loqui", 4, null!, async (w, o, k, m) =>
            {
                k.WriteType(w, typeof(NpcLevel));
            });
        });

        await TestHelper.VerifyString(str);

        var readItem = GetReadResults(str, "Loqui", (kernel, reader) =>
        {
            return kernel.GetNextType(reader, "Mutagen.Bethesda.Skyrim");
        });

        readItem.Should().Be(typeof(NpcLevel));
    }

    [Fact]
    public async Task List()
    {
        var str = await GetResults(async (k, w) =>
        {
            k.StartListSection(w, "MyList");
            k.WriteString(w, null, "Hello", default);
            k.WriteString(w, null, "There", default);
            k.WriteString(w, null, "World", default);
            k.EndListSection(w);
            k.WriteInt8(w, "SomeInt", 4, default);
        });

        await TestHelper.VerifyString(str);

        await CheckReadResults(str,
            new ReadResults<List<string>>(
                "MyList",
                async (kernel, reader) =>
                {
                    var ret = new List<string>();
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = kernel.ReadString(reader).StripNull(string.Empty);
                        ret.Add(item);
                    }

                    kernel.EndListSection(reader);
                    return ret;
                },
                new List<string>() { "Hello", "There", "World" }),
            new ReadResults<int?>("SomeInt", async (k, r) => k.ReadUInt8(r), 4));
    }

    record SomeClass(int? Int, string String);

    [Fact]
    public async Task LoquiList()
    {
        var item1 = new SomeClass(4, "Hello");
        var item2 = new SomeClass(6, "World");

        var str = await GetResults(async (k, w) =>
        {
            var meta = new SerializationMetaData(GameRelease.SkyrimSE);
            k.StartListSection(w, "MyList");
            await k.WriteLoqui(w, null, item1, meta, async (subW, obj, subKernel, meta) =>
            {
                subKernel.WriteInt32(subW, "Int", obj.Int, default);
                subKernel.WriteString(subW, "String", obj.String, default);
            });
            await k.WriteLoqui(w, null, item2, meta, async (subW, obj, subKernel, meta) =>
            {
                subKernel.WriteInt32(subW, "Int", obj.Int, default);
                subKernel.WriteString(subW, "String", obj.String, default);
            });
            k.EndListSection(w);
            k.WriteInt8(w, "SomeInt", 4, default);
        });
        await TestHelper.VerifyString(str);

        await CheckReadResults(str,
            new ReadResults<List<SomeClass>>(
                "MyList",
                async (kernel, reader) =>
                {
                    var metaData = new SerializationMetaData(GameRelease.SkyrimSE);
                    var ret = new List<SomeClass>();
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = await kernel.ReadLoqui(reader, metaData, static async (r, k, m) =>
                        {
                            int? i = default;
                            string s = string.Empty;
                            while (k.TryGetNextField(r, out var name))
                            {
                                switch (name)
                                {
                                    case "Int":
                                        i = k.ReadInt32(r);
                                        break;
                                    case "String":
                                        s = k.ReadString(r).StripNull(nameof(String));
                                        break;
                                    default:
                                        throw new DataMisalignedException();
                                }
                            }

                            return new SomeClass(i, s);
                        });
                        if (item != null)
                        {
                            ret.Add(item);
                        }
                    }

                    kernel.EndListSection(reader);
                    return ret;
                },
                new List<SomeClass> { item1, item2 }),
            new ReadResults<int?>("SomeInt", async (k, r) => k.ReadUInt8(r), 4));
    }

    [Fact]
    public async Task Dict()
    {
        var item1 = new SomeClass(1, "value");
        var item2 = new SomeClass(2, "value2");

        var str = await GetResults(async (k, w) =>
        {
            k.StartDictionarySection(w, "MyDict");
            k.StartDictionaryItem(w);
            k.StartDictionaryKey(w);
            k.WriteInt8(w, null, 1, default);
            k.EndDictionaryKey(w);
            k.StartDictionaryValue(w);
            k.WriteString(w, null, "value", default);
            k.EndDictionaryValue(w);
            k.EndDictionaryItem(w);
            k.StartDictionaryItem(w);
            k.StartDictionaryKey(w);
            k.WriteInt8(w, null, 2, default);
            k.EndDictionaryKey(w);
            k.StartDictionaryValue(w);
            k.WriteString(w, null, "value2", default);
            k.EndDictionaryValue(w);
            k.EndDictionaryItem(w);
            k.EndDictionarySection(w);
            k.WriteInt8(w, "SomeInt", 4, default);
        });
        await TestHelper.VerifyString(str);

        await CheckReadResults(str,
            new ReadResults<List<SomeClass>>(
                "MyDict", 
                async (kernel, reader) =>
                {
                    var ret = new List<SomeClass>();
                    kernel.StartDictionarySection(reader);
                    while (kernel.TryHasNextDictionaryItem(reader))
                    {
                        kernel.StartDictionaryKey(reader);
                        int? k = kernel.ReadInt32(reader);
                        kernel.EndDictionaryKey(reader);
                        kernel.StartDictionaryValue(reader);
                        string val = kernel.ReadString(reader).StripNull(string.Empty);
                        kernel.EndDictionaryValue(reader);
                        kernel.EndDictionaryItem(reader);
                        ret.Add(new SomeClass(k, val));
                    }

                    kernel.EndDictionarySection(reader);
                    return ret;
                },
                new List<SomeClass>() { item1, item2 }),
            new ReadResults<int?>("SomeInt", async (k, r) => k.ReadUInt8(r), 4));
    }

    [Fact]
    public async Task Array2d()
    {
        var str = await GetResults(async (k, w) =>
        {
            k.StartArray2dSection(w, "MyArr");
            k.StartArray2dYSection(w);
            k.StartArray2dXItem(w);
            k.WriteInt8(w, null, 1, default);
            k.EndArray2dXItem(w);
            k.StartArray2dXItem(w);
            k.WriteInt8(w, null, 2, default);
            k.EndArray2dXItem(w);
            k.EndArray2dYSection(w);
            k.StartArray2dYSection(w);
            k.StartArray2dXItem(w);
            k.WriteInt8(w, null, 4, default);
            k.EndArray2dXItem(w);
            k.EndArray2dYSection(w);
            k.EndArray2dSection(w);
        });
        await TestHelper.VerifyString(str);

        var readItem = GetReadResults(str, "MyArr", (kernel, reader) =>
        {
            var ret = new List<(int X, int Y, int? Val)>();
            kernel.StartArray2dSection(reader);
            int y = 0;
            while (kernel.TryHasNextArray2dYSection(reader))
            {
                kernel.StartArray2dYSection(reader);
                int x = 0;
                while (kernel.TryHasNextArray2dXItem(reader))
                {
                    kernel.StartArray2dXItem(reader);
                    ret.Add((x, y, kernel.ReadInt32(reader)));
                    kernel.EndArray2dXItem(reader);
                    x++;
                }

                kernel.EndArray2dYSection(reader);
                y++;
            }

            kernel.EndArray2dSection(reader);
            return ret;
        });
        readItem.Should().Equal(
            (0, 0, 1),
            (1, 0, 2),
            (0, 1, 4));
    }

    [Fact]
    public async Task TranslatedString()
    {
        await DoPrimitiveTest<ITranslatedStringGetter?>(
            "TranslatedString",
            (k, w, name, item, def) => k.WriteTranslatedString(w, name, item, def),
            (k, r) => k.ReadTranslatedString(r),
            new TranslatedString(Language.English, "Hello"),
            new TranslatedString(Language.English),
            new TranslatedString(Language.English, default(string?)),
            new TranslatedString(Language.English, "Hello"),
            new TranslatedString(Language.English,
                new KeyValuePair<Language, string>(Language.English, "Hello"),
                new KeyValuePair<Language, string>(Language.French, "Bonjour")),
            new TranslatedString(Language.Spanish,
                new KeyValuePair<Language, string>(Language.English, "Hello"),
                new KeyValuePair<Language, string>(Language.French, "Bonjour")),
            new TranslatedString(Language.English, "Multi\r\nLine"),
            new TranslatedString(Language.Spanish,
                new KeyValuePair<Language, string>(Language.English, "Multi\r\nLine"),
                new KeyValuePair<Language, string>(Language.French, "Multi\r\nLine")));
    }

    [Fact]
    public async Task Bytes()
    {
        await DoPrimitiveTest<ReadOnlyMemorySlice<byte>?>(
            "Bytes",
            (k, w, name, item, def) => k.WriteBytes(w, name, item, def),
            (k, r) => k.ReadBytes(r),
            equality: (l, r) => l.SequenceEqual(r),
            new ReadOnlyMemorySlice<byte>(new byte[] { 1 }),
            new ReadOnlyMemorySlice<byte>(),
            new ReadOnlyMemorySlice<byte>(Array.Empty<byte>()),
            new ReadOnlyMemorySlice<byte>(new byte[] { 1, 2, 3, 4, 5, 254, 255 }));
    }
    
    [Fact]
    public async Task Group()
    {
        var objs = new TestGroup()
        {
            SomeGroupField = true,
        };
        objs.Records.SetTo(new List<TestMajorRecord>()
        {
            new TestMajorRecord(Plugins.FormKey.Factory("123456:Skyrim.esm"), "Hello"),
            new TestMajorRecord(Plugins.FormKey.Factory("123457:Skyrim.esm"), "World"),
        });
        
        var str = await GetResults(async (k, w) =>
        {
            var meta = new SerializationMetaData(GameRelease.SkyrimSE);
            await SerializationHelper.WriteGroup(w, objs, "MyGroup", meta, k,
                async (w, g, k, m) =>
                {
                    k.WriteBool(w, nameof(TestGroup.SomeGroupField), true, default);
                },
                new WriteAsync<TWriterKernel, TWriter, TestMajorRecord>(async (w, i, k, m) =>
                {
                    k.WriteFormKey(w, nameof(TestMajorRecord.FormKey), i.FormKey, default);
                    k.WriteString(w, "String", i.String, default);
                    k.WriteString(w, "EditorID", i.EditorID, default);
                }));
        });
        await TestHelper.VerifyString(str);

        await CheckReadResults(str,
            new ReadResults<TestGroup>(
                "MyGroup",
                async (kernel, reader) =>
                {
                    var g = new TestGroup();
                    var meta = new SerializationMetaData(GameRelease.SkyrimSE);
                    await SerializationHelper.ReadIntoGroup(reader, g, meta, kernel,
                        groupReader: async (r, o, k, m, n) =>
                        {
                            switch (n)
                            {
                                case nameof(TestGroup.SomeGroupField):
                                    o.SomeGroupField = k.ReadBool(r).StripNull(nameof(TestGroup.SomeGroupField));
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        },
                        itemReader: async (r, k, m) =>
                        {
                            FormKey fk = default;
                            string s = string.Empty;
                            while (k.TryGetNextField(r, out var name))
                            {
                                switch (name)
                                {
                                    case "FormKey":
                                        fk = k.ReadFormKey(r) ?? throw new NullReferenceException();
                                        break;
                                    case "String":
                                        s = k.ReadString(r).StripNull(nameof(String));
                                        break;
                                    default:
                                        throw new DataMisalignedException();
                                }
                            }

                            return new TestMajorRecord(fk, s);
                        });
                    return g;
                },
                objs));
    }
    
    [Theory, DefaultAutoData]
    public async Task GroupFolder(
        DirectoryPath existingDir,
        IFileSystem fileSystem)
    {
        var objs = new TestGroup()
        {
            SomeGroupField = true,
        };
        objs.Records.SetTo(new List<TestMajorRecord>()
        {
            new TestMajorRecord(Plugins.FormKey.Factory("123456:Skyrim.esm"), "Hello")
            {
                EditorID = "EditorID01"
            },
            new TestMajorRecord(Plugins.FormKey.Factory("123457:Skyrim.esm"), "World")
            {
                EditorID = "EditorID02"
            },
        });
        
        var streamPackage = new StreamPackage(null!, existingDir);
        var meta = new SerializationMetaData(
            GameRelease.SkyrimSE, new InlineWorkDropoff(), fileSystem,
            NormalFileStreamCreator.Instance, CancellationToken.None);
        await SerializationHelper.WriteFilePerRecord(
            streamPackage,
            objs,
            "MyGroup",
            meta,
            MutagenSerializationWriterKernel<TWriterKernel, TWriter>.Instance,
            async (w, g, k, m) =>
            {
                k.WriteBool(w, nameof(TestGroup.SomeGroupField), true, default);
            },
            (g, m) => true,
            new WriteAsync<TWriterKernel, TWriter, TestMajorRecord>(async (w, i, k, m) =>
            {
                k.WriteFormKey(w, nameof(TestMajorRecord.FormKey), i.FormKey, default);
                k.WriteString(w, "String", i.String, default);
                k.WriteString(w, "EditorID", i.EditorID, default);
            }),
            withNumbering: true);
        
        await TestHelper.VerifyFileSystem(fileSystem);

        var g = new TestGroup();
        await SerializationHelper.ReadFilePerRecord<TReaderKernel, TReader, TestGroup, TestMajorRecord>(
            new StreamPackage(null!, existingDir),
            g,
            "MyGroup",
            meta,
            new TReaderKernel(),
            groupReader: async (r, o, k, m, n) =>
            {
                switch (n)
                {
                    case nameof(TestGroup.SomeGroupField):
                        o.SomeGroupField = k.ReadBool(r).StripNull(nameof(TestGroup.SomeGroupField));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            },
            itemReader: async (r, k, m) =>
            {
                FormKey fk = default;
                string s = string.Empty;
                string? edid = null;
                while (k.TryGetNextField(r, out var name))
                {
                    switch (name)
                    {
                        case "FormKey":
                            fk = k.ReadFormKey(r) ?? throw new NullReferenceException();
                            break;
                        case "String":
                            s = k.ReadString(r).StripNull(nameof(String));
                            break;
                        case "EditorID":
                            edid = k.ReadString(r);
                            break;
                        default:
                            throw new DataMisalignedException();
                    }
                }
        
                return new TestMajorRecord(fk, s)
                {
                    EditorID = edid
                };
            });

        g.Equals(objs).Should().BeTrue();
    }
    
    [Theory, DefaultAutoData]
    public async Task BlocksListGroup(
        DirectoryPath existingDir,
        IFileSystem fileSystem)
    {
        var objs = new TestBlockGroup()
        {
            SomeValue = 123,
            Blocks = new List<Block>()
            {
                new Block()
                {
                    BlockNumber = 456,
                    SomeValue = "Hello",
                    SubBlocks = new List<SubBlock>()
                    {
                        new SubBlock()
                        {
                            BlockNumber = 279,
                            SomeValue = "EmptySubBlock",
                        },
                        new SubBlock()
                        {
                            BlockNumber = 987,
                            SomeValue = "World",
                            Records = new List<TestMajorRecord>()
                            {
                                new TestMajorRecord(Plugins.FormKey.Factory("123456:Skyrim.esm"), "Hello World")
                            }
                        },
                    }
                },
                new Block()
                {
                    BlockNumber = 742,
                    SomeValue = "EmptyBlock",
                }
            }
        };
        
        var meta = new SerializationMetaData(
            GameRelease.SkyrimSE, new InlineWorkDropoff(), fileSystem, 
            NormalFileStreamCreator.Instance, CancellationToken.None);
        var streamPackage = new StreamPackage(null!, existingDir);
        await SerializationHelper.AddBlocksToWork(
            streamPackage,
            objs,
            "MyBlocks",
            blockRetriever: b => b.Blocks,
            subBlockRetriever: b => b.SubBlocks,
            majorRetriever: b => b.Records,
            blockNumberRetriever: b => b.BlockNumber,
            subBlockNumberRetriever: b => b.BlockNumber,
            metaData: meta,
            kernel: MutagenSerializationWriterKernel<TWriterKernel, TWriter>.Instance,
            metaWriter: async (w, o, k, m) =>
            {
                k.WriteInt32(w, nameof(TestBlockGroup.SomeValue), o.SomeValue, default);
            },
            (g, m) => true,
            blockWriter: async (w, o, k, m) =>
            {
                k.WriteString(w, nameof(Block.SomeValue), o.SomeValue, default);
            },
            (g, m) => true,
            subBlockWriter: async (w, o, k, m) =>
            {
                k.WriteString(w, nameof(SubBlock.SomeValue), o.SomeValue, default);
            },
            (g, m) => true,
            majorWriter: async (w, o, k, m) =>
            {
                k.WriteString(w, nameof(TestMajorRecord.String), o.String, default);
                k.WriteFormKey(w, nameof(TestMajorRecord.FormKey), o.FormKey, default);
            },
            withNumbering: true);
        
        await TestHelper.VerifyFileSystem(fileSystem);
        
        var g = new TestBlockGroup();
        await SerializationHelper.ReadFilePerRecordIntoBlocks<TReaderKernel, TReader, TestBlockGroup, Block, SubBlock, TestMajorRecord>(
            streamPackage,
            g, 
            "MyBlocks",
            meta, new TReaderKernel(),
            groupReader: async (r, o, k, m, n) =>
            {
                switch (n)
                {
                    case nameof(TestBlockGroup.SomeValue):
                        o.SomeValue = k.ReadInt32(r).StripNull(nameof(TestBlockGroup.SomeValue));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            },
            groupSetter: (b, blocks) =>
            {
                b.Blocks.SetTo(blocks);
            },
            blockReader: async (r, o, k, m, n) =>
            {
                switch (n)
                {
                    case nameof(Block.SomeValue):
                        o.SomeValue = k.ReadString(r).StripNull(nameof(Block.SomeValue));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            },
            blockSet: (b, blockNum, subBlocks) =>
            {
                b.BlockNumber = blockNum;
                b.SubBlocks.SetTo(subBlocks);
            },
            subBlockReader: async (r, o, k, m, n) =>
            {
                switch (n)
                {
                    case nameof(SubBlock.SomeValue):
                        o.SomeValue = k.ReadString(r).StripNull(nameof(SubBlock.SomeValue));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            },
            subBlockSet: (b, blockNum, recs) =>
            {
                b.BlockNumber = blockNum;
                b.Records.SetTo(recs);
            },
            majorReader: async (r, k, m) =>
            {
                FormKey fk = default;
                string s = string.Empty;
                while (k.TryGetNextField(r, out var name))
                {
                    switch (name)
                    {
                        case "FormKey":
                            fk = k.ReadFormKey(r) ?? throw new NullReferenceException();
                            break;
                        case "String":
                            s = k.ReadString(r).StripNull(nameof(String));
                            break;
                        default:
                            throw new DataMisalignedException();
                    }
                }
    
                return new TestMajorRecord(fk, s);
            });

        g.Equals(objs).Should().BeTrue();
    }

    
    [Theory, DefaultAutoData]
    public async Task XYBlocksListGroup(
        DirectoryPath existingDir,
        IFileSystem fileSystem)
    {
        var group = new TestXYBlockGroup()
        {
            SomeValue = 936,
            Records = new List<TestXYRecord>()
            {
                new TestXYRecord(Plugins.FormKey.Factory("556677:Skyrim.esm"), "RecordData")
                {
                    SomeValue = 123,
                    Blocks = new List<XYBlock>()
                    {
                        new XYBlock()
                        {
                            BlockNumberX = 456,
                            BlockNumberY = 654,
                            SomeValue = "Hello",
                            SubBlocks = new List<XYSubBlock>()
                            {
                                new XYSubBlock()
                                {
                                    BlockNumberX = 279,
                                    BlockNumberY = 972,
                                    SomeValue = "EmptySubBlock",
                                },
                                new XYSubBlock()
                                {
                                    BlockNumberX = 987,
                                    BlockNumberY = 789,
                                    SomeValue = "World",
                                    Records = new List<TestMajorRecord>()
                                    {
                                        new TestMajorRecord(Plugins.FormKey.Factory("123456:Skyrim.esm"), "Hello World")
                                    }
                                },
                            }
                        },
                        new XYBlock()
                        {
                            BlockNumberX = 742,
                            BlockNumberY = 247,
                            SomeValue = "EmptyBlock",
                        }
                    }
                }
            }
        };
        
        var meta = new SerializationMetaData(
            GameRelease.SkyrimSE, new InlineWorkDropoff(), fileSystem,
            NormalFileStreamCreator.Instance, CancellationToken.None);
        var streamPackage = new StreamPackage(null!, existingDir);
        await SerializationHelper.AddXYBlocksToWork(
            streamPackage,
            group,
            "Worldspaces",
            topRecordRetriever: b => b.Records,
            blockRetriever: b => b.Blocks,
            subBlockRetriever: b => b.SubBlocks,
            majorRetriever: b => b.Records,
            blockNumberRetriever: b => new P2Int16(b.BlockNumberX, b.BlockNumberY),
            subBlockNumberRetriever: b => new P2Int16(b.BlockNumberX, b.BlockNumberY),
            metaData: meta,
            kernel: MutagenSerializationWriterKernel<TWriterKernel, TWriter>.Instance,
            groupWriter: async (w, o, k, m) =>
            {
                k.WriteInt32(w, nameof(TestXYBlockGroup.SomeValue), o.SomeValue, default);
            },
            (g, m) => true,
            topRecordWriter: async (w, o, k, m) =>
            {
                k.WriteInt32(w, nameof(TestXYRecord.SomeValue), o.SomeValue, default);
                k.WriteString(w, nameof(TestMajorRecord.String), o.String, default);
            },
            (g, m) => true,
            blockWriter: async (w, o, k, m) =>
            {
                k.WriteString(w, nameof(XYBlock.SomeValue), o.SomeValue, default);
            },
            (g, m) => true,
            subBlockWriter: async (w, o, k, m) =>
            {
                k.WriteString(w, nameof(XYSubBlock.SomeValue), o.SomeValue, default);
            },
            (g, m) => true,
            majorWriter: async (w, o, k, m) =>
            {
                k.WriteString(w, nameof(TestMajorRecord.String), o.String, default);
                k.WriteFormKey(w, nameof(TestMajorRecord.FormKey), o.FormKey, default);
            },
            withNumbering: true);
        
        await TestHelper.VerifyFileSystem(fileSystem);

        var outGroup = new TestXYBlockGroup();
        await SerializationHelper.ReadIntoXYBlocks<TReaderKernel, TReader, TestXYBlockGroup, TestXYRecord, XYBlock, XYSubBlock, TestMajorRecord>(
            streamPackage,
            outGroup,
            "Worldspaces",
            meta,
            new TReaderKernel(),
            groupReader: async (r, o, k, m, n) =>
            {
                switch (n)
                {
                    case nameof(TestXYBlockGroup.SomeValue):
                        o.SomeValue = k.ReadInt32(r).StripNull(nameof(TestXYBlockGroup.SomeValue));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            },
            objReader: async (r, k, m) =>
            {
                FormKey fk = default;
                string s = string.Empty;
                int i = default;
                while (k.TryGetNextField(r, out var name))
                {
                    switch (name)
                    {
                        case "FormKey":
                            fk = k.ReadFormKey(r) ?? throw new NullReferenceException();
                            break;
                        case nameof(TestXYRecord.String):
                            s = k.ReadString(r).StripNull(nameof(TestXYRecord.String));
                            break;
                        case nameof(TestXYRecord.SomeValue):
                            i = k.ReadInt32(r) ?? 0;
                            break;
                        default:
                            throw new DataMisalignedException();
                    }
                }
    
                return new TestXYRecord(fk, s) { SomeValue = i };
            },
            blockReader: async (r, o, k, m, n) =>
            {
                switch (n)
                {
                    case nameof(XYBlock.SomeValue):
                        o.SomeValue = k.ReadString(r).StripNull(nameof(XYBlock.SomeValue));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            },
            blockSet: async (b, blockNum, subBlocks) =>
            {
                b.BlockNumberX = blockNum.X;
                b.BlockNumberY = blockNum.Y;
                b.SubBlocks.SetTo(subBlocks);
            },
            subBlockReader: async (r, o, k, m, n) =>
            {
                switch (n)
                {
                    case nameof(XYSubBlock.SomeValue):
                        o.SomeValue = k.ReadString(r).StripNull(nameof(XYSubBlock.SomeValue));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            },
            subBlockSet: async (b, blockNum, recs) =>
            {
                b.BlockNumberX = blockNum.X;
                b.BlockNumberY = blockNum.Y;
                b.Records.SetTo(recs);
            },
            majorReader: async (r, k, m) =>
            {
                FormKey fk = default;
                string s = string.Empty;
                while (k.TryGetNextField(r, out var name))
                {
                    switch (name)
                    {
                        case "FormKey":
                            fk = k.ReadFormKey(r) ?? throw new NullReferenceException();
                            break;
                        case "String":
                            s =  SerializationHelper.StripNull(k.ReadString(r), nameof(String));
                            break;
                        default:
                            throw new DataMisalignedException();
                    }
                }
    
                return new TestMajorRecord(fk, s);
            },
            groupSetter: (b, recs) =>
            {
                b.Records.SetTo(recs);
            },
            topRecordSetter: (b, blocks) =>
            {
                b.Blocks.SetTo(blocks);
            });

        outGroup.Equals(group).Should().BeTrue();
    }

    public enum SomeEnum
    {
        First,
        Second,
        Third
    }

    [Fact]
    public async Task Enum()
    {
        await DoPrimitiveTest<SomeEnum?>(
            "Enum",
            (k, w, name, item, def) => k.WriteEnum<SomeEnum>(w, name, item, def),
            (k, r) => k.ReadEnum<SomeEnum>(r),
            SomeEnum.Second,
            default(SomeEnum),
            SomeEnum.Second);
    }

    [Flags]
    public enum SomeFlagsEnum
    {
        First = 0x01,
        Second = 0x02,
        Third = 0x04
    }

    [Fact]
    public async Task FlagsEnum()
    {
        await DoPrimitiveTest<SomeFlagsEnum?>(
            "FlagsEnum",
            (k, w, name, item, def) => k.WriteEnum<SomeFlagsEnum>(w, name, item, def),
            (k, r) => k.ReadEnum<SomeFlagsEnum>(r),
            SomeFlagsEnum.Second,
            default(SomeFlagsEnum),
            SomeFlagsEnum.Second,
            SomeFlagsEnum.First | SomeFlagsEnum.Third,
            SomeFlagsEnum.First | (SomeFlagsEnum)0x10);
    }
}