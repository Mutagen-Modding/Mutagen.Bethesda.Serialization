//HintName: ITestModGetter_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class ITestModGetter_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        foreach (var listItem in item.SomeList)
        {
            kernel.WriteString(writer, null, listItem);
        }
        foreach (var listItem in item.SomeList2)
        {
            kernel.WriteString(writer, null, listItem);
        }
        foreach (var listItem in item.SomeList3)
        {
            kernel.WriteString(writer, null, listItem);
        }
        foreach (var listItem in item.SomeList4)
        {
            kernel.WriteString(writer, null, listItem);
        }
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

