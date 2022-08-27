//HintName: ISomeModGetter_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class ISomeModGetter_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.WriteInt32(writer, "SomeInt", item.SomeInt);
        using (kernel.StartListSection(writer, "SomeList")
        {
            foreach (var listItem in item.SomeList)
            {
                kernel.WriteInt32(writer, null, listItem);
            }
        }
        SubObject_Serialization.Serialize(item.SomeObject, writer, kernel);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

