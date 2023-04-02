//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Noggog;

#nullable enable

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static void SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
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

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink2, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink3, default(IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink4, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink5, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink6, default(IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink7, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink8, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink9, default(IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink10, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink11, default(IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>.Default.Equals(item.SomeAssetLink12, default(IAssetLinkGetter<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<AssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink13, default(IAssetLinkGetter<AssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink14, default(IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink15, default(IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<IAssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink16, default(IAssetLinkGetter<IAssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink17, default(IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink18, default(IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink19, default(IAssetLinkGetter<IAssetLinkGetter<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink20, default(IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMajorRecordGetter>>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>>.Default.Equals(item.SomeAssetLink21, default(IAssetLinkGetter<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.ITestMajorRecordGetter>>))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static void DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            DeserializeSingleFieldInto(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

    }

    public static void DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            case "SomeAssetLink":
                obj.SomeAssetLink = kernel.ReadString(reader);
                break;
            case "SomeAssetLink2":
                obj.SomeAssetLink2 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink3":
                obj.SomeAssetLink3 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink4":
                obj.SomeAssetLink4 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink5":
                obj.SomeAssetLink5 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink6":
                obj.SomeAssetLink6 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink7":
                obj.SomeAssetLink7 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink8":
                obj.SomeAssetLink8 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink9":
                obj.SomeAssetLink9 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink10":
                obj.SomeAssetLink10 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink11":
                obj.SomeAssetLink11 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink12":
                obj.SomeAssetLink12 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink13":
                obj.SomeAssetLink13 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink14":
                obj.SomeAssetLink14 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink15":
                obj.SomeAssetLink15 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink16":
                obj.SomeAssetLink16 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink17":
                obj.SomeAssetLink17 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink18":
                obj.SomeAssetLink18 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink19":
                obj.SomeAssetLink19 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink20":
                obj.SomeAssetLink20 = kernel.ReadString(reader);
                break;
            case "SomeAssetLink21":
                obj.SomeAssetLink21 = kernel.ReadString(reader);
                break;
            default:
                break;
        }
    }

}

