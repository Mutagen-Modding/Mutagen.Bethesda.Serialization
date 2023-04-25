using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task WriteGroup<TKernel, TWriteObject, TGroup, TObject>(
        TWriteObject writer,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        WriteAsync<TKernel, TWriteObject, TGroup> groupWriter,
        WriteAsync<TKernel, TWriteObject, TObject> itemWriter)
        where TGroup : class, IReadOnlyCollection<TObject>
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        if (group.Count == 0) return;
        await kernel.WriteLoqui(writer, fieldName, group, metaData, async (w, g, k, m) =>
        {
            await groupWriter(w, g, k, m);
            k.StartListSection(w, "Records");
            foreach (var recordGetter in g)
            {
                await k.WriteLoqui(w, null, recordGetter, m, itemWriter);
            }
            k.EndListSection(w);
        });
    }
    
    public static async Task ReadIntoGroup<TKernel, TReadObject, TGroup, TObject>(
        TReadObject reader,
        TGroup group,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        ReadAsync<TKernel, TReadObject, TObject> itemReader)
        where TGroup : class, IGroup<TObject>
        where TObject : class, IMajorRecord
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        group.Clear();
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "Records":
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = await itemReader(reader, kernel, metaData);
                        group.Add(item);
                    }
                    kernel.EndListSection(reader);
                    break;
                default:
                    await groupReader(reader, group, kernel, metaData, name);
                    break;
            }
        }
    }
    
    public static async Task ReadIntoListGroup<TKernel, TReadObject, TGroup, TObject>(
        TReadObject reader,
        TGroup group,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        ReadAsync<TKernel, TReadObject, TObject> itemReader)
        where TGroup : class, IListGroup<TObject>
        where TObject : class, ILoquiObject
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        group.Clear();
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "Records":
                    kernel.StartListSection(reader);
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = await itemReader(reader, kernel, metaData);
                        group.Add(item);
                    }
                    kernel.EndListSection(reader);
                    break;
                default:
                    await groupReader(reader, group, kernel, metaData, name);
                    break;
            }
        }
    }
}