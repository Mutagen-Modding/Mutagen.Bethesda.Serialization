//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteFormKey(writer, "SomeMember0", item.SomeMember0, default(FormKey));
        kernel.WriteFormKey(writer, "SomeMember1", item.SomeMember1, default(Mutagen.Bethesda.Plugins.FormKey));
        kernel.WriteFormKey(writer, "SomeMember2", item.SomeMember2, default(FormKey?));
        kernel.WriteFormKey(writer, "SomeMember3", item.SomeMember3, default(Mutagen.Bethesda.Plugins.FormKey?));
        kernel.WriteFormKey(writer, "SomeMember4", item.SomeMember4, default(Nullable<FormKey>));
        kernel.WriteFormKey(writer, "SomeMember5", item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.FormKey>));
        kernel.WriteFormKey(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember6Default);
        kernel.WriteFormKey(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember7Default);
        kernel.WriteFormKey(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember8Default);
        kernel.WriteFormKey(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember9Default);
        kernel.WriteFormKey(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember10Default);
        kernel.WriteFormKey(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember11Default);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        if (!EqualityComparer<FormKey>.Default.Equals(item.SomeMember0, default(FormKey))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey>.Default.Equals(item.SomeMember1, default(Mutagen.Bethesda.Plugins.FormKey))) return true;
        if (!EqualityComparer<FormKey?>.Default.Equals(item.SomeMember2, default(FormKey?))) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey?>.Default.Equals(item.SomeMember3, default(Mutagen.Bethesda.Plugins.FormKey?))) return true;
        if (!EqualityComparer<Nullable<FormKey>>.Default.Equals(item.SomeMember4, default(Nullable<FormKey>))) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.FormKey>>.Default.Equals(item.SomeMember5, default(Nullable<Mutagen.Bethesda.Plugins.FormKey>))) return true;
        if (!EqualityComparer<FormKey>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember6Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember7Default)) return true;
        if (!EqualityComparer<FormKey?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember8Default)) return true;
        if (!EqualityComparer<Mutagen.Bethesda.Plugins.FormKey?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<FormKey>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Mutagen.Bethesda.Plugins.FormKey>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.TestMod.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

