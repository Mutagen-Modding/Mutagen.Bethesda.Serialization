//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static void SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        List<Action> parallelToDo = new List<Action>();
        SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList,
            fieldName: "SomeList",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false,
            toDo: parallelToDo);
        SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList2,
            fieldName: "SomeList2",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false,
            toDo: parallelToDo);
        SerializationHelper.WriteMajorRecordList<TKernel, TWriteObject, ITestMajorRecordGetter>(
            streamPackage: writer.StreamPackage,
            list: item.SomeList3,
            fieldName: "SomeList3",
            metaData: metaData,
            kernel: kernel,
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m),
            withNumbering: false,
            toDo: parallelToDo);
        if (item.SomeList4 is {} checkedSomeList4
            && checkedSomeList4.Length > 0)
        {
            kernel.StartListSection(writer, "SomeList4");
            foreach (var listItem in checkedSomeList4)
            {
                kernel.WriteLoqui(writer, null, listItem, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
            }
            kernel.EndListSection(writer);
        }
        Parallel.Invoke(parallelToDo.ToArray());
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (item.SomeList.Count > 0) return true;
        if (item.SomeList2.Count > 0) return true;
        if (item.SomeList3.Count > 0) return true;
        if (item.SomeList4.Length > 0) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static void DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name,
        List<Action> parallelToDo)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            case "SomeList4":
                {
                    obj.SomeList4 = SerializationHelper.ReadArray(
                        reader: reader,
                        kernel: kernel,
                        metaData: metaData,
                        itemReader: static (r, k, m) =>
                        {
                            return k.ReadLoqui(r, m, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>(r, k, m));
                        });
                }
                break;
            default:
                break;
        }
    }
    
    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
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

        SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            list: obj.SomeList,
            metaData: metaData,
            kernel: kernel,
            itemReader: static (r, k, m) => k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>),
            toDo: parallelToDo);
        SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            list: obj.SomeList2,
            metaData: metaData,
            kernel: kernel,
            itemReader: static (r, k, m) => k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>),
            toDo: parallelToDo);
        SerializationHelper.ReadMajorRecordList<ISerializationReaderKernel<TReadObject>, TReadObject, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord>(
            streamPackage: reader.StreamPackage,
            list: obj.SomeList3,
            metaData: metaData,
            kernel: kernel,
            itemReader: static (r, k, m) => k.ReadLoqui(r, m, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>),
            toDo: parallelToDo);
        Parallel.Invoke(parallelToDo.ToArray());
    }

}

