using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task<GenderedItem<TObject>> ReadGenderedItem<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TKernel kernel,
        SerializationMetaData metaData,
        GenderedItem<TObject> ret,
        ReadNamedAsync<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "Male":
                    ret.Male = await itemReader(reader, kernel, metaData, "Male");
                    break;
                case "Female":
                    ret.Female = await itemReader(reader, kernel, metaData, "Female");
                    break;
            }
        }

        return ret;
    }

    public static async Task WriteGendered<TKernel, TWriteObject, TObject>(
        TWriteObject writer,
        string? fieldName, 
        IGenderedItemGetter<TObject> item,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TObject> itemWriter)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        await kernel.WriteLoqui(writer, fieldName, item, metaData, async (w, o, k, m) =>
        {
            k.WriteWithName(w, "Male", o.Male, m, itemWriter);
            k.WriteWithName(w, "Female", o.Female, m, itemWriter);
        });
    }
}