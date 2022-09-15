//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        SerializationHelper.WriteGroup<TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup,
            fieldName: "SomeGroup",
            kernel: kernel,
            groupWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TWriteObject, ITestMajorRecordGetter>(w, i, k),
            itemWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TWriteObject>(w, i, k));
        SerializationHelper.WriteGroup<TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup2,
            fieldName: "SomeGroup2",
            kernel: kernel,
            groupWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TWriteObject, ITestMajorRecordGetter>(w, i, k),
            itemWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TWriteObject>(w, i, k));
        SerializationHelper.WriteGroup<TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup3,
            fieldName: "SomeGroup3",
            kernel: kernel,
            groupWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TWriteObject, ITestMajorRecordGetter>(w, i, k),
            itemWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TWriteObject>(w, i, k));
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

