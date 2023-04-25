//HintName: TestMod_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.WorkEngine;
using System.Threading.Tasks;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        IWorkDropoff workDropoff)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        var metaData = new SerializationMetaData(item.GameRelease, workDropoff);
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
        var tasks = new List<Task>();
        tasks.Add(SerializationHelper.AddBlocksToWork<TKernel, TWriteObject, IListGroupGetter<ICellBlockGetter>, ICellBlockGetter, ICellSubBlockGetter, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            obj: item.SomeGroup,
            fieldName: "SomeGroup",
            metaData: metaData,
            kernel: kernel,
            blockRetriever: static x => x.Items,
            subBlockRetriever: static x => x.SubBlocks,
            majorRetriever: static x => x.Records,
            blockNumberRetriever: static x => x.BlockNumber,
            subBlockNumberRetriever: static x => x.BlockNumber,
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.Serialize<TKernel, TWriteObject, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellSubBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false));
        tasks.Add(SerializationHelper.AddBlocksToWork<TKernel, TWriteObject, IListGroupGetter<ICellBlockGetter>, ICellBlockGetter, ICellSubBlockGetter, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            obj: item.SomeGroup2,
            fieldName: "SomeGroup2",
            metaData: metaData,
            kernel: kernel,
            blockRetriever: static x => x.Items,
            subBlockRetriever: static x => x.SubBlocks,
            majorRetriever: static x => x.Records,
            blockNumberRetriever: static x => x.BlockNumber,
            subBlockNumberRetriever: static x => x.BlockNumber,
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.Serialize<TKernel, TWriteObject, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellSubBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false));
        tasks.Add(SerializationHelper.AddBlocksToWork<TKernel, TWriteObject, IListGroupGetter<ICellBlockGetter>, ICellBlockGetter, ICellSubBlockGetter, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            obj: item.SomeGroup3,
            fieldName: "SomeGroup3",
            metaData: metaData,
            kernel: kernel,
            blockRetriever: static x => x.Items,
            subBlockRetriever: static x => x.SubBlocks,
            majorRetriever: static x => x.Records,
            blockNumberRetriever: static x => x.BlockNumber,
            subBlockNumberRetriever: static x => x.BlockNumber,
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.Serialize<TKernel, TWriteObject, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellSubBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false));
        await Task.WhenAll(tasks.ToArray());
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter? item)
    {
        if (item == null) return false;
        var metaData = new SerializationMetaData(item.GameRelease, null!);
        if (item.SomeGroup.Count > 0) return true;
        if (item.SomeGroup2.Count > 0) return true;
        if (item.SomeGroup3.Count > 0) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        ModKey modKey,
        Serialization.SourceGenerator.TestsRelease release,
        IWorkDropoff workDropoff)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod(modKey, release);
        await DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            workDropoff: workDropoff);
        return obj;
    }

    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod obj,
        IWorkDropoff workDropoff)
        where TReadObject : IContainStreamPackage
    {
        var metaData = new SerializationMetaData(obj.GameRelease, workDropoff);
        var tasks = new List<Task>();
        tasks.Add(SerializationHelper.ReadFilePerRecordIntoBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, IListGroup<CellBlock>, CellBlock, CellSubBlock, TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            group: obj.SomeGroup,
            fieldName: "SomeGroup",
            metaData: metaData,
            kernel: kernel,
            groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.DeserializeSingleFieldInto<TReadObject, CellBlock>(r, k, i, m, n),
            blockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
            subBlockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellSubBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
            majorReader: static async (r, k, m) => SerializationHelper.StripNull(await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>), "SomeGroup"),
            groupSetter: static (b, sub) =>
            {
                b.Items.SetTo(sub);
            },
            blockSet: static (b, i, sub) =>
            {
                b.BlockNumber = i;
                b.SubBlocks.SetTo(sub);
            },
            subBlockSet: static (b, i, sub) =>
            {
                b.BlockNumber = i;
                b.Records.SetTo(sub);
            }));
        tasks.Add(SerializationHelper.ReadFilePerRecordIntoBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, IListGroup<CellBlock>, CellBlock, CellSubBlock, TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            group: obj.SomeGroup2,
            fieldName: "SomeGroup2",
            metaData: metaData,
            kernel: kernel,
            groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.DeserializeSingleFieldInto<TReadObject, CellBlock>(r, k, i, m, n),
            blockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
            subBlockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellSubBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
            majorReader: static async (r, k, m) => SerializationHelper.StripNull(await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>), "SomeGroup2"),
            groupSetter: static (b, sub) =>
            {
                b.Items.SetTo(sub);
            },
            blockSet: static (b, i, sub) =>
            {
                b.BlockNumber = i;
                b.SubBlocks.SetTo(sub);
            },
            subBlockSet: static (b, i, sub) =>
            {
                b.BlockNumber = i;
                b.Records.SetTo(sub);
            }));
        tasks.Add(SerializationHelper.ReadFilePerRecordIntoBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, IListGroup<CellBlock>, CellBlock, CellSubBlock, TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            group: obj.SomeGroup3,
            fieldName: "SomeGroup3",
            metaData: metaData,
            kernel: kernel,
            groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.DeserializeSingleFieldInto<TReadObject, CellBlock>(r, k, i, m, n),
            blockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
            subBlockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellSubBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
            majorReader: static async (r, k, m) => SerializationHelper.StripNull(await k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>), "SomeGroup3"),
            groupSetter: static (b, sub) =>
            {
                b.Items.SetTo(sub);
            },
            blockSet: static (b, i, sub) =>
            {
                b.BlockNumber = i;
                b.SubBlocks.SetTo(sub);
            },
            subBlockSet: static (b, i, sub) =>
            {
                b.BlockNumber = i;
                b.Records.SetTo(sub);
            }));
        await Task.WhenAll(tasks.ToArray());
    }

}

