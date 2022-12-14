using System.Drawing;
using System.Text;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.KernelTests;

public abstract class AKernelTest<TWriterKernel, TWriter, TReaderKernel, TReader>
    where TWriterKernel : ISerializationWriterKernel<TWriter>, new()
    where TReaderKernel : ISerializationReaderKernel<TReader>, new()
{
    private async Task<string> GetResults(
        Action<MutagenSerializationWriterKernel<TWriterKernel, TWriter>, TWriter> toDo)
    {
        var kernel = MutagenSerializationWriterKernel<TWriterKernel, TWriter>.Instance;
        var memStream = new MemoryStream();
        var writerObj = kernel.GetNewObject(memStream);
        toDo(kernel, writerObj);
        kernel.Finalize(memStream, writerObj);
        if (writerObj is IDisposable disp) disp.Dispose();
        memStream.Position = 0;
        StreamReader reader = new StreamReader(memStream);
        var ret = await reader.ReadToEndAsync();
        return ret;
    }

    private T GetReadResults<T>(
        string str,
        string nickname,
        Func<TReaderKernel, TReader, T> toDo)
    {
        var kernel = new TReaderKernel();
        var readerObj = kernel.GetNewObject(new MemoryStream(Encoding.UTF8.GetBytes(str)));
        T obj = default;
        bool found = false;
        while (kernel.TryGetNextField(readerObj, out var name))
        {
            if (name == nickname)
            {
                obj = toDo(kernel, readerObj);
                found = true;
                break;
            }
            else
            {
                kernel.Skip(readerObj);
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
        void Check(TReaderKernel kernel, TReader reader);
    }

    record ReadResults<T>(string Name, Func<TReaderKernel, TReader, T> Reader, T Expected) : IReadResults
    {
        public void Check(TReaderKernel kernel, TReader reader)
        {
            var val = Reader(kernel, reader);
            if (Expected is IReadOnlyList<object> e)
            {
                var list = (IReadOnlyList<object>)val;
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
    
    private void CheckReadResults(
        string str,
        params IReadResults[] results)
    {
        var kernel = new TReaderKernel();
        var readerObj = kernel.GetNewObject(new MemoryStream(Encoding.UTF8.GetBytes(str)));
        var dict = results.ToDictionary(x => x.Name, x => x);
        while (kernel.TryGetNextField(readerObj, out var name))
        {
            if (!dict.TryGetValue(name, out var result))
            {
                throw new Exception();
            }
            result.Check(kernel, readerObj);
        }

        if (readerObj is IDisposable disp) disp.Dispose();
    }

    private async Task<string> GetPrimitiveWriteTest<T>(
        string nickName,
        Action<MutagenSerializationWriterKernel<TWriterKernel, TWriter>, TWriter, string?, T?, T?> callback,
        T nonDefaultToTest,
        params T[] items)
    {
        return await GetResults((k, w) =>
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
        Func<TReaderKernel, TReader, T> readCallback,
        Func<T?, T?, bool> equality,
        T nonDefaultToTest,
        params T[] items)
    {
        var str = await GetPrimitiveWriteTest(nickName, writeCallback, nonDefaultToTest, items);
        await TestHelper.VerifyString(str);
        CheckReadResults(
            str,
            new ReadResults<T>[]
            {
                new ReadResults<T>($"{nickName}1", readCallback, default(T)),
            }.Concat(items.Select((x, i) =>
            {
                var name = $"{nickName}{(2 + i)}";
                return new ReadResults<T>(name, readCallback, x);
            })).ToArray());
    }

    [Fact]
    public async Task Nothing()
    {
        var str = await GetResults((k, w) => { });
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
        await DoPrimitiveTest<string>(
            "String",
            (k, w, name, item, def) => k.WriteString(w, name, item, def),
            (k, r) => k.ReadString(r),
            "Hello",
            string.Empty,
            "Hello");
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
        var str = await GetResults((k, w) => { k.WriteFormKey(w, "FormKey", fk, default); });

        var kernel = new TReaderKernel();
        var readerObj = kernel.GetNewObject(new MemoryStream(Encoding.UTF8.GetBytes(str)));
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
        var str = await GetResults((k, w) =>
        {
            k.WriteLoqui<int>(w, "Loqui", 4, null, (w, o, k, m) =>
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
        var str = await GetResults((k, w) =>
        {
            k.StartListSection(w, "MyList");
            k.WriteString(w, null, "Hello", default);
            k.WriteString(w, null, "There", default);
            k.WriteString(w, null, "World", default);
            k.EndListSection(w);
            k.WriteInt8(w, "SomeInt", 4, default);
        });

        await TestHelper.VerifyString(str);

        CheckReadResults(str,
            new ReadResults<List<string>>(
                "MyList",
                (kernel, reader) =>
                {
                    var ret = new List<string>();
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = kernel.ReadString(reader);
                        ret.Add(item);
                    }

                    kernel.EndListSection(reader);
                    return ret;
                },
                new List<string>() { "Hello", "There", "World" }),
            new ReadResults<int?>("SomeInt", (k, r) => k.ReadUInt8(r), 4));
    }

    record SomeClass(int? Int, string String);

    [Fact]
    public async Task LoquiList()
    {
        var item1 = new SomeClass(4, "Hello");
        var item2 = new SomeClass(6, "World");

        var str = await GetResults((k, w) =>
        {
            var meta = new SerializationMetaData(GameRelease.SkyrimSE);
            k.StartListSection(w, "MyList");
            k.WriteLoqui(w, null, item1, meta, (subW, obj, subKernel, meta) =>
            {
                subKernel.WriteInt32(subW, "Int", obj.Int, default);
                subKernel.WriteString(subW, "String", obj.String, default);
            });
            k.WriteLoqui(w, null, item2, meta, (subW, obj, subKernel, meta) =>
            {
                subKernel.WriteInt32(subW, "Int", obj.Int, default);
                subKernel.WriteString(subW, "String", obj.String, default);
            });
            k.EndListSection(w);
            k.WriteInt8(w, "SomeInt", 4, default);
        });
        await TestHelper.VerifyString(str);

        CheckReadResults(str,
            new ReadResults<List<SomeClass>>(
                "MyList",
                (kernel, reader) =>
                {
                    var metaData = new SerializationMetaData(GameRelease.SkyrimSE);
                    var ret = new List<SomeClass>();
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = kernel.ReadLoqui(reader, metaData, static (r, k, m) =>
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
                                        s = k.ReadString(r);
                                        break;
                                    default:
                                        throw new DataMisalignedException();
                                }
                            }

                            return new SomeClass(i, s);
                        });
                        ret.Add(item);
                    }

                    kernel.EndListSection(reader);
                    return ret;
                },
                new List<SomeClass> { item1, item2 }),
            new ReadResults<int?>("SomeInt", (k, r) => k.ReadUInt8(r), 4));
    }

    [Fact]
    public async Task Dict()
    {
        var item1 = new SomeClass(1, "value");
        var item2 = new SomeClass(2, "value2");

        var str = await GetResults((k, w) =>
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

        CheckReadResults(str,
            new ReadResults<List<SomeClass>>(
                "MyDict", 
                (kernel, reader) =>
                {
                    var ret = new List<SomeClass>();
                    kernel.StartDictionarySection(reader);
                    while (kernel.TryHasNextDictionaryItem(reader))
                    {
                        kernel.StartDictionaryKey(reader);
                        int? k = kernel.ReadInt32(reader);
                        kernel.EndDictionaryKey(reader);
                        kernel.StartDictionaryValue(reader);
                        string val = kernel.ReadString(reader);
                        kernel.EndDictionaryValue(reader);
                        kernel.EndDictionaryItem(reader);
                        ret.Add(new SomeClass(k, val));
                    }

                    kernel.EndDictionarySection(reader);
                    return ret;
                },
                new List<SomeClass>() { item1, item2 }),
            new ReadResults<int?>("SomeInt", (k, r) => k.ReadUInt8(r), 4));
    }

    [Fact]
    public async Task Array2d()
    {
        var str = await GetResults((k, w) =>
        {
            k.StartArray2dSection(w, "MyArr");
            k.StartArray2dYSection(w);
            k.StartArray2dXSection(w);
            k.WriteInt8(w, null, 1, default);
            k.EndArray2dXSection(w);
            k.StartArray2dXSection(w);
            k.WriteInt8(w, null, 2, default);
            k.EndArray2dXSection(w);
            k.EndArray2dYSection(w);
            k.StartArray2dYSection(w);
            k.StartArray2dXSection(w);
            k.WriteInt8(w, null, 4, default);
            k.EndArray2dXSection(w);
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
                    kernel.StartArray2dXSection(reader);
                    ret.Add((x, y, kernel.ReadInt32(reader)));
                    kernel.EndArray2dXSection(reader);
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
                new KeyValuePair<Language, string>(Language.French, "Bonjour")));
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
        var str = await GetResults((k, w) =>
        {
            var objs = new List<SomeClass>()
            {
                new SomeClass(7, "Hello"),
                new SomeClass(10, "World"),
            };
            var meta = new SerializationMetaData(GameRelease.SkyrimSE);
            SerializationHelper.WriteGroup(w, objs, "MyGroup", meta, k,
                (w, g, k, m) => { k.WriteBool(w, "SomeGroupField", true, default); },
                new Write<TWriterKernel, TWriter, SomeClass>((w, i, k, m) =>
                {
                    k.WriteInt32(w, "Int", i.Int, default);
                    k.WriteString(w, "String", i.String, default);
                }));
        });
        await TestHelper.VerifyString(str);
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