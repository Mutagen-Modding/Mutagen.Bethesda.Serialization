//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.WorkEngine;
using System.IO.Abstractions;
using System.Threading.Tasks;

#nullable enable

#pragma warning disable CS1998 // No awaits used
#pragma warning disable CS0618 // Obsolete

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        await SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static async Task SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        kernel.WriteP2Float(writer, "SomeMember0", item.SomeMember0, default(Noggog.P2Float));
        kernel.WriteP2Float(writer, "SomeMember1", item.SomeMember1, default(Noggog.P2Float?));
        kernel.WriteP2Float(writer, "SomeMember2", item.SomeMember2, default(Nullable<Noggog.P2Float>));
        kernel.WriteP2Float(writer, "SomeMember3", item.SomeMember3, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember3Default);
        kernel.WriteP2Float(writer, "SomeMember4", item.SomeMember4, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember4Default);
        kernel.WriteP2Float(writer, "SomeMember5", item.SomeMember5, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember5Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        if (item == null) return false;
        if (!P2Float.NullableRawEqualityComparer.Equals(item.SomeMember0, default(Noggog.P2Float))) return true;
        if (!P2Float.NullableRawEqualityComparer.Equals(item.SomeMember1, default(Noggog.P2Float?))) return true;
        if (!P2Float.NullableRawEqualityComparer.Equals(item.SomeMember2, default(Nullable<Noggog.P2Float>))) return true;
        if (!P2Float.NullableRawEqualityComparer.Equals(item.SomeMember3, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember3Default)) return true;
        if (!P2Float.NullableRawEqualityComparer.Equals(item.SomeMember4, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember4Default)) return true;
        if (!P2Float.NullableRawEqualityComparer.Equals(item.SomeMember5, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember5Default)) return true;
        return false;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        await DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static async Task DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        switch (name)
        {
            case "SomeMember0":
                obj.SomeMember0 = SerializationHelper.StripNull(kernel.ReadP2Float(reader), name: "SomeMember0");
                break;
            case "SomeMember1":
                obj.SomeMember1 = kernel.ReadP2Float(reader);
                break;
            case "SomeMember2":
                obj.SomeMember2 = kernel.ReadP2Float(reader);
                break;
            case "SomeMember3":
                obj.SomeMember3 = SerializationHelper.StripNull(kernel.ReadP2Float(reader), name: "SomeMember3");
                break;
            case "SomeMember4":
                obj.SomeMember4 = kernel.ReadP2Float(reader);
                break;
            case "SomeMember5":
                obj.SomeMember5 = kernel.ReadP2Float(reader);
                break;
            default:
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        while (kernel.TryGetNextField(reader, out var name))
        {
            await DeserializeSingleFieldInto<TReadObject>(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

    }

}

