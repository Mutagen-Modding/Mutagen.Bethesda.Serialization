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
        if (item.SomeList is {} checkedSomeList
            && checkedSomeList.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList");
            foreach (var listItem in checkedSomeList)
            {
                kernel.WriteLoqui(writer, null, listItem, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList2 is {} checkedSomeList2
            && checkedSomeList2.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList2");
            foreach (var listItem in checkedSomeList2)
            {
                kernel.WriteLoqui(writer, null, listItem, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList3 is {} checkedSomeList3
            && checkedSomeList3.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList3");
            foreach (var listItem in checkedSomeList3)
            {
                kernel.WriteLoqui(writer, null, listItem, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
            }
            kernel.EndListSection(writer);
        }
        if (item.SomeList4 is {} checkedSomeList4
            && checkedSomeList4.Count > 0)
        {
            kernel.StartListSection(writer, "SomeList4");
            foreach (var listItem in checkedSomeList4)
            {
                kernel.WriteLoqui(writer, null, listItem, metaData, static (w, i, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Serialize<TKernel, TWriteObject>(w, i, k, m));
            }
            kernel.EndListSection(writer);
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        SerializationMetaData metaData)
    {
        if (item.SomeList.Count > 0) return true;
        if (item.SomeList2.Count > 0) return true;
        if (item.SomeList3.Count > 0) return true;
        if (item.SomeList4.Count > 0) return true;
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
                case "SomeList":
                    kernel.StartListSection(reader, "SomeList");
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = kernel.ReadLoqui(reader, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TKernel, TReadObject>(r, k, m));
                        item.SomeList.Add(item);
                    }
                    kernel.EndListSection(reader);
                case "SomeList2":
                    kernel.StartListSection(reader, "SomeList2");
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = kernel.ReadLoqui(reader, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TKernel, TReadObject>(r, k, m));
                        item.SomeList2.Add(item);
                    }
                    kernel.EndListSection(reader);
                case "SomeList3":
                    kernel.StartListSection(reader, "SomeList3");
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = kernel.ReadLoqui(reader, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TKernel, TReadObject>(r, k, m));
                        item.SomeList3.Add(item);
                    }
                    kernel.EndListSection(reader);
                case "SomeList4":
                    kernel.StartListSection(reader, "SomeList4");
                    while (kernel.TryHasNextItem(reader))
                    {
                        var item = kernel.ReadLoqui(reader, metaData, static (r, k, m) => Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeLoqui_Serialization.Deserialize<TKernel, TReadObject>(r, k, m));
                        item.SomeList4.Add(item);
                    }
                    kernel.EndListSection(reader);
                default:
                    break;
            }
        }
    }

}

