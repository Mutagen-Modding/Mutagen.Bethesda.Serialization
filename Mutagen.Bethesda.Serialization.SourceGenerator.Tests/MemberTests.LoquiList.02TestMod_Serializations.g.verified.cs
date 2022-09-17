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
        if (item.SomeList is {} checkedSomeList
            && checkedSomeList.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList");
            foreach (var listItem in checkedSomeList)
            {
                kernel.WriteLoqui(writer, null, listItem, static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList2 is {} checkedSomeList2
            && checkedSomeList2.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList2");
            foreach (var listItem in checkedSomeList2)
            {
                kernel.WriteLoqui(writer, null, listItem, static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList3 is {} checkedSomeList3
            && checkedSomeList3.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList3");
            foreach (var listItem in checkedSomeList3)
            {
                kernel.WriteLoqui(writer, null, listItem, static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList4 is {} checkedSomeList4
            && checkedSomeList4.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList4");
            foreach (var listItem in checkedSomeList4)
            {
                kernel.WriteLoqui(writer, null, listItem, static (w, i, k) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k));
            }
            kernel.EndListSection(writer);
        }
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

