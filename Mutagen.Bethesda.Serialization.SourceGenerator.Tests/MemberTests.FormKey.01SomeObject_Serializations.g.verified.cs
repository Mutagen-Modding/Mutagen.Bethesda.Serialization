//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Plugins;
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
        kernel.WriteFormKey(writer, "SomeMember0", item.SomeMember0, default(FormKey));
        kernel.WriteFormKey(writer, "SomeMember1", item.SomeMember1, default(Mutagen.Bethesda.Plugins.FormKey));
        kernel.WriteFormKey(writer, "SomeMember2", item.SomeMember2, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeMember3", item.SomeMember3, default(Mutagen.Bethesda.Plugins.FormKey?));
        kernel.WriteFormKey(writer, "SomeMember4", item.SomeMember4, default(Nullable<FormKey>));
        kernel.WriteFormKey(writer, "SomeMember5", item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.FormKey>));
        kernel.WriteFormKey(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteFormKey(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteFormKey(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteFormKey(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteFormKey(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteFormKey(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<FormKey>.Default.Equals(item.SomeMember0, default(FormKey))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey>.Default.Equals(item.SomeMember1, default(Mutagen.Bethesda.Plugins.FormKey))) return true;
        if (!EqualityComparer<FormKey?>.Default.Equals(item.SomeMember2, default(FormKey?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey?>.Default.Equals(item.SomeMember3, default(Mutagen.Bethesda.Plugins.FormKey?))) return true;
        if (!EqualityComparer<Nullable<FormKey>>.Default.Equals(item.SomeMember4, default(Nullable<FormKey>))) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.FormKey>>.Default.Equals(item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.FormKey>))) return true;
        if (!EqualityComparer<FormKey>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<FormKey?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<FormKey>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.FormKey>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        while (kernel.TryGetNextField(out var name))
        {
            switch (name)
            {
                case: "SomeMember0":
                    item.SomeMember0 = kernel.ReadFormKey(writer);
                case: "SomeMember1":
                    item.SomeMember1 = kernel.ReadFormKey(writer);
                case: "SomeMember2":
                    item.SomeMember2 = kernel.ReadFormKey(writer);
                case: "SomeMember3":
                    item.SomeMember3 = kernel.ReadFormKey(writer);
                case: "SomeMember4":
                    item.SomeMember4 = kernel.ReadFormKey(writer);
                case: "SomeMember5":
                    item.SomeMember5 = kernel.ReadFormKey(writer);
                case: "SomeMember6":
                    item.SomeMember6 = kernel.ReadFormKey(writer);
                case: "SomeMember7":
                    item.SomeMember7 = kernel.ReadFormKey(writer);
                case: "SomeMember8":
                    item.SomeMember8 = kernel.ReadFormKey(writer);
                case: "SomeMember9":
                    item.SomeMember9 = kernel.ReadFormKey(writer);
                case: "SomeMember10":
                    item.SomeMember10 = kernel.ReadFormKey(writer);
                case: "SomeMember11":
                    item.SomeMember11 = kernel.ReadFormKey(writer);
                default:
                    break;
            }
        }
    }

}

