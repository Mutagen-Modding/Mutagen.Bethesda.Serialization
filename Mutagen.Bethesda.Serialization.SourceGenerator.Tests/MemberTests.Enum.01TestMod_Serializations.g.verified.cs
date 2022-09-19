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
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum>(writer, "SomeEnum", item.SomeEnum, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum>(writer, "SomeEnum2", item.SomeEnum2, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2>(writer, "SomeEnum3", item.SomeEnum3, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2>(writer, "SomeEnum4", item.SomeEnum4, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3>(writer, "SomeEnum5", item.SomeEnum5, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3>(writer, "SomeEnum6", item.SomeEnum6, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3));
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        var metaData = new SerializationMetaData(item.GameRelease);
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum>.Default.Equals(item.SomeEnum, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum>>.Default.Equals(item.SomeEnum2, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2>.Default.Equals(item.SomeEnum3, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2>>.Default.Equals(item.SomeEnum4, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum2>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3>.Default.Equals(item.SomeEnum5, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3>>.Default.Equals(item.SomeEnum6, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.MyEnum3>))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

