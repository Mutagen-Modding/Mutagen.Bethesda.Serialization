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
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList2 is {} checkedSomeList2
            && checkedSomeList2.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList2");
            foreach (var listItem in checkedSomeList2)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList3 is {} checkedSomeList3
            && checkedSomeList3.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList3");
            foreach (var listItem in checkedSomeList3)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList4 is {} checkedSomeList4
            && checkedSomeList4.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList4");
            foreach (var listItem in checkedSomeList4)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList5 is {} checkedSomeList5)
        {
            kernel.StartListSection(writer, "SomeList5");
            foreach (var listItem in checkedSomeList5)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList6 is {} checkedSomeList6)
        {
            kernel.StartListSection(writer, "SomeList6");
            foreach (var listItem in checkedSomeList6)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList7 is {} checkedSomeList7)
        {
            kernel.StartListSection(writer, "SomeList7");
            foreach (var listItem in checkedSomeList7)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList8 is {} checkedSomeList8)
        {
            kernel.StartListSection(writer, "SomeList8");
            foreach (var listItem in checkedSomeList8)
            {
                kernel.WriteString(writer, null, listItem);
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

