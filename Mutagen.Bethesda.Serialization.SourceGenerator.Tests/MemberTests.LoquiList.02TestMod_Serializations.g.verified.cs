﻿//HintName: TestMod_Serializations.g.cs
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
            Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize(listItem, writer, kernel);
        }
        kernel.EndListSection(writer);
        kernel.StartListSection(writer, "SomeList2");
        foreach (var listItem in item.SomeList2)
        {
            Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize(listItem, writer, kernel);
        }
        kernel.EndListSection(writer);
        kernel.StartListSection(writer, "SomeList3");
        foreach (var listItem in item.SomeList3)
        {
            Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize(listItem, writer, kernel);
        }
        kernel.EndListSection(writer);
        kernel.StartListSection(writer, "SomeList4");
        foreach (var listItem in item.SomeList4)
        {
            Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize(listItem, writer, kernel);
        }
        kernel.EndListSection(writer);
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

