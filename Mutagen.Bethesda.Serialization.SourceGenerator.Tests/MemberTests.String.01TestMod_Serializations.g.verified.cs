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
        var metaData = new SerializationMetaData(item.GameRelease);
        kernel.WriteString(writer, "SomeMember0", item.SomeMember0, default(string));
        kernel.WriteString(writer, "SomeMember1", item.SomeMember1, default(String));
        kernel.WriteString(writer, "SomeMember2", item.SomeMember2, default(string?));
        kernel.WriteString(writer, "SomeMember3", item.SomeMember3, default(String?));
        kernel.WriteString(writer, "SomeMember4", item.SomeMember4, default(Nullable<string>));
        kernel.WriteString(writer, "SomeMember5", item.SomeMember5, default(Nullable<String>));
        kernel.WriteString(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember6Default);
        kernel.WriteString(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember7Default);
        kernel.WriteString(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember8Default);
        kernel.WriteString(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember9Default);
        kernel.WriteString(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember10Default);
        kernel.WriteString(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember11Default);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        if (!EqualityComparer<string>.Default.Equals(item.SomeMember0, default(string))) return true;
        if (!EqualityComparer<String>.Default.Equals(item.SomeMember1, default(String))) return true;
        if (!EqualityComparer<string?>.Default.Equals(item.SomeMember2, default(string?))) return true;
        if (!EqualityComparer<String?>.Default.Equals(item.SomeMember3, default(String?))) return true;
        if (!EqualityComparer<Nullable<string>>.Default.Equals(item.SomeMember4, default(Nullable<string>))) return true;
        if (!EqualityComparer<Nullable<String>>.Default.Equals(item.SomeMember5, default(Nullable<String>))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember6Default)) return true;
        if (!EqualityComparer<String>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember7Default)) return true;
        if (!EqualityComparer<string?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember8Default)) return true;
        if (!EqualityComparer<String?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<string>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<String>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

