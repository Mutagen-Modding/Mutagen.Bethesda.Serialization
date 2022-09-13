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
        kernel.StartListSection(writer, "SomeList");
        foreach (var listItem in item.SomeList)
        {
            kernel.WriteString(writer, null, listItem);
        }
        kernel.EndListSection(writer);
        kernel.StartListSection(writer, "SomeList2");
        foreach (var listItem in item.SomeList2)
        {
            kernel.WriteString(writer, null, listItem);
        }
        kernel.EndListSection(writer);
        kernel.StartListSection(writer, "SomeList3");
        foreach (var listItem in item.SomeList3)
        {
            kernel.WriteString(writer, null, listItem);
        }
        kernel.EndListSection(writer);
        kernel.StartListSection(writer, "SomeList4");
        foreach (var listItem in item.SomeList4)
        {
            kernel.WriteString(writer, null, listItem);
        }
        kernel.EndListSection(writer);
        if (item.SomeList5 is {} checkedSomeList5)
        {
            kernel.StartListSection(writer, "checkedSomeList5");
            foreach (var listItem in item.SomeList5)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList6 is {} checkedSomeList6)
        {
            kernel.StartListSection(writer, "checkedSomeList6");
            foreach (var listItem in item.SomeList6)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList7 is {} checkedSomeList7)
        {
            kernel.StartListSection(writer, "checkedSomeList7");
            foreach (var listItem in item.SomeList7)
            {
                kernel.WriteString(writer, null, listItem);
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList8 is {} checkedSomeList8)
        {
            kernel.StartListSection(writer, "checkedSomeList8");
            foreach (var listItem in item.SomeList8)
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

