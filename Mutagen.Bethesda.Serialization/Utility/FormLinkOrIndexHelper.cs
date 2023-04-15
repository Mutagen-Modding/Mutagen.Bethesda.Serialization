using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static void ReadFormLinkOrIndex<TKernel, TReadObject, TMajorGetter>(
        TReadObject reader,
        TKernel kernel,
        IFormLinkOrIndex<TMajorGetter> item,
        SerializationMetaData metaData)
        where TKernel : ISerializationReaderKernel<TReadObject>
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (item.UsesLink())
        {
            item.Link.SetTo(kernel.ReadFormKey(reader));
        }
        else
        {
            item.Index = kernel.ReadUInt32(reader);
        }
    }

    public static void WriteFormLinkOrIndex<TKernel, TWriteObject, TMajorGetter>(
        TWriteObject writer,
        string? fieldName, 
        IFormLinkOrIndexGetter<TMajorGetter> item,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (item.UsesLink())
        {
            kernel.WriteFormKey(writer, fieldName, item.Link.FormKey, FormKey.Null);
        }
        else
        {
            kernel.WriteUInt32(writer, fieldName, item.Index.Value, 0);
        }
    }
}