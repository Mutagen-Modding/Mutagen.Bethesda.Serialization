//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Plugins.Assets;
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
        kernel.WriteString(writer, "SomeAssetLink", item.SomeAssetLink?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink2", item.SomeAssetLink2?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink3", item.SomeAssetLink3?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink4", item.SomeAssetLink4?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink5", item.SomeAssetLink5?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink6", item.SomeAssetLink6?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink7", item.SomeAssetLink7?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink8", item.SomeAssetLink8?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink9", item.SomeAssetLink9?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink10", item.SomeAssetLink10?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink11", item.SomeAssetLink11?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink12", item.SomeAssetLink12?.RawPath, default(string));
        kernel.WriteString(writer, "SomeAssetLink13", item.SomeAssetLink13?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink14", item.SomeAssetLink14?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink15", item.SomeAssetLink15?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink16", item.SomeAssetLink16?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink17", item.SomeAssetLink17?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink18", item.SomeAssetLink18?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink19", item.SomeAssetLink19?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink20", item.SomeAssetLink20?.RawPath, default(string?));
        kernel.WriteString(writer, "SomeAssetLink21", item.SomeAssetLink21?.RawPath, default(string?));
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

