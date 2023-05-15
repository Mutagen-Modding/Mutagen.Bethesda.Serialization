//HintName: TestMod_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.WorkEngine;
using System.IO.Abstractions;
using System.Threading.Tasks;

#nullable enable

#pragma warning disable CA1998 // No awaits used
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
        ICreateStream? streamCreator)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        var metaData = new SerializationMetaData(item.GameRelease, workDropoff, fileSystem, streamCreator);
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
        kernel.WriteEnum<GameRelease>(writer, "GameRelease", item.GameRelease, default, checkDefaults: false);
        await SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup,
            fieldName: "SomeGroup",
            metaData: metaData,
            kernel: kernel,
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
        await SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup2,
            fieldName: "SomeGroup2",
            metaData: metaData,
            kernel: kernel,
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
        await SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup3,
            fieldName: "SomeGroup3",
            metaData: metaData,
            kernel: kernel,
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter? item)
    {
        if (item == null) return false;
        var metaData = new SerializationMetaData(item.GameRelease, null!, null!, null!);
        if (item.SomeGroup.Count > 0) return true;
        if (item.SomeGroup2.Count > 0) return true;
        if (item.SomeGroup3.Count > 0) return true;
        return false;
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
        ReadInto<ISerializationReaderKernel<TReadObject>, TReadObject, TMeta>? metaReader)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod(modKey, release);
        await DeserializeInto<TReadObject, TMeta>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator,
            extraMeta: extraMeta,
            metaReader: metaReader);
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
        switch (name)
        {
            case "SomeGroup":
                await SerializationHelper.ReadIntoGroup<ISerializationReaderKernel<TReadObject>, TReadObject, IGroup<TestMajorRecord>, TestMajorRecord>(
                    reader: reader,
                    group: obj.SomeGroup,
                    metaData: metaData,
                    kernel: kernel,
                    groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.DeserializeSingleFieldInto<TReadObject, TestMajorRecord>(r, k, i, m, n),
                    itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>)).StripNull("Group"));
                break;
            case "SomeGroup2":
                await SerializationHelper.ReadIntoGroup<ISerializationReaderKernel<TReadObject>, TReadObject, IGroup<TestMajorRecord>, TestMajorRecord>(
                    reader: reader,
                    group: obj.SomeGroup2,
                    metaData: metaData,
                    kernel: kernel,
                    groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.DeserializeSingleFieldInto<TReadObject, TestMajorRecord>(r, k, i, m, n),
                    itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>)).StripNull("Group"));
                break;
            case "SomeGroup3":
                await SerializationHelper.ReadIntoGroup<ISerializationReaderKernel<TReadObject>, TReadObject, IGroup<TestMajorRecord>, TestMajorRecord>(
                    reader: reader,
                    group: obj.SomeGroup3,
                    metaData: metaData,
                    kernel: kernel,
                    groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.DeserializeSingleFieldInto<TReadObject, TestMajorRecord>(r, k, i, m, n),
                    itemReader: static async (r, k, m) => (await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>)).StripNull("Group"));
                break;
            default:
                if (extraMeta != null && metaReader != null && name.Equals(extraMeta.GetType().Name))
                {
                    await metaReader(reader, extraMeta, kernel, metaData);
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
        ReadInto<ISerializationReaderKernel<TReadObject>, TReadObject, TMeta>? metaReader)
        where TReadObject : IContainStreamPackage
    {
        var metaData = new SerializationMetaData(obj.GameRelease, workDropoff, fileSystem, streamCreator);
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

