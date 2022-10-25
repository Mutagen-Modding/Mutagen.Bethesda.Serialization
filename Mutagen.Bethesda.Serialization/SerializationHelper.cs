using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization;

public static class SerializationHelper
{
    public static void WriteGroup<TKernel, TWriteObject, TGroup, TObject>(
        TWriteObject writer,
        TGroup group,
        string? fieldName,
        SerializationMetaData meta,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        Write<TKernel, TWriteObject, TObject> itemWriter)
        where TGroup : class, IReadOnlyCollection<TObject>
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        if (group.Count == 0) return;
        kernel.WriteLoqui(writer, fieldName, group, meta, (w, g, k, m) =>
        {
            groupWriter(w, g, k, m);
            k.StartListSection(w, "Records");
            foreach (var recordGetter in g)
            {
                k.WriteLoqui(w, null, recordGetter, m, itemWriter);
            }
            k.EndListSection(w);
        });
    }
    
    public static void ReadIntoGroup<TKernel, TReadObject, TGroup, TObject>(
        TReadObject reader,
        TGroup group,
        SerializationMetaData meta,
        TKernel kernel,
        ReadInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TGroup : class, IGroup<TObject>
        where TObject : class, IMajorRecordGetter
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        group.Clear();
        groupReader(reader, group, kernel, meta);
        // kernel.WriteLoqui(reader, fieldName, group, meta, (w, g, k, m) =>
        // {
        //     groupWriter(w, g, k, m);
        //     k.StartListSection(w, "Records");
        //     foreach (var recordGetter in g)
        //     {
        //         k.WriteLoqui(w, null, recordGetter, m, itemWriter);
        //     }
        //     k.EndListSection(w);
        // });
    }
}