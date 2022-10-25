//HintName: SomeObject_Serializations.g.cs
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
        kernel.WriteString(writer, "SomeMember0", item.SomeMember0, default(string));
        kernel.WriteString(writer, "SomeMember1", item.SomeMember1, default(String));
        kernel.WriteString(writer, "SomeMember2", item.SomeMember2, default(string?));
        kernel.WriteString(writer, "SomeMember3", item.SomeMember3, default(String?));
        kernel.WriteString(writer, "SomeMember4", item.SomeMember4, default(Nullable<string>));
        kernel.WriteString(writer, "SomeMember5", item.SomeMember5, default(Nullable<String>));
        kernel.WriteString(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteString(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteString(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteString(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteString(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteString(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<string>.Default.Equals(item.SomeMember0, default(string))) return true;
        if (!EqualityComparer<String>.Default.Equals(item.SomeMember1, default(String))) return true;
        if (!EqualityComparer<string?>.Default.Equals(item.SomeMember2, default(string?))) return true;
        if (!EqualityComparer<String?>.Default.Equals(item.SomeMember3, default(String?))) return true;
        if (!EqualityComparer<Nullable<string>>.Default.Equals(item.SomeMember4, default(Nullable<string>))) return true;
        if (!EqualityComparer<Nullable<String>>.Default.Equals(item.SomeMember5, default(Nullable<String>))) return true;
        if (!EqualityComparer<string>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<String>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<string?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<String?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<string>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<String>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
                case "SomeMember0":
                    obj.SomeMember0 = kernel.ReadString(reader);
                    break;
                case "SomeMember1":
                    obj.SomeMember1 = kernel.ReadString(reader);
                    break;
                case "SomeMember2":
                    obj.SomeMember2 = kernel.ReadString(reader);
                    break;
                case "SomeMember3":
                    obj.SomeMember3 = kernel.ReadString(reader);
                    break;
                case "SomeMember4":
                    obj.SomeMember4 = kernel.ReadString(reader);
                    break;
                case "SomeMember5":
                    obj.SomeMember5 = kernel.ReadString(reader);
                    break;
                case "SomeMember6":
                    obj.SomeMember6 = kernel.ReadString(reader);
                    break;
                case "SomeMember7":
                    obj.SomeMember7 = kernel.ReadString(reader);
                    break;
                case "SomeMember8":
                    obj.SomeMember8 = kernel.ReadString(reader);
                    break;
                case "SomeMember9":
                    obj.SomeMember9 = kernel.ReadString(reader);
                    break;
                case "SomeMember10":
                    obj.SomeMember10 = kernel.ReadString(reader);
                    break;
                case "SomeMember11":
                    obj.SomeMember11 = kernel.ReadString(reader);
                    break;
                default:
                    break;
            }
        }

    }

}

