//HintName: TestMod_Serializations.g.cs
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using System.Drawing;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class TestMod_Serialization
{
    public static void Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
    {
        kernel.WriteColor(writer, "SomeMember0", item.SomeMember0, default(System.Drawing.Color));
        kernel.WriteColor(writer, "SomeMember1", item.SomeMember1, default(Color));
        kernel.WriteColor(writer, "SomeMember2", item.SomeMember2, default(System.Drawing.Color?));
        kernel.WriteColor(writer, "SomeMember3", item.SomeMember3, default(Color?));
        kernel.WriteColor(writer, "SomeMember4", item.SomeMember4, default(Nullable<System.Drawing.Color>));
        kernel.WriteColor(writer, "SomeMember5", item.SomeMember5, default(Nullable<Color>));
        kernel.WriteColor(writer, "SomeMember6", item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember6Default);
        kernel.WriteColor(writer, "SomeMember7", item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember7Default);
        kernel.WriteColor(writer, "SomeMember8", item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember8Default);
        kernel.WriteColor(writer, "SomeMember9", item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember9Default);
        kernel.WriteColor(writer, "SomeMember10", item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember10Default);
        kernel.WriteColor(writer, "SomeMember11", item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember11Default);
    }

    public static bool HasSerializationItems(Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter item)
    {
        if (!EqualityComparer<System.Drawing.Color>.Default.Equals(item.SomeMember0, default(System.Drawing.Color))) return true;
        if (!EqualityComparer<Color>.Default.Equals(item.SomeMember1, default(Color))) return true;
        if (!EqualityComparer<System.Drawing.Color?>.Default.Equals(item.SomeMember2, default(System.Drawing.Color?))) return true;
        if (!EqualityComparer<Color?>.Default.Equals(item.SomeMember3, default(Color?))) return true;
        if (!EqualityComparer<Nullable<System.Drawing.Color>>.Default.Equals(item.SomeMember4, default(Nullable<System.Drawing.Color>))) return true;
        if (!EqualityComparer<Nullable<Color>>.Default.Equals(item.SomeMember5, default(Nullable<Color>))) return true;
        if (!EqualityComparer<System.Drawing.Color>.Default.Equals(item.SomeMember6, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember6Default)) return true;
        if (!EqualityComparer<Color>.Default.Equals(item.SomeMember7, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember7Default)) return true;
        if (!EqualityComparer<System.Drawing.Color?>.Default.Equals(item.SomeMember8, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember8Default)) return true;
        if (!EqualityComparer<Color?>.Default.Equals(item.SomeMember9, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember9Default)) return true;
        if (!EqualityComparer<Nullable<System.Drawing.Color>>.Default.Equals(item.SomeMember10, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember10Default)) return true;
        if (!EqualityComparer<Nullable<Color>>.Default.Equals(item.SomeMember11, Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestModGetter.SomeMember11Default)) return true;
        return false;
    }

    public static Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ITestMod Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel)
    {
        throw new NotImplementedException();
    }

}

