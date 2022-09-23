//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Plugins.Assets;
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
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<AssetLink<INpcGetter>>.Default.Equals(item.SomeAssetLink, default(AssetLink<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter>>.Default.Equals(item.SomeAssetLink2, default(Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>>.Default.Equals(item.SomeAssetLink3, default(Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>))) return true;
        if (!EqualityComparer<AssetLinkGetter<INpcGetter>>.Default.Equals(item.SomeAssetLink4, default(AssetLinkGetter<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<INpcGetter>>.Default.Equals(item.SomeAssetLink5, default(Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<Mutagen.Bethesda.Skyrim.INpcGetter>>.Default.Equals(item.SomeAssetLink6, default(Mutagen.Bethesda.Plugins.Assets.AssetLinkGetter<Mutagen.Bethesda.Skyrim.INpcGetter>))) return true;
        if (!EqualityComparer<IAssetLink<INpcGetter>>.Default.Equals(item.SomeAssetLink7, default(IAssetLink<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>>.Default.Equals(item.SomeAssetLink8, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>>.Default.Equals(item.SomeAssetLink9, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>))) return true;
        if (!EqualityComparer<IAssetLinkGetter<INpcGetter>>.Default.Equals(item.SomeAssetLink10, default(IAssetLinkGetter<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>>.Default.Equals(item.SomeAssetLink11, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>>.Default.Equals(item.SomeAssetLink12, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>))) return true;
        if (!EqualityComparer<AssetLink<INpcGetter>?>.Default.Equals(item.SomeAssetLink13, default(AssetLink<INpcGetter>?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter>?>.Default.Equals(item.SomeAssetLink14, default(Mutagen.Bethesda.Plugins.Assets.AssetLink<INpcGetter>?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>?>.Default.Equals(item.SomeAssetLink15, default(Mutagen.Bethesda.Plugins.Assets.AssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>?))) return true;
        if (!EqualityComparer<IAssetLink<INpcGetter>?>.Default.Equals(item.SomeAssetLink16, default(IAssetLink<INpcGetter>?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>?>.Default.Equals(item.SomeAssetLink17, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>?>.Default.Equals(item.SomeAssetLink18, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>?))) return true;
        if (!EqualityComparer<IAssetLinkGetter<INpcGetter>?>.Default.Equals(item.SomeAssetLink19, default(IAssetLinkGetter<INpcGetter>?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>?>.Default.Equals(item.SomeAssetLink20, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<INpcGetter>?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>?>.Default.Equals(item.SomeAssetLink21, default(Mutagen.Bethesda.Plugins.Assets.IAssetLink<Mutagen.Bethesda.Skyrim.INpcGetter>?))) return true;
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
                case: "SomeAssetLink":
                    item.SomeAssetLink = kernel.ReadString(writer);
                case: "SomeAssetLink2":
                    item.SomeAssetLink2 = kernel.ReadString(writer);
                case: "SomeAssetLink3":
                    item.SomeAssetLink3 = kernel.ReadString(writer);
                case: "SomeAssetLink4":
                    item.SomeAssetLink4 = kernel.ReadString(writer);
                case: "SomeAssetLink5":
                    item.SomeAssetLink5 = kernel.ReadString(writer);
                case: "SomeAssetLink6":
                    item.SomeAssetLink6 = kernel.ReadString(writer);
                case: "SomeAssetLink7":
                    item.SomeAssetLink7 = kernel.ReadString(writer);
                case: "SomeAssetLink8":
                    item.SomeAssetLink8 = kernel.ReadString(writer);
                case: "SomeAssetLink9":
                    item.SomeAssetLink9 = kernel.ReadString(writer);
                case: "SomeAssetLink10":
                    item.SomeAssetLink10 = kernel.ReadString(writer);
                case: "SomeAssetLink11":
                    item.SomeAssetLink11 = kernel.ReadString(writer);
                case: "SomeAssetLink12":
                    item.SomeAssetLink12 = kernel.ReadString(writer);
                case: "SomeAssetLink13":
                    item.SomeAssetLink13 = kernel.ReadString(writer);
                case: "SomeAssetLink14":
                    item.SomeAssetLink14 = kernel.ReadString(writer);
                case: "SomeAssetLink15":
                    item.SomeAssetLink15 = kernel.ReadString(writer);
                case: "SomeAssetLink16":
                    item.SomeAssetLink16 = kernel.ReadString(writer);
                case: "SomeAssetLink17":
                    item.SomeAssetLink17 = kernel.ReadString(writer);
                case: "SomeAssetLink18":
                    item.SomeAssetLink18 = kernel.ReadString(writer);
                case: "SomeAssetLink19":
                    item.SomeAssetLink19 = kernel.ReadString(writer);
                case: "SomeAssetLink20":
                    item.SomeAssetLink20 = kernel.ReadString(writer);
                case: "SomeAssetLink21":
                    item.SomeAssetLink21 = kernel.ReadString(writer);
                default:
                    break;
            }
        }
    }

}

