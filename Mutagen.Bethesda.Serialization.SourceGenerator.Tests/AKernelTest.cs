﻿using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public abstract class AKernelTest<TWriterKernel, TWriter>
    where TWriterKernel : ISerializationWriterKernel<TWriter>, new()
{
    private async Task<string> GetResults(Action<TWriterKernel, TWriter> toDo)
    {
        var kernel = new TWriterKernel();
        var memStream = new MemoryStream();
        var obj = kernel.GetNewObject(memStream);
        toDo(kernel, obj);
        kernel.Finalize(memStream, obj);
        if (obj is IDisposable disp) disp.Dispose();
        memStream.Position = 0;
        StreamReader reader = new StreamReader(memStream);
        var ret = await reader.ReadToEndAsync();
        return ret;
    }
    
    private async Task<string> GetPrimitiveWriteTest<T>(
        Action<TWriterKernel, TWriter, string?, T?> callback,
        params T[] items)
    {
        return await GetResults((k, w) =>
        {
            int i = 0;
            callback(k, w, $"Name{i++}", default);
            foreach (var item in items)
            {
                callback(k, w, $"Name{i++}", item);
            }
        });
    }
    
    [Fact]
    public async Task Nothing()
    {
        var str = await GetResults((k, w) => { });
        await Verifier.Verify(str);
    }
    
    [Fact]
    public async Task Char()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<char?>(
            (k, w, name, item) => k.WriteChar(w, name, item),
            'c',
            (char)165));
    }
    
    [Fact]
    public async Task Bool()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<bool?>(
            (k, w, name, item) => k.WriteBool(w, name, item),
            true,
            false));
    }
    
    [Fact]
    public async Task String()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<string>(
            (k, w, name, item) => k.WriteString(w, name, item),
            string.Empty,
            "Hello"));
    }
    
    [Fact]
    public async Task Int8()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<sbyte?>(
            (k, w, name, item) => k.WriteInt8(w, name, item),
            -4,
            5,
            0,
            sbyte.MinValue,
            sbyte.MaxValue));
    }
    
    [Fact]
    public async Task Int16()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<short?>(
            (k, w, name, item) => k.WriteInt16(w, name, item),
            -4,
            5,
            0,
            short.MinValue,
            short.MaxValue));
    }
    
    [Fact]
    public async Task Int32()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<int?>(
            (k, w, name, item) => k.WriteInt32(w, name, item),
            -4,
            5,
            0,
            int.MinValue,
            int.MaxValue));
    }
    
    [Fact]
    public async Task Int64()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<long?>(
            (k, w, name, item) => k.WriteInt64(w, name, item),
            -4,
            5,
            0,
            long.MinValue,
            long.MaxValue));
    }
    
    [Fact]
    public async Task UInt8()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<byte?>(
            (k, w, name, item) => k.WriteUInt8(w, name, item),
            5,
            byte.MinValue,
            byte.MaxValue));
    }
    
    [Fact]
    public async Task UInt16()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<ushort?>(
            (k, w, name, item) => k.WriteUInt16(w, name, item),
            5,
            ushort.MinValue,
            ushort.MaxValue));
    }
    
    [Fact]
    public async Task UInt32()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<uint?>(
            (k, w, name, item) => k.WriteUInt32(w, name, item),
            5,
            uint.MinValue,
            uint.MaxValue));
    }
    
    [Fact]
    public async Task UInt64()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<ulong?>(
            (k, w, name, item) => k.WriteUInt64(w, name, item),
            5,
            ulong.MinValue,
            ulong.MaxValue));
    }
    
    [Fact]
    public async Task Float()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<float?>(
            (k, w, name, item) => k.WriteFloat(w, name, item),
            0,
            -5,
            5,
            float.MinValue,
            float.MaxValue));
    }
    
    [Fact]
    public async Task ModKey()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<ModKey?>(
            (k, w, name, item) => k.WriteModKey(w, name, item),
            Plugins.ModKey.Null,
            Plugins.ModKey.FromNameAndExtension("SomeMod.esp")));
    }
    
    [Fact]
    public async Task FormKey()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<FormKey?>(
            (k, w, name, item) => k.WriteFormKey(w, name, item),
            Plugins.FormKey.Null,
            Plugins.FormKey.Factory("123456:SomeMod.esp")));
    }
    
    [Fact]
    public async Task RecordType()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<RecordType?>(
            (k, w, name, item) => k.WriteRecordType(w, name, item),
            Plugins.RecordType.Null,
            new RecordType("TEST")));
    }
    
    [Fact]
    public async Task P2Int()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<P2Int?>(
            (k, w, name, item) => k.WriteP2Int(w, name, item),
            new P2Int(),
            new P2Int(1, -3),
            new P2Int(int.MaxValue, int.MaxValue),
            new P2Int(int.MinValue, int.MinValue)));
    }
    
    [Fact]
    public async Task P2Int16()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<P2Int16?>(
            (k, w, name, item) => k.WriteP2Int16(w, name, item),
            new P2Int16(),
            new P2Int16(1, -3),
            new P2Int16(short.MaxValue, short.MaxValue),
            new P2Int16(short.MinValue, short.MinValue)));
    }
    
    [Fact]
    public async Task P2Float()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<P2Float?>(
            (k, w, name, item) => k.WriteP2Float(w, name, item),
            new P2Float(),
            new P2Float(1, -3),
            new P2Float(float.MaxValue, float.MaxValue),
            new P2Float(float.MinValue, float.MinValue)));
    }
    
    [Fact]
    public async Task P3Float()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<P3Float?>(
            (k, w, name, item) => k.WriteP3Float(w, name, item),
            new P3Float(),
            new P3Float(1, -3, 0),
            new P3Float(float.MaxValue, float.MaxValue, float.MaxValue),
            new P3Float(float.MinValue, float.MinValue, float.MinValue)));
    }
    
    [Fact]
    public async Task P3UInt8()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<P3UInt8?>(
            (k, w, name, item) => k.WriteP3UInt8(w, name, item),
            new P3UInt8(),
            new P3UInt8(1, 3, 0),
            new P3UInt8(byte.MaxValue, byte.MaxValue, byte.MaxValue),
            new P3UInt8(byte.MinValue, byte.MinValue, byte.MinValue)));
    }
    
    [Fact]
    public async Task P3Int16()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<P3Int16?>(
            (k, w, name, item) => k.WriteP3Int16(w, name, item),
            new P3Int16(),
            new P3Int16(1, 3, 0),
            new P3Int16(short.MaxValue, short.MaxValue, short.MaxValue),
            new P3Int16(short.MinValue, short.MinValue, short.MinValue)));
    }
    
    [Fact]
    public async Task P3UInt16()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<P3UInt16?>(
            (k, w, name, item) => k.WriteP3UInt16(w, name, item),
            new P3UInt16(),
            new P3UInt16(1, 3, 0),
            new P3UInt16(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue),
            new P3UInt16(ushort.MinValue, ushort.MinValue, ushort.MinValue)));
    }
    
    [Fact]
    public async Task Percent()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<Percent?>(
            (k, w, name, item) => k.WritePercent(w, name, item),
            new Percent(),
            new Percent(0d),
            new Percent(0.5d),
            new Percent(1d)));
    }
    
    [Fact]
    public async Task Color()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<Color?>(
            (k, w, name, item) => k.WriteColor(w, name, item),
            new Color(),
            System.Drawing.Color.FromArgb(1, 2, 3, 4),
            System.Drawing.Color.FromArgb(1, 2, 3)));
    }
    
    [Fact]
    public async Task List()
    {
        var str =  await GetResults((k, w) =>
        {
            k.StartListSection(w, "MyList");
            k.WriteInt8(w, null, 1);
            k.WriteInt8(w, null, 2);
            k.WriteInt8(w, null, 3);
            k.EndListSection(w);
            k.WriteInt8(w, "SomeInt", 4);
        });
        await Verifier.Verify(str);
    }
    
    [Fact]
    public async Task Dict()
    {
        var str =  await GetResults((k, w) =>
        {
            k.StartDictionarySection(w, "MyDict");
            k.StartDictionaryItem(w);
            k.StartDictionaryKey(w);
            k.WriteInt8(w, null, 1);
            k.EndDictionaryKey(w);
            k.StartDictionaryValue(w);
            k.WriteString(w, null, "value");
            k.EndDictionaryValue(w);
            k.EndDictionaryItem(w);
            k.StartDictionaryItem(w);
            k.StartDictionaryKey(w);
            k.WriteInt8(w, null, 2);
            k.EndDictionaryKey(w);
            k.StartDictionaryValue(w);
            k.WriteString(w, null, "value2");
            k.EndDictionaryValue(w);
            k.EndDictionaryItem(w);
            k.EndDictionarySection(w);
            k.WriteInt8(w, "SomeInt", 4);
        });
        await Verifier.Verify(str);
    }
    
    [Fact]
    public async Task Array2d()
    {
        var str =  await GetResults((k, w) =>
        {
            k.StartArray2dSection(w, "MyArr");
            k.StartArray2dYSection(w);
            k.StartArray2dXSection(w);
            k.WriteInt8(w, null, 1);
            k.EndArray2dXSection(w);
            k.StartArray2dXSection(w);
            k.WriteInt8(w, null, 2);
            k.EndArray2dXSection(w);
            k.EndArray2dYSection(w);
            k.StartArray2dYSection(w);
            k.StartArray2dXSection(w);
            k.WriteInt8(w, null, 4);
            k.EndArray2dXSection(w);
            k.EndArray2dYSection(w);
            k.EndArray2dSection(w);
        });
        await Verifier.Verify(str);
    }
    
    [Fact]
    public async Task TranslatedString()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<ITranslatedStringGetter?>(
            (k, w, name, item) => k.WriteTranslatedString(w, name, item),
            new TranslatedString(Language.English),
            new TranslatedString(Language.English, default(string?)),
            new TranslatedString(Language.English, "Hello"),
            new TranslatedString(Language.English,
                new KeyValuePair<Language, string>(Language.English, "Hello"),
                new KeyValuePair<Language, string>(Language.French, "Bonjour")),
            new TranslatedString(Language.Spanish,
                new KeyValuePair<Language, string>(Language.English, "Hello"),
                new KeyValuePair<Language, string>(Language.French, "Bonjour"))));
    }
    
    [Fact]
    public async Task Bytes()
    {
        await Verifier.Verify(await GetPrimitiveWriteTest<ReadOnlyMemorySlice<byte>?>(
            (k, w, name, item) => k.WriteBytes(w, name, item),
            new ReadOnlyMemorySlice<byte>(),
            new ReadOnlyMemorySlice<byte>(Array.Empty<byte>()),
            new ReadOnlyMemorySlice<byte>(new byte[] { 1, 2, 3, 4, 5, 254, 255 })));
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
        await Verifier.Verify(await GetPrimitiveWriteTest<SomeEnum?>(
            (k, w, name, item) => k.WriteEnum<SomeEnum>(w, name, item),
            default(SomeEnum),
            SomeEnum.Second));
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
        await Verifier.Verify(await GetPrimitiveWriteTest<SomeFlagsEnum?>(
            (k, w, name, item) => k.WriteEnum<SomeFlagsEnum>(w, name, item),
            default(SomeFlagsEnum),
            SomeFlagsEnum.Second,
            SomeFlagsEnum.First | SomeFlagsEnum.Third,
            SomeFlagsEnum.First | (SomeFlagsEnum)0x10));
    }
}