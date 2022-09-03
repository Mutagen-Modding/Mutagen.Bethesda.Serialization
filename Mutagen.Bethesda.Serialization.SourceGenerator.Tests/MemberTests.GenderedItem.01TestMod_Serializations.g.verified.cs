//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TWriteObject>(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        TWriteObject writer,
        ISerializationWriterKernel<TWriteObject> kernel)
    {
        kernel.WriteString(writer, "SomeGenderedIntMale", item.SomeGenderedInt.Male);
        kernel.WriteString(writer, "SomeGenderedIntFemale", item.SomeGenderedInt.Female);
        kernel.WriteString(writer, "SomeGenderedInt2Male", item.SomeGenderedInt2.Male);
        kernel.WriteString(writer, "SomeGenderedInt2Female", item.SomeGenderedInt2.Female);
        kernel.WriteString(writer, "SomeGenderedInt3Male", item.SomeGenderedInt3.Male);
        kernel.WriteString(writer, "SomeGenderedInt3Female", item.SomeGenderedInt3.Female);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

