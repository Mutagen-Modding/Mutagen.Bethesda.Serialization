﻿//HintName: TestMod_Serializations.g.cs
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
        kernel.WriteChar(writer, "SomeMember0", item.SomeMember0, default(char));
        kernel.WriteChar(writer, "SomeMember1", item.SomeMember1, default(Char));
        kernel.WriteChar(writer, "SomeMember2", item.SomeMember2, default(char?));
        kernel.WriteChar(writer, "SomeMember3", item.SomeMember3, default(Char?));
        kernel.WriteChar(writer, "SomeMember4", item.SomeMember4, default(Nullable<char>));
        kernel.WriteChar(writer, "SomeMember5", item.SomeMember5, default(Nullable<Char>));
        kernel.WriteChar(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember6Default);
        kernel.WriteChar(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember7Default);
        kernel.WriteChar(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember8Default);
        kernel.WriteChar(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember9Default);
        kernel.WriteChar(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember10Default);
        kernel.WriteChar(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember11Default);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        if (!EqualityComparer<char>.Default.Equals(item.SomeMember0, default(char))) return true;
        if (!EqualityComparer<Char>.Default.Equals(item.SomeMember1, default(Char))) return true;
        if (!EqualityComparer<char?>.Default.Equals(item.SomeMember2, default(char?))) return true;
        if (!EqualityComparer<Char?>.Default.Equals(item.SomeMember3, default(Char?))) return true;
        if (!EqualityComparer<Nullable<char>>.Default.Equals(item.SomeMember4, default(Nullable<char>))) return true;
        if (!EqualityComparer<Nullable<Char>>.Default.Equals(item.SomeMember5, default(Nullable<Char>))) return true;
        if (!EqualityComparer<char>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember6Default)) return true;
        if (!EqualityComparer<Char>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember7Default)) return true;
        if (!EqualityComparer<char?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember8Default)) return true;
        if (!EqualityComparer<Char?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<char>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Char>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

