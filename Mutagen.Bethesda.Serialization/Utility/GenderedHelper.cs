using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static GenderedItem<TObject> ReadGenderedItem<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TKernel kernel,
        SerializationMetaData metaData,
        GenderedItem<TObject> ret,
        ReadNamed<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "Male":
                    ret.Male = itemReader(reader, kernel, metaData, "Male");
                    break;
                case "Female":
                    ret.Female = itemReader(reader, kernel, metaData, "Female");
                    break;
            }
        }

        return ret;
    }

    public static void WriteGendered<TKernel, TWriteObject, TObject>(
        TWriteObject writer,
        string? fieldName, 
        IGenderedItemGetter<TObject> item,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TObject> itemWriter)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteLoqui(writer, fieldName, item, metaData, (w, o, k, m) =>
        {
            k.WriteWithName(w, "Male", o.Male, m, itemWriter);
            k.WriteWithName(w, "Female", o.Female, m, itemWriter);
        });
    }
}