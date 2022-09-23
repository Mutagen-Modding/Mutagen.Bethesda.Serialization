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
        kernel.WriteRecordType(writer, "SomeMember0", item.SomeMember0, default(RecordType));
        kernel.WriteRecordType(writer, "SomeMember1", item.SomeMember1, default(Mutagen.Bethesda.Plugins.RecordType));
        kernel.WriteRecordType(writer, "SomeMember2", item.SomeMember2, default(RecordType?));
        kernel.WriteRecordType(writer, "SomeMember3", item.SomeMember3, default(Mutagen.Bethesda.Plugins.RecordType?));
        kernel.WriteRecordType(writer, "SomeMember4", item.SomeMember4, default(Nullable<RecordType>));
        kernel.WriteRecordType(writer, "SomeMember5", item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.RecordType>));
        kernel.WriteRecordType(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default);
        kernel.WriteRecordType(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default);
        kernel.WriteRecordType(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default);
        kernel.WriteRecordType(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default);
        kernel.WriteRecordType(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default);
        kernel.WriteRecordType(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default);
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (!EqualityComparer<RecordType>.Default.Equals(item.SomeMember0, default(RecordType))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType>.Default.Equals(item.SomeMember1, default(Mutagen.Bethesda.Plugins.RecordType))) return true;
        if (!EqualityComparer<RecordType?>.Default.Equals(item.SomeMember2, default(RecordType?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType?>.Default.Equals(item.SomeMember3, default(Mutagen.Bethesda.Plugins.RecordType?))) return true;
        if (!EqualityComparer<Nullable<RecordType>>.Default.Equals(item.SomeMember4, default(Nullable<RecordType>))) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.RecordType>>.Default.Equals(item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.RecordType>))) return true;
        if (!EqualityComparer<RecordType>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember6Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember7Default)) return true;
        if (!EqualityComparer<RecordType?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember8Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.RecordType?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<RecordType>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.RecordType>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject.SomeMember11Default)) return true;
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
                    item.SomeMember0 = kernel.ReadRecordType(writer);
                case: "SomeMember1":
                    item.SomeMember1 = kernel.ReadRecordType(writer);
                case: "SomeMember2":
                    item.SomeMember2 = kernel.ReadRecordType(writer);
                case: "SomeMember3":
                    item.SomeMember3 = kernel.ReadRecordType(writer);
                case: "SomeMember4":
                    item.SomeMember4 = kernel.ReadRecordType(writer);
                case: "SomeMember5":
                    item.SomeMember5 = kernel.ReadRecordType(writer);
                case: "SomeMember6":
                    item.SomeMember6 = kernel.ReadRecordType(writer);
                case: "SomeMember7":
                    item.SomeMember7 = kernel.ReadRecordType(writer);
                case: "SomeMember8":
                    item.SomeMember8 = kernel.ReadRecordType(writer);
                case: "SomeMember9":
                    item.SomeMember9 = kernel.ReadRecordType(writer);
                case: "SomeMember10":
                    item.SomeMember10 = kernel.ReadRecordType(writer);
                case: "SomeMember11":
                    item.SomeMember11 = kernel.ReadRecordType(writer);
                default:
                    break;
            }
        }
    }

}

