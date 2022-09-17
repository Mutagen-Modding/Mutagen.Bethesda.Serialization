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
        kernel.WriteString(writer, "SomeGenderedIntMale", item.SomeGenderedInt.Male, default(string));
        kernel.WriteString(writer, "SomeGenderedIntFemale", item.SomeGenderedInt.Female, default(string));
        kernel.WriteString(writer, "SomeGenderedInt2Male", item.SomeGenderedInt2.Male, default(string));
        kernel.WriteString(writer, "SomeGenderedInt2Female", item.SomeGenderedInt2.Female, default(string));
        kernel.WriteString(writer, "SomeGenderedInt3Male", item.SomeGenderedInt3.Male, default(string));
        kernel.WriteString(writer, "SomeGenderedInt3Female", item.SomeGenderedInt3.Female, default(string));
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

