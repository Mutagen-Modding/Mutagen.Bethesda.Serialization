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
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter? item)
    {
        if (item == null) return false;
        var metaData = new SerializationMetaData(item.GameRelease, null!, null!, null!);
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        ModKey modKey,
        Serialization.SourceGenerator.TestsRelease release,
        IWorkDropoff? workDropoff,
        IFileSystem? fileSystem,
        ICreateStream? streamCreator)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod(modKey, release);
        await DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            workDropoff: workDropoff,
            fileSystem: fileSystem,
            streamCreator: streamCreator);
        return obj;
    }

    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod obj,
        IWorkDropoff? workDropoff,
        IFileSystem? fileSystem,
        ICreateStream? streamCreator)
        where TReadObject : IContainStreamPackage
    {
        var metaData = new SerializationMetaData(obj.GameRelease, workDropoff, fileSystem, streamCreator);
    }

}

