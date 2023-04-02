//HintName: TestMod_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Noggog;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static void SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        List<Action> parallelToDo = new List<Action>();
        SerializationHelper.AddBlocksToWork<TKernel, TWriteObject, IListGroupGetter<ICellBlockGetter>, ICellBlockGetter, ISubCellBlockGetter, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            obj: item.SomeGroup,
            fieldName: "SomeGroup",
            metaData: metaData,
            kernel: kernel,
            blockRetriever: x => x.Items,
            subBlockRetriever: x => x.SubBlocks,
            majorRetriever: x => x.Records,
            blockNumberRetriever: x => x.BlockNumber,
            subBlockNumberRetriever: x => x.BlockNumber,
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.Serialize<TKernel, TWriteObject, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubCellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            toDo: parallelToDo);
        SerializationHelper.AddBlocksToWork<TKernel, TWriteObject, IListGroupGetter<ICellBlockGetter>, ICellBlockGetter, ISubCellBlockGetter, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            obj: item.SomeGroup2,
            fieldName: "SomeGroup2",
            metaData: metaData,
            kernel: kernel,
            blockRetriever: x => x.Items,
            subBlockRetriever: x => x.SubBlocks,
            majorRetriever: x => x.Records,
            blockNumberRetriever: x => x.BlockNumber,
            subBlockNumberRetriever: x => x.BlockNumber,
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.Serialize<TKernel, TWriteObject, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubCellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            toDo: parallelToDo);
        SerializationHelper.AddBlocksToWork<TKernel, TWriteObject, IListGroupGetter<ICellBlockGetter>, ICellBlockGetter, ISubCellBlockGetter, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            obj: item.SomeGroup3,
            fieldName: "SomeGroup3",
            metaData: metaData,
            kernel: kernel,
            blockRetriever: x => x.Items,
            subBlockRetriever: x => x.SubBlocks,
            majorRetriever: x => x.Records,
            blockNumberRetriever: x => x.BlockNumber,
            subBlockNumberRetriever: x => x.BlockNumber,
            metaWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.Serialize<TKernel, TWriteObject, ICellBlockGetter>(w, i, k, m),
            blockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            subBlockWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubCellBlock_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            majorWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            toDo: parallelToDo);
        Parallel.Invoke(parallelToDo.ToArray());
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter? item)
    {
        if (item == null) return false;
        var metaData = new SerializationMetaData(item.GameRelease);
        if (item.SomeGroup.Count > 0) return true;
        if (item.SomeGroup2.Count > 0) return true;
        if (item.SomeGroup3.Count > 0) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        ModKey modKey,
        Serialization.SourceGenerator.TestsRelease release)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod(modKey, release);
        DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj);
        return obj;
    }

    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod obj)
        where TReadObject : IContainStreamPackage
    {
        var metaData = new SerializationMetaData(obj.GameRelease);
        List<Action> parallelToDo = new List<Action>();
        while (kernel.TryGetNextField(reader, out var name))
        {
            DeserializeSingleFieldInto(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name,
                parallelToDo: parallelToDo);
        }

        Parallel.Invoke(parallelToDo.ToArray());
    }

    public static void DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod obj,
        SerializationMetaData metaData,
        string name,
        List<Action> parallelToDo)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            case "SomeGroup":
                SerializationHelper.ReadFolderPerRecordIntoBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, IListGroup<CellBlock>, CellBlock, SubCellBlock, TestMajorRecord>(
                    streamPackage: reader.StreamPackage,
                    group: obj.SomeGroup,
                    metaData: metaData,
                    kernel: kernel,
                    groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.DeserializeSingleFieldInto<TReadObject, CellBlock>(r, k, i, m, n),
                    blockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
                    subBlockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubCellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
                    majorReader: static (r, k, m) => k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>),
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
                    },
                    toDo: parallelToDo);
                break;
            case "SomeGroup2":
                SerializationHelper.ReadFolderPerRecordIntoBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, IListGroup<CellBlock>, CellBlock, SubCellBlock, TestMajorRecord>(
                    streamPackage: reader.StreamPackage,
                    group: obj.SomeGroup2,
                    metaData: metaData,
                    kernel: kernel,
                    groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.DeserializeSingleFieldInto<TReadObject, CellBlock>(r, k, i, m, n),
                    blockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
                    subBlockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubCellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
                    majorReader: static (r, k, m) => k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>),
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
                    },
                    toDo: parallelToDo);
                break;
            case "SomeGroup3":
                SerializationHelper.ReadFolderPerRecordIntoBlocks<ISerializationReaderKernel<TReadObject>, TReadObject, IListGroup<CellBlock>, CellBlock, SubCellBlock, TestMajorRecord>(
                    streamPackage: reader.StreamPackage,
                    group: obj.SomeGroup3,
                    metaData: metaData,
                    kernel: kernel,
                    groupReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ListGroup_Serialization.DeserializeSingleFieldInto<TReadObject, CellBlock>(r, k, i, m, n),
                    blockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.CellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
                    subBlockReader: static (r, i, k, m, n) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SubCellBlock_Serialization.DeserializeSingleFieldInto<TReadObject>(r, k, i, m, n),
                    majorReader: static (r, k, m) => k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>),
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
                    },
                    toDo: parallelToDo);
                break;
            default:
                break;
        }
    }

}

