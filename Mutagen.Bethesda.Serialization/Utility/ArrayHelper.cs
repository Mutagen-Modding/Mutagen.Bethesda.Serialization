using Noggog;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public static async Task ReadIntoArray<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TObject[] arr,
        TKernel kernel,
        SerializationMetaData metaData,
        ReadAsync<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        int i = 0;
        kernel.StartListSection(reader);
        while (i < arr.Length && kernel.TryHasNextItem(reader))
        {
            var item = await itemReader(reader, kernel, metaData);
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
    
    public static async Task<TObject[]> ReadArray<TKernel, TReadObject, TObject>(
        TReadObject reader,
        TKernel kernel,
        SerializationMetaData metaData,
        ReadAsync<TKernel, TReadObject, TObject> itemReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        kernel.StartListSection(reader);
        var list = new List<TObject>();
        while (kernel.TryHasNextItem(reader))
        {
            var item = await itemReader(reader, kernel, metaData);
            list.Add(item);
        }
        
        kernel.EndListSection(reader);
        return list.ToArray();
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
}