//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteString(writer, "SomeGenderedIntMale", item.SomeGenderedInt.Male, default(string));
        kernel.WriteString(writer, "SomeGenderedIntFemale", item.SomeGenderedInt.Female, default(string));
        kernel.WriteString(writer, "SomeGenderedInt2Male", item.SomeGenderedInt2.Male, default(string));
        kernel.WriteString(writer, "SomeGenderedInt2Female", item.SomeGenderedInt2.Female, default(string));
        kernel.WriteString(writer, "SomeGenderedInt3Male", item.SomeGenderedInt3.Male, default(string));
        kernel.WriteString(writer, "SomeGenderedInt3Female", item.SomeGenderedInt3.Female, default(string));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt.Female, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt2.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt2.Female, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt3.Male, default(string))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeGenderedInt3.Female, default(string))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "SomeGenderedInt":
                    item.SomeGenderedInt.Male = kernel.ReadString(reader);
                    item.SomeGenderedInt.Female = kernel.ReadString(reader);
                case "SomeGenderedInt2":
                    item.SomeGenderedInt2.Male = kernel.ReadString(reader);
                    item.SomeGenderedInt2.Female = kernel.ReadString(reader);
                case "SomeGenderedInt3":
                    item.SomeGenderedInt3.Male = kernel.ReadString(reader);
                    item.SomeGenderedInt3.Female = kernel.ReadString(reader);
                default:
                    break;
            }
        }
    }

}

