﻿//HintName: SomeObject_Serializations.g.cs
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
        kernel.WriteP2Int(writer, "SomeMember0", item.SomeMember0, default(Noggog.P2Int));
        kernel.WriteP2Int(writer, "SomeMember1", item.SomeMember1, default(Noggog.P2Int?));
        kernel.WriteP2Int(writer, "SomeMember2", item.SomeMember2, default(Nullable<Noggog.P2Int>));
        kernel.WriteP2Int(writer, "SomeMember3", item.SomeMember3, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember3Default);
        kernel.WriteP2Int(writer, "SomeMember4", item.SomeMember4, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember4Default);
        kernel.WriteP2Int(writer, "SomeMember5", item.SomeMember5, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember5Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<Noggog.P2Int>.Default.Equals(item.SomeMember0, default(Noggog.P2Int))) return true;
        if (!EqualityComparer<Noggog.P2Int?>.Default.Equals(item.SomeMember1, default(Noggog.P2Int?))) return true;
        if (!EqualityComparer<Nullable<Noggog.P2Int>>.Default.Equals(item.SomeMember2, default(Nullable<Noggog.P2Int>))) return true;
        if (!EqualityComparer<Noggog.P2Int>.Default.Equals(item.SomeMember3, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember3Default)) return true;
        if (!EqualityComparer<Noggog.P2Int?>.Default.Equals(item.SomeMember4, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember4Default)) return true;
        if (!EqualityComparer<Nullable<Noggog.P2Int>>.Default.Equals(item.SomeMember5, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember5Default)) return true;
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
                    obj.SomeMember0 = kernel.ReadP2Int(reader);
                    break;
                case "SomeMember1":
                    obj.SomeMember1 = kernel.ReadP2Int(reader);
                    break;
                case "SomeMember2":
                    obj.SomeMember2 = kernel.ReadP2Int(reader);
                    break;
                case "SomeMember3":
                    obj.SomeMember3 = kernel.ReadP2Int(reader);
                    break;
                case "SomeMember4":
                    obj.SomeMember4 = kernel.ReadP2Int(reader);
                    break;
                case "SomeMember5":
                    obj.SomeMember5 = kernel.ReadP2Int(reader);
                    break;
                default:
                    break;
            }
        }

    }

}
