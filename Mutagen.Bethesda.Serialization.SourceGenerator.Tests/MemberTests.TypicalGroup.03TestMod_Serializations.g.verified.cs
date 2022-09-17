//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup,
            fieldName: "SomeGroup",
            kernel: kernel,
            groupWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k),
            itemWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k));
        SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup2,
            fieldName: "SomeGroup2",
            kernel: kernel,
            groupWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k),
            itemWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k));
        SerializationHelper.WriteGroup<TKernel, TWriteObject, IGroupGetter<ITestMajorRecordGetter>, ITestMajorRecordGetter>(
            writer: writer,
            group: item.SomeGroup3,
            fieldName: "SomeGroup3",
            kernel: kernel,
            groupWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Group_Serialization.Serialize<TKernel, TWriteObject, ITestMajorRecordGetter>(w, i, k),
            itemWriter: static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMajorRecord_Serialization.Serialize<TKernel, TWriteObject>(w, i, k));
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

