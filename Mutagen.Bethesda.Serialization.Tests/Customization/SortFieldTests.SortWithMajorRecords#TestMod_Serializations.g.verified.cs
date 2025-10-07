//HintName: TestMod_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.Exceptions;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

#pragma warning disable CS1998 // No awaits used
#pragma warning disable CS0618 // Obsolete

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        IWorkDropoff? workDropoff,
        IFileSystem? fileSystem,
        ICreateStream? streamCreator,
        CancellationToken cancel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        cancel.ThrowIfCancellationRequested();
        var metaData = new SerializationMetaData(item.GameRelease, workDropoff, fileSystem, streamCreator, cancel);
        await SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static async Task SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        kernel.WriteModKey(writer, "ModKey", item.ModKey, default, checkDefaults: false);
        kernel.WriteEnum<GameRelease>(writer, "GameRelease", item.GameRelease, default, checkDefaults: false);
        try
        {
            if (item.SortableRecords is {} checkedSortableRecords
                && checkedSortableRecords.Count > 0)
            {
                kernel.StartListSection(writer, "SortableRecords");
                var sortedItems = checkedSortableRecords
                    .OrderBy(x => x.Priority)
                    .ThenBy(x => x.EditorID)
                    .ToList();
                foreach (var listItem in sortedItems)
                {
                    await kernel.WriteLoqui(writer, null, listItem, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SortableMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
                }
                kernel.EndListSection(writer);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            SubrecordException.EnrichAndThrow(e, item);
            throw;
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter? item,
        CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        try
        {
            var metaData = new SerializationMetaData(item.GameRelease, null!, null!, null!, cancel);
            if (item.SortableRecords.Count > 0) return true;
            return false;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            SubrecordException.EnrichAndThrow(e, item);
            throw;
        }
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod> Deserialize<TReadObject, TMeta>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        ModKey modKey,
        Serialization.SourceGenerator.TestsRelease release,
        IWorkDropoff? workDropoff,
        IFileSystem? fileSystem,
        ICreateStream? streamCreator,
        TMeta? extraMeta,
        ReadInto<ISerializationReaderKernel<TReadObject>, TReadObject, TMeta>? metaReader,
        CancellationToken cancel)
        where TReadObject : IContainStreamPackage
    {
        cancel.ThrowIfCancellationRequested();
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod(
            modKey,
            release);
        await DeserializeInto<TReadObject, TMeta>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator,
            extraMeta: extraMeta,
            metaReader: metaReader,
            cancel: cancel);
        return obj;
    }

    public static async Task DeserializeSingleFieldInto<TReadObject, TMeta>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod obj,
        SerializationMetaData metaData,
        string name,
        TMeta? extraMeta,
        ReadInto<ISerializationReaderKernel<TReadObject>, TReadObject, TMeta>? metaReader)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        switch (name)
        {
            case "SortableRecords":
                kernel.StartListSection(reader);
                while (kernel.TryHasNextItem(reader))
                {
                    var item = SerializationHelper.StripNull(await kernel.ReadLoqui(reader, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SortableMajorRecord_Serialization.Deserialize<TReadObject>(r, k, m)), name: "SortableRecords");
                    obj.SortableRecords.Add(item);
                }
                kernel.EndListSection(reader);
                break;
            default:
                if (extraMeta != null && metaReader != null && name.Equals(extraMeta.GetType().Name))
                {
                    await metaReader(reader, extraMeta, kernel, metaData);
                }
                else
                {
                    kernel.Skip(reader);
                }
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject, TMeta>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod obj,
        IWorkDropoff? workDropoff,
        IFileSystem? fileSystem,
        ICreateStream? streamCreator,
        TMeta? extraMeta,
        ReadInto<ISerializationReaderKernel<TReadObject>, TReadObject, TMeta>? metaReader,
        CancellationToken cancel)
        where TReadObject : IContainStreamPackage
    {
        cancel.ThrowIfCancellationRequested();
        var metaData = new SerializationMetaData(obj.GameRelease, workDropoff, fileSystem, streamCreator, cancel);
        while (kernel.TryGetNextField(reader, out var name))
        {
            await DeserializeSingleFieldInto<TReadObject, TMeta>(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name,
                extraMeta: extraMeta,
                metaReader: metaReader);
        }

    }

}

