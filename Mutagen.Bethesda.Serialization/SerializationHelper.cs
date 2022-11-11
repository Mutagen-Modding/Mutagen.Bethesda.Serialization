using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization;

public static class SerializationHelper
{
    public static void WriteGroup<TKernel, TWriteObject, TGroup, TObject>(
        TWriteObject writer,
        TGroup group,
        string? fieldName,
        SerializationMetaData metaData,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        Write<TKernel, TWriteObject, TObject> itemWriter)
        where TGroup : class, IReadOnlyCollection<TObject>
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        if (group.Count == 0) return;
        kernel.WriteLoqui(writer, fieldName, group, metaData, (w, g, k, m) =>
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
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> itemReader)
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
                        var item = itemReader(reader, kernel, metaData);
                        group.Add(item);
                    }
                    kernel.EndListSection(reader);
                    break;
                default:
                    groupReader(reader, group, kernel, metaData, name);
                    break;
            }
        }
    }
    
    public static void ReadIntoListGroup<TKernel, TReadObject, TGroup, TObject>(
        TReadObject reader,
        TGroup group,
        SerializationMetaData metaData,
        TKernel kernel,
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader,
        Read<TKernel, TReadObject, TObject> itemReader)
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
                        var item = itemReader(reader, kernel, metaData);
                        group.Add(item);
                    }
                    kernel.EndListSection(reader);
                    break;
                default:
                    groupReader(reader, group, kernel, metaData, name);
                    break;
            }
        }
    }

    public static void ReadIntoArray<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TObject[] arr,
        TKernel kernel,
        SerializationMetaData metaData,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        int i = 0;
        kernel.StartListSection(reader);
        while (i < arr.Length && kernel.TryHasNextItem(reader))
        {
            var item = itemReader(reader, kernel, metaData);
            arr[i] = item;
            i++;
        }

        if (kernel.TryHasNextItem(reader))
        {
            throw new DataMisalignedException($"Array had more items than the max allowed: {arr.Length}");
        }

        if (i < arr.Length)
        {
            throw new DataMisalignedException($"Array was not filled entirely with items. {i} < {arr.Length}");
        }
        
        kernel.EndListSection(reader);
    }

    public static void ReadIntoSlice<TKernel, TReadObject, TObject>(
        TReadObject reader,
        MemorySlice<TObject> arr,
        TKernel kernel,
        SerializationMetaData metaData,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        int i = 0;
        kernel.StartListSection(reader);
        while (i < arr.Length && kernel.TryHasNextItem(reader))
        {
            var item = itemReader(reader, kernel, metaData);
            arr[i] = item;
            i++;
        }

        if (kernel.TryHasNextItem(reader))
        {
            throw new DataMisalignedException($"Array had more items than the max allowed: {arr.Length}");
        }

        if (i < arr.Length)
        {
            throw new DataMisalignedException($"Array was not filled entirely with items. {i} < {arr.Length}");
        }
        
        kernel.EndListSection(reader);
    }

    public static MemorySlice<TObject> ReadSlice<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TKernel kernel,
        SerializationMetaData metaData,
        Read<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        int i = 0;
        List<TObject> ret = new();
        kernel.StartListSection(reader);
        while (kernel.TryHasNextItem(reader))
        {
            var item = itemReader(reader, kernel, metaData);
            ret.Add(item);
            i++;
        }
        
        kernel.EndListSection(reader);

        return ret.ToArray();
    }

    public static T StripNull<T>(T? item, string name)
    {
        if (item == null)
        {
            throw new NullReferenceException($"{name} was null");
        }

        return item;
    }
}