//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup,
            fieldName: "SomeGroup",
            meta: metaData,
            kernel: kernel,
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
        SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup2,
            fieldName: "SomeGroup2",
            meta: metaData,
            kernel: kernel,
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
        SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup3,
            fieldName: "SomeGroup3",
            meta: metaData,
            kernel: kernel,
            groupWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k, m),
            itemWriter: static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        if (item.SomeGroup.Count > 0) return true;
        if (item.SomeGroup2.Count > 0) return true;
        if (item.SomeGroup3.Count > 0) return true;
        return false;
    }

    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod obj)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "SomeGroup":
                    SerializationHelper.ReadIntoGroup<ISerializationReaderKernel<TReadObject>, TReadObject, IGroup<TestMajorRecord>, TestMajorRecord>(
                        reader: reader,
                        group: obj.SomeGroup,
                        meta: metaData,
                        kernel: kernel,
                        groupReader: static (r, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.DeserializeInto<TReadObject, TestMajorRecord>(r, k, i, m),
                        itemReader: static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>(r, k, m));
                    break;
                case "SomeGroup2":
                    SerializationHelper.ReadIntoGroup<ISerializationReaderKernel<TReadObject>, TReadObject, IGroup<TestMajorRecord>, TestMajorRecord>(
                        reader: reader,
                        group: obj.SomeGroup2,
                        meta: metaData,
                        kernel: kernel,
                        groupReader: static (r, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.DeserializeInto<TReadObject, TestMajorRecord>(r, k, i, m),
                        itemReader: static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>(r, k, m));
                    break;
                case "SomeGroup3":
                    SerializationHelper.ReadIntoGroup<ISerializationReaderKernel<TReadObject>, TReadObject, IGroup<TestMajorRecord>, TestMajorRecord>(
                        reader: reader,
                        group: obj.SomeGroup3,
                        meta: metaData,
                        kernel: kernel,
                        groupReader: static (r, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.DeserializeInto<TReadObject, TestMajorRecord>(r, k, i, m),
                        itemReader: static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Deserialize<TReadObject>(r, k, m));
                    break;
                default:
                    break;
            }
        }

    }

}

