using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;
using YamlDotNet.RepresentationModel;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class YamlSerializationWriterKernel : ISerializationWriterKernel<YamlNode>
{
    public YamlNode GetNewObject()
    {
        throw new NotImplementedException();
    }

    public void WriteString(YamlNode writer, string? fieldName, string? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt8(YamlNode writer, string? fieldName, sbyte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt16(YamlNode writer, string? fieldName, short? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt32(YamlNode writer, string? fieldName, int? item)
    {
        throw new NotImplementedException();
    }

    public void WriteInt64(YamlNode writer, string? fieldName, long? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt8(YamlNode writer, string? fieldName, byte? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt16(YamlNode writer, string? fieldName, ushort? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt32(YamlNode writer, string? fieldName, uint? item)
    {
        throw new NotImplementedException();
    }

    public void WriteUInt64(YamlNode writer, string? fieldName, ulong? item)
    {
        throw new NotImplementedException();
    }

    public void WriteFormKey(YamlNode writer, string? fieldName, FormKey? formKey)
    {
        throw new NotImplementedException();
    }

    public void WriteColor(YamlNode writer, string? fieldName, Color? color)
    {
        throw new NotImplementedException();
    }

    public void WriteTranslatedString(YamlNode writer, string? fieldName, ITranslatedStringGetter? translatedString)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(YamlNode writer, string? fieldName, ReadOnlyMemorySlice<byte>? bytes)
    {
        throw new NotImplementedException();
    }

    public IDisposable StartListSection(YamlNode writer, string? fieldName)
    {
        throw new NotImplementedException();
    }
}