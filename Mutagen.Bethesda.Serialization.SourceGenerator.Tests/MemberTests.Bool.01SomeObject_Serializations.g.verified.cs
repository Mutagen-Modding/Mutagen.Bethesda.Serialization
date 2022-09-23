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
        kernel.WriteBool(writer, "SomeMember0", item.SomeMember0, default(bool));
        kernel.WriteBool(writer, "SomeMember1", item.SomeMember1, default(Boolean));
        kernel.WriteBool(writer, "SomeMember2", item.SomeMember2, default(bool?));
        kernel.WriteBool(writer, "SomeMember3", item.SomeMember3, default(Boolean?));
        kernel.WriteBool(writer, "SomeMember4", item.SomeMember4, default(Nullable<bool>));
        kernel.WriteBool(writer, "SomeMember5", item.SomeMember5, default(Nullable<Boolean>));
        kernel.WriteBool(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteBool(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteBool(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteBool(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteBool(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteBool(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<bool>.Default.Equals(item.SomeMember0, default(bool))) return true;
        if (!EqualityComparer<Boolean>.Default.Equals(item.SomeMember1, default(Boolean))) return true;
        if (!EqualityComparer<bool?>.Default.Equals(item.SomeMember2, default(bool?))) return true;
        if (!EqualityComparer<Boolean?>.Default.Equals(item.SomeMember3, default(Boolean?))) return true;
        if (!EqualityComparer<Nullable<bool>>.Default.Equals(item.SomeMember4, default(Nullable<bool>))) return true;
        if (!EqualityComparer<Nullable<Boolean>>.Default.Equals(item.SomeMember5, default(Nullable<Boolean>))) return true;
        if (!EqualityComparer<bool>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Boolean>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<bool?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Boolean?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<bool>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Boolean>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
                    item.SomeMember0 = kernel.ReadBool(writer);
                case: "SomeMember1":
                    item.SomeMember1 = kernel.ReadBool(writer);
                case: "SomeMember2":
                    item.SomeMember2 = kernel.ReadBool(writer);
                case: "SomeMember3":
                    item.SomeMember3 = kernel.ReadBool(writer);
                case: "SomeMember4":
                    item.SomeMember4 = kernel.ReadBool(writer);
                case: "SomeMember5":
                    item.SomeMember5 = kernel.ReadBool(writer);
                case: "SomeMember6":
                    item.SomeMember6 = kernel.ReadBool(writer);
                case: "SomeMember7":
                    item.SomeMember7 = kernel.ReadBool(writer);
                case: "SomeMember8":
                    item.SomeMember8 = kernel.ReadBool(writer);
                case: "SomeMember9":
                    item.SomeMember9 = kernel.ReadBool(writer);
                case: "SomeMember10":
                    item.SomeMember10 = kernel.ReadBool(writer);
                case: "SomeMember11":
                    item.SomeMember11 = kernel.ReadBool(writer);
                default:
                    break;
            }
        }
    }

}

