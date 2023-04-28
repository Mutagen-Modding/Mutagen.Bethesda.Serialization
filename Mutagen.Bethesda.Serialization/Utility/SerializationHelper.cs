using Mutagen.Bethesda.Plugins.Records;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Utility;

public static partial class SerializationHelper
{
    public delegate void DeserializeIntoCall<TReadObject, TObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        TObject obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage;

    public static void DeserializeAllFieldsInto<TReadObject, TObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        TObject obj,
        SerializationMetaData metaData,
        DeserializeIntoCall<TReadObject, TObject> deserializeInto)
        where TReadObject : IContainStreamPackage
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            deserializeInto(reader, kernel, obj, metaData, name);
        }
    }
    
    private static string RecordDataFileName(string expectedExtension)
    {
        return $"RecordData{expectedExtension}";
    }

    private static string TypicalGroupFileName(string expectedExtension)
    {
        return $"GroupRecordData{expectedExtension}";
    }
    
    public static string RecordFileNameProvider(
        IMajorRecordGetter recordGetter, 
        string expectedExtension,
        int? number)
    {
        var edid = recordGetter.EditorID;
        return DecorateWithNumber($"{edid ?? recordGetter.FormKey.ToFilesafeString()}{expectedExtension}", number);
    }
    
    public static string RecordNameProvider(
        IMajorRecordGetter recordGetter,
        int? number)
    {
        var edid = recordGetter.EditorID;
        return DecorateWithNumber($"{edid ?? recordGetter.FormKey.ToFilesafeString()}", number);
    }

    public static string DecorateWithNumber(string str, int? number)
    {
        if (number == null) return str;
        return $"[{number}] {str}";
    }

    public static uint? TryGetNumber(ReadOnlySpan<char> str)
    {
        if (str.Length < 1) return null;
        if (str[0] != '[') return null;
        var index = str.IndexOf("] ");
        if (index == -1)
        {
            return null;
        }
        if (uint.TryParse(str.Slice(1, index - 1), out var n))
        {
            return n;
        }
        return null;
    }

    private static async Task<string> ReadPathToWork<TKernel, TReadObject, TGroup>(
        StreamPackage streamPackage,
        TGroup group,
        string fileName,
        SerializationMetaData metaData, 
        TKernel kernel, 
        ReadNamedInto<TKernel, TReadObject, TGroup> groupReader)
        where TKernel : ISerializationReaderKernel<TReadObject>
    {
        var path = Path.Combine(streamPackage.Path!, fileName);
        await metaData.WorkDropoff.EnqueueAndWait(() =>
        {
            if (metaData.FileSystem.File.Exists(path))
            {
                using var groupStream = metaData.FileSystem.File.OpenRead(path);

                var reader = kernel.GetNewObject(streamPackage with { Stream = groupStream });
                while (kernel.TryGetNextField(reader, out var name))
                {
                    groupReader(reader, group, kernel, metaData, name);
                }
            }
        });
        return path;
    }
    
    public static T StripNull<T>(T? item, string name)
        where T : class
    {
        if (item == null)
        {
            throw new NullReferenceException($"{name} was null");
        }

        return item;
    }

    public static T StripNull<T>(T? item, string name)
        where T : struct
    {
        if (item == null)
        {
            throw new NullReferenceException($"{name} was null");
        }

        return item.Value;
    }
}