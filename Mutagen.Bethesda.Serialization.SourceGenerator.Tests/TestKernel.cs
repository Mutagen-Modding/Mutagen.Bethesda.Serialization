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

    public void WriteModKey(TestWriter writer, string? fieldName, ModKey? modKey)
    {
        throw new NotImplementedException();
    }

    public void WriteFormKey(TestWriter writer, string? fieldName, FormKey? formKey)
    {
        throw new NotImplementedException();
    }

    public void WriteRecordType(TestWriter writer, string? fieldName, RecordType? recordType)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int(TestWriter writer, string? fieldName, P2Int? p2)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Int16(TestWriter writer, string? fieldName, P2Int16? p2)
    {
        throw new NotImplementedException();
    }

    public void WriteP2Float(TestWriter writer, string? fieldName, P2Float? p3)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Float(TestWriter writer, string? fieldName, P3Float? p3)
    {
        throw new NotImplementedException();
    }

    public void WriteP3UInt8(TestWriter writer, string? fieldName, P3UInt8? p3)
    {
        throw new NotImplementedException();
    }

    public void WriteP3Int16(TestWriter writer, string? fieldName, P3Int16? p3)
    {
        throw new NotImplementedException();
    }

    public void WriteP3UInt16(TestWriter writer, string? fieldName, P3UInt16? p3)
    {
        throw new NotImplementedException();
    }

    public void WritePercent(TestWriter writer, string? fieldName, Percent? percent)
    {
        throw new NotImplementedException();
    }

    public void WriteColor(TestWriter writer, string? fieldName, Color? color)
    {
        throw new NotImplementedException();
    }

    public void WriteTranslatedString(TestWriter writer, string? fieldName, ITranslatedStringGetter? translatedString)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(TestWriter writer, string? fieldName, ReadOnlyMemorySlice<byte>? bytes)
    {
        throw new NotImplementedException();
    }

    public void WriteEnum<TEnum>(TestWriter writer, string? fieldName, TEnum? item) where TEnum : struct, Enum, IConvertible
    {
        throw new NotImplementedException();
    }

    public TestWriter StartListSection(TestWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void EndListSection()
    {
        throw new NotImplementedException();
    }

    public TestWriter StartDictionarySection(TestWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void StopDictionarySection()
    {
        throw new NotImplementedException();
    }

    public TestWriter StartDictionaryItem(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryItem()
    {
        throw new NotImplementedException();
    }

    public TestWriter StartDictionaryKey(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryKey()
    {
        throw new NotImplementedException();
    }

    public TestWriter StartDictionaryValue(TestWriter writer)
    {
        throw new NotImplementedException();
    }

    public void StopDictionaryValue()
    {
        throw new NotImplementedException();
    }

    public TestWriter StartArray2dSection(TestWriter writer, string? fieldName)
    {
        throw new NotImplementedException();
    }

    public void StopArray2dSectionSection()
    {
        throw new NotImplementedException();
    }

    public TestWriter StartArray2dItem(TestWriter writer, int x, int y)
    {
        throw new NotImplementedException();
    }

    public void StopArray2dItem()
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

    TestWriter ISerializationWriterKernel<TestWriter>.GetNewObject(Stream stream)
    {
        throw new NotImplementedException();
    }
}