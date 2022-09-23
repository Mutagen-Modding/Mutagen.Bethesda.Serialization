//HintName: SomeObject_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using System.Drawing;

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
        kernel.WriteColor(writer, "SomeMember0", item.SomeMember0, default(System.Drawing.Color));
        kernel.WriteColor(writer, "SomeMember1", item.SomeMember1, default(Color));
        kernel.WriteColor(writer, "SomeMember2", item.SomeMember2, default(System.Drawing.Color?));
        kernel.WriteColor(writer, "SomeMember3", item.SomeMember3, default(Color?));
        kernel.WriteColor(writer, "SomeMember4", item.SomeMember4, default(Nullable<System.Drawing.Color>));
        kernel.WriteColor(writer, "SomeMember5", item.SomeMember5, default(Nullable<Color>));
        kernel.WriteColor(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteColor(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteColor(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteColor(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteColor(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteColor(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<System.Drawing.Color>.Default.Equals(item.SomeMember0, default(System.Drawing.Color))) return true;
        if (!EqualityComparer<Color>.Default.Equals(item.SomeMember1, default(Color))) return true;
        if (!EqualityComparer<System.Drawing.Color?>.Default.Equals(item.SomeMember2, default(System.Drawing.Color?))) return true;
        if (!EqualityComparer<Color?>.Default.Equals(item.SomeMember3, default(Color?))) return true;
        if (!EqualityComparer<Nullable<System.Drawing.Color>>.Default.Equals(item.SomeMember4, default(Nullable<System.Drawing.Color>))) return true;
        if (!EqualityComparer<Nullable<Color>>.Default.Equals(item.SomeMember5, default(Nullable<Color>))) return true;
        if (!EqualityComparer<System.Drawing.Color>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Color>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<System.Drawing.Color?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Color?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<System.Drawing.Color>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Color>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
                    item.SomeMember0 = kernel.ReadColor(writer);
                case: "SomeMember1":
                    item.SomeMember1 = kernel.ReadColor(writer);
                case: "SomeMember2":
                    item.SomeMember2 = kernel.ReadColor(writer);
                case: "SomeMember3":
                    item.SomeMember3 = kernel.ReadColor(writer);
                case: "SomeMember4":
                    item.SomeMember4 = kernel.ReadColor(writer);
                case: "SomeMember5":
                    item.SomeMember5 = kernel.ReadColor(writer);
                case: "SomeMember6":
                    item.SomeMember6 = kernel.ReadColor(writer);
                case: "SomeMember7":
                    item.SomeMember7 = kernel.ReadColor(writer);
                case: "SomeMember8":
                    item.SomeMember8 = kernel.ReadColor(writer);
                case: "SomeMember9":
                    item.SomeMember9 = kernel.ReadColor(writer);
                case: "SomeMember10":
                    item.SomeMember10 = kernel.ReadColor(writer);
                case: "SomeMember11":
                    item.SomeMember11 = kernel.ReadColor(writer);
                default:
                    break;
            }
        }
    }

}

