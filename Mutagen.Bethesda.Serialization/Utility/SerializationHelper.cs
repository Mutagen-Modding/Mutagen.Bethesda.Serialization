using System.IO.Abstractions;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Streams;
using Noggog;
using Noggog.IO;

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

    public static string RecordDataFileNameWithoutExtension => "RecordData";
    
    public static string RecordDataFileName(string expectedExtension)
    {
        return $"{RecordDataFileNameWithoutExtension}{expectedExtension}";
    }

    public static string TypicalGroupFileName(string expectedExtension)
    {
        return $"GroupRecordData{expectedExtension}";
    }

    private static string EdidStringProcessor(IMajorRecordGetter recordGetter)
    {
        var edid = recordGetter.EditorID;
        if (edid == null) return string.Empty;
        return $"{edid} - ";
    }
    
    public static string RecordFileNameProvider(
        IMajorRecordGetter recordGetter, 
        string expectedExtension,
        int? number)
    {
        return DecorateWithNumber($"{EdidStringProcessor(recordGetter)}{recordGetter.FormKey.ToFilesafeString()}{expectedExtension}", number);
    }
    
    public static string RecordNameProvider(
        IMajorRecordGetter recordGetter,
        int? number)
    {
        return DecorateWithNumber($"{EdidStringProcessor(recordGetter)}{recordGetter.FormKey.ToFilesafeString()}", number);
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
        if (metaData.FileSystem.File.Exists(path))
        {
            await metaData.WorkDropoff.EnqueueAndWait(() =>
            {
                using var groupStream = metaData.FileSystem.File.OpenRead(path);

                var reader = kernel.GetNewObject(streamPackage with { Stream = groupStream });
                while (kernel.TryGetNextField(reader, out var name))
                {
                    groupReader(reader, group, kernel, metaData, name);
                }
            });
        }
        return path;
    }
    
    public static T StripNull<T>(this T? item, string name)
        where T : class
    {
        if (item == null)
        {
            throw new NullReferenceException($"{name} was null");
        }

        return item;
    }

    public static T StripNull<T>(this T? item, string name)
        where T : struct
    {
        if (item == null)
        {
            throw new NullReferenceException($"{name} was null");
        }

        return item.Value;
    }

    private static void ExtractMetaInternal<TReadUnit, TMeta>(
        IFileSystem fileSystem,
        string modKeyPath,
        string path,
        ICreateStream streamCreator,
        ISerializationReaderKernel<TReadUnit> kernel,
        TMeta? extraMeta,
        ReadInto<ISerializationReaderKernel<TReadUnit>, TReadUnit, TMeta>? metaReader,
        CancellationToken cancel,
        out ModKey modKey,
        out GameRelease? release)
    {
        cancel.ThrowIfCancellationRequested();
        if (!fileSystem.File.Exists(path))
        {
            throw new FileNotFoundException("Could not find file to parse", path);
        }
        
        ModKey? potentialModKey = null;
        GameRelease? potentialRelease = null;

        if (ModKey.TryFromFileName(Path.GetFileName(modKeyPath), out var mk))
        {
            potentialModKey = mk;
        }

        cancel.ThrowIfCancellationRequested();
        using (var stream = streamCreator.GetStreamFor(fileSystem, path, write: false))
        {
            var reader = kernel.GetNewObject(new StreamPackage(stream, Path.GetDirectoryName(path)));

            bool keepLooking = true;
            while (keepLooking && kernel.TryGetNextField(reader, out var name))
            {
                cancel.ThrowIfCancellationRequested();
                switch (name)
                {
                    case "ModKey" when potentialModKey == null:
                        potentialModKey = kernel.ReadModKey(reader);
                        break;
                    case "GameRelease":
                        potentialRelease = kernel.ReadEnum<GameRelease>(reader);
                        if (potentialModKey != null)
                        {
                            keepLooking = false;
                        }
                        break;
                    default:
                        if (extraMeta != null && metaReader != null && name.Equals(extraMeta.GetType().Name))
                        {
                            metaReader(reader, extraMeta, kernel, new SerializationMetaData(cancel));
                        }
                        else
                        {
                            kernel.Skip(reader);
                        }
                        break;
                }
            }
        }

        if (potentialModKey == null)
        {
            throw new MalformedDataException($"Could not locate a {nameof(ModKey)} to use from path: {modKeyPath}");
        }

        modKey = potentialModKey.Value;
        release = potentialRelease;
    }
    
    public static void ExtractMeta<TReadUnit, TMeta>(
        IFileSystem fileSystem,
        string modKeyPath,
        string path,
        ICreateStream streamCreator,
        ISerializationReaderKernel<TReadUnit> kernel,
        TMeta? extraMeta,
        ReadInto<ISerializationReaderKernel<TReadUnit>, TReadUnit, TMeta>? metaReader,
        CancellationToken cancel,
        out ModKey modKey,
        out GameRelease release)
    {
        ExtractMetaInternal<TReadUnit, TMeta>(
            fileSystem: fileSystem,
            modKeyPath: modKeyPath,
            path: path,
            streamCreator: streamCreator,
            kernel: kernel,
            extraMeta: extraMeta,
            metaReader: metaReader,
            cancel: cancel,
            modKey: out modKey,
            release: out var releaseNullable);
        release = releaseNullable ?? throw new MalformedDataException($"Could not locate a {nameof(GameRelease)} to use from path: {path}");
    }
    
    public static void ExtractMeta<TReadUnit, TMeta>(
        IFileSystem fileSystem,
        string modKeyPath,
        string path,
        ICreateStream streamCreator,
        ISerializationReaderKernel<TReadUnit> kernel,
        TMeta? extraMeta,
        ReadInto<ISerializationReaderKernel<TReadUnit>, TReadUnit, TMeta>? metaReader,
        CancellationToken cancel,
        out ModKey modKey)
    {
        ExtractMetaInternal(
            fileSystem: fileSystem,
            modKeyPath: modKeyPath,
            path: path,
            streamCreator: streamCreator,
            kernel: kernel,
            extraMeta: extraMeta,
            metaReader: metaReader,
            cancel: cancel,
            modKey: out modKey,
            release: out _);
    }

    public static IDisposable GetStreamCreator(
        ICreateStream? inputCreateStream,
        IFileSystem fileSystem,
        DirectoryPath path,
        out ICreateStream createStream)
    {
        if (inputCreateStream != null && inputCreateStream is not NoPreferenceStreamCreator)
        {
            createStream = inputCreateStream;
            return Disposable.Empty;
        }

        var reported = new ReportedCleanupStreamCreateWrapper(fileSystem, path, NormalFileStreamCreator.Instance);
        createStream = reported;
        return reported;
    }
}