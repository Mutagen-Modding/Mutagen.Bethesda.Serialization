using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class MutagenTestConverter 
    : IMutagenSerializationBootstrap<
        TestKernel, TestReader,
        TestKernel, TestWriter>
{
    public static readonly MutagenTestConverter Instance = new();
    
    private MutagenTestConverter()
    {
    }
}

public class TestReader
{
}

public class TestWriter
{
}

public class TestKernel : ISerializationReaderKernel<TestReader>, ISerializationWriterKernel<TestWriter>
{
    TestReader ISerializationReaderKernel<TestReader>.GetNewObject(Stream stream)
    {
        throw new NotImplementedException();
    }

    public bool TryGetNextField(out string name)
    {
        throw new NotImplementedException();
    }

    public char ReadChar(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public void Finalize(Stream stream, TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void WriteChar(TestWriter writer, string? fieldName, char? item)
    {
        throw new NotImplementedException();
    }

    public void WriteBool(TestWriter writer, string? fieldName, bool? item)
    {
        throw new NotImplementedException();
    }

    public void WriteString(TestWriter writer, string? fieldName, string? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt8(TestWriter writer, string? fieldName, sbyte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt16(TestWriter writer, string? fieldName, short? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt32(TestWriter writer, string? fieldName, int? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt64(TestWriter writer, string? fieldName, long? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt8(TestWriter writer, string? fieldName, byte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt16(TestWriter writer, string? fieldName, ushort? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt32(TestWriter writer, string? fieldName, uint? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt64(TestWriter writer, string? fieldName, ulong? item)
    {
        throw new NotImplementedException();
    }

    public void WriteFloat(TestWriter writer, string? fieldName, float? item)
    {
        throw new NotImplementedException();
    }

    public void WriteModKey(TestWriter writer, string? fieldName, ModKey? item)
    {
        throw new NotImplementedException();
    }

    public void WriteFormKey(TestWriter writer, string? fieldName, FormKey? item)
    {
        throw new NotImplementedException();
    }

    public void WriteRecordType(TestWriter writer, string? fieldName, RecordType? item)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int(TestWriter writer, string? fieldName, P2Int? item)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int16(TestWriter writer, string? fieldName, P2Int16? item)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Float(TestWriter writer, string? fieldName, P2Float? item)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Float(TestWriter writer, string? fieldName, P3Float? item)
    {
        throw new NotImplementedException();
    }

    public void WriteP3UInt8(TestWriter writer, string? fieldName, P3UInt8? item)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Int16(TestWriter writer, string? fieldName, P3Int16? item)
    {
        throw new NotImplementedException();
    }

    public void WriteP3UInt16(TestWriter writer, string? fieldName, P3UInt16? item)
    {
        throw new NotImplementedException();
    }

    public void WritePercent(TestWriter writer, string? fieldName, Percent? item)
    {
        throw new NotImplementedException();
    }

    public void WriteColor(TestWriter writer, string? fieldName, Color? item)
    {
        throw new NotImplementedException();
    }

    public void WriteTranslatedString(TestWriter writer, string? fieldName, ITranslatedStringGetter? item)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(TestWriter writer, string? fieldName, ReadOnlyMemorySlice<byte>? item)
    {
        throw new NotImplementedException();
    }

    public void WriteEnum<TEnum>(TestWriter writer, string? fieldName, TEnum? item) where TEnum : struct, Enum, IConvertible
    {
        throw new NotImplementedException();
    }

    public void WriteWithName<TKernel, TObject>(MutagenSerializationWriterKernel<TKernel, TestWriter> kernel, TestWriter writer, string? fieldName,
        TObject item, SerializationMetaData serializationMetaData, Write<TKernel, TestWriter, TObject> writeCall) where TKernel : ISerializationWriterKernel<TestWriter>, new()
    {
        throw new NotImplementedException();
    }

    public void WriteLoqui<TKernel, TObject>(MutagenSerializationWriterKernel<TKernel, TestWriter> kernel, TestWriter writer, string? fieldName,
        TObject item, SerializationMetaData serializationMetaData, Write<TKernel, TestWriter, TObject> writeCall) where TKernel : ISerializationWriterKernel<TestWriter>, new()
    {
        throw new NotImplementedException();
    }

    public void StartListSection(TestWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void EndListSection(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StartDictionarySection(TestWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void EndDictionarySection(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StartDictionaryItem(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void EndDictionaryItem(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StartDictionaryKey(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void EndDictionaryKey(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StartDictionaryValue(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void EndDictionaryValue(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StartArray2dSection(TestWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void EndArray2dSection(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StartArray2dXSection(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void EndArray2dXSection(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StartArray2dYSection(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void EndArray2dYSection(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public bool ReadBool(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public TEnum ReadEnum<TEnum>(TestReader reader) where TEnum : struct, Enum, IConvertible
    {
        throw new NotImplementedException();
    }

    public string ReadString(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public sbyte ReadInt8(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public short ReadInt16(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public int ReadInt32(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public long ReadInt64(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public byte ReadUInt8(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public ushort ReadUInt16(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public uint ReadUInt32(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public ulong ReadUInt64(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public FormKey ReadFormKey(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public Color ReadColor(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public RecordType ReadRecordType(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public TranslatedString ReadTranslatedString(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemorySlice<byte> ReadBytes(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public void StartListSection(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public void EndListSection(TestReader reader)
    {
        throw new NotImplementedException();
    }

    public bool TryHasNextItem(TestReader reader)
    {
        throw new NotImplementedException();
    }

    TestWriter ISerializationWriterKernel<TestWriter>.GetNewObject(Stream stream)
    {
        throw new NotImplementedException();
    }
}