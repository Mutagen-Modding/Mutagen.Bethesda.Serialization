using System.IO.Abstractions;
using System.Reactive.Disposables;
using System.Text;
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

    // ext4 holds a file name of at most 255 bytes, and NTFS one of 255 UTF-16 units, which a name of
    // at most 255 UTF-8 bytes never exceeds.
    private const int MaxFileNameBytes = 255;

    public static string RecordFileNameProvider(
        IMajorRecordGetter recordGetter, 
        string expectedExtension,
        int? number)
    {
        // A file's xxhash sibling swaps its extension for one that may be longer, so its name must fit too.
        var longestExtension = expectedExtension.Length == 0 || expectedExtension.Length >= XxHashShortCircuitExporter.HashExtension.Length
            ? expectedExtension
            : XxHashShortCircuitExporter.HashExtension;
        return RecordName(recordGetter, expectedExtension, longestExtension, number);
    }
    
    public static string RecordNameProvider(
        IMajorRecordGetter recordGetter,
        int? number)
    {
        return RecordName(recordGetter, string.Empty, string.Empty, number);
    }

    private static string RecordName(IMajorRecordGetter recordGetter, string extension, string longestExtension, int? number)
    {
        var formKey = recordGetter.FormKey.ToFilesafeString();
        var edid = recordGetter.EditorID;
        if (edid == null) return DecorateWithNumber($"{formKey}{extension}", number);
        var tail = $" - {formKey}";
        var budget = MaxFileNameBytes - Encoding.UTF8.GetByteCount(DecorateWithNumber($"{tail}{longestExtension}", number));
        return DecorateWithNumber($"{Utf8PrefixWithin(edid, budget)}{tail}{extension}", number);
    }

    private static string Utf8PrefixWithin(string str, int maxBytes)
    {
        var bytes = 0;
        var length = 0;
        foreach (var rune in str.EnumerateRunes())
        {
            bytes += rune.Utf8SequenceLength;
            if (bytes > maxBytes) break;
            length += rune.Utf16SequenceLength;
        }
        return str[..length];
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