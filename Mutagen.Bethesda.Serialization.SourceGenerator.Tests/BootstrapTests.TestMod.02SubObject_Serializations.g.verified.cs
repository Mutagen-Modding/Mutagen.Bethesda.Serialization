//HintName: SubObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SubObject_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter.SubObject item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.WriteString(writer, "SubField", item.SubField);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeModGetter.SubObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

