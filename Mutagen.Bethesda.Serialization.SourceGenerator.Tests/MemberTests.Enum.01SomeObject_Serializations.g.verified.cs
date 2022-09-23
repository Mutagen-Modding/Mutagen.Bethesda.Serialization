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
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum>(writer, "SomeEnum", item.SomeEnum, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum>(writer, "SomeEnum2", item.SomeEnum2, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2>(writer, "SomeEnum3", item.SomeEnum3, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2>(writer, "SomeEnum4", item.SomeEnum4, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3>(writer, "SomeEnum5", item.SomeEnum5, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3));
        kernel.WriteEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3>(writer, "SomeEnum6", item.SomeEnum6, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3));
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum>.Default.Equals(item.SomeEnum, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum>>.Default.Equals(item.SomeEnum2, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2>.Default.Equals(item.SomeEnum3, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2>>.Default.Equals(item.SomeEnum4, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2>))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3>.Default.Equals(item.SomeEnum5, default(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3))) return true;
        if (!EqualityComparer<System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3>>.Default.Equals(item.SomeEnum6, default(System.Nullable<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3>))) return true;
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
                case: "SomeEnum":
                    item.SomeEnum = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum>(writer);
                case: "SomeEnum2":
                    item.SomeEnum2 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum>(writer);
                case: "SomeEnum3":
                    item.SomeEnum3 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2>(writer);
                case: "SomeEnum4":
                    item.SomeEnum4 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum2>(writer);
                case: "SomeEnum5":
                    item.SomeEnum5 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3>(writer);
                case: "SomeEnum6":
                    item.SomeEnum6 = kernel.ReadEnum<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter.MyEnum3>(writer);
                default:
                    break;
            }
        }
    }

}

