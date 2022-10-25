//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

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
    {
        kernel.WriteFormKey(writer, "SomeFormKey", item.SomeFormKey.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey2", item.SomeFormKey2.FormKeyNullable, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeFormKey3", item.SomeFormKey3.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey4", item.SomeFormKey4.FormKeyNullable, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeFormKey5", item.SomeFormKey5.FormKeyNullable, default(FormKey));
        kernel.WriteFormKey(writer, "SomeFormKey6", item.SomeFormKey6.FormKeyNullable, default(FormKey?));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<FormLink<INpcGetter>>.Default.Equals(item.SomeFormKey, default(FormLink<INpcGetter>))) return true;
        if (!EqualityComparer<FormLinkNullable<INpcGetter>>.Default.Equals(item.SomeFormKey2, default(FormLinkNullable<INpcGetter>))) return true;
        if (!EqualityComparer<IFormLink<INpcGetter>>.Default.Equals(item.SomeFormKey3, default(IFormLink<INpcGetter>))) return true;
        if (!EqualityComparer<IFormLinkNullable<INpcGetter>>.Default.Equals(item.SomeFormKey4, default(IFormLinkNullable<INpcGetter>))) return true;
        if (!EqualityComparer<IFormLinkGetter<INpcGetter>>.Default.Equals(item.SomeFormKey5, default(IFormLinkGetter<INpcGetter>))) return true;
        if (!EqualityComparer<IFormLinkNullableGetter<INpcGetter>>.Default.Equals(item.SomeFormKey6, default(IFormLinkNullableGetter<INpcGetter>))) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
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
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            switch (name)
            {
                case "SomeFormKey":
                    obj.SomeFormKey.SetTo(kernel.ReadFormKey(reader));
                    break;
                case "SomeFormKey2":
                    obj.SomeFormKey2.SetTo(kernel.ReadFormKey(reader));
                    break;
                case "SomeFormKey3":
                    obj.SomeFormKey3.SetTo(kernel.ReadFormKey(reader));
                    break;
                case "SomeFormKey4":
                    obj.SomeFormKey4.SetTo(kernel.ReadFormKey(reader));
                    break;
                case "SomeFormKey5":
                    obj.SomeFormKey5.SetTo(kernel.ReadFormKey(reader));
                    break;
                case "SomeFormKey6":
                    obj.SomeFormKey6.SetTo(kernel.ReadFormKey(reader));
                    break;
                default:
                    break;
            }
        }

    }

}

