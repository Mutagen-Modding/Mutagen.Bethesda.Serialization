using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Serialization.Yaml;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.UtilityTests;

public class ArrayHelperTests : UtilityTests
{
    [Theory, DefaultAutoData]
    public async Task ReadIntoArray(
        YamlSerializationReaderKernel kernel,
        SerializationMetaData meta)
    {
        var i = new int?[3];
        var str =
            """
            key:
            - 4
            - 5
            - 6
            """;
        var stream = GenerateStreamFromString(str);
        var reader = kernel.GetNewObject(new StreamPackage(stream, null));
        kernel.ReadString(reader);
        await SerializationHelper.ReadIntoArray(
            reader,
            i,
            kernel,
            meta,
            async (r, k, m) =>
            {
                return k.ReadInt32(r);
            });
        i.ShouldEqualEnumerable(4, 5, 6);
    }
    
    [Theory, DefaultAutoData]
    public async Task ReadArray(
        YamlSerializationReaderKernel kernel,
        SerializationMetaData meta)
    {
        var str =
            """
            key:
            - 4
            - 5
            - 6
            """;
        var stream = GenerateStreamFromString(str);
        var reader = kernel.GetNewObject(new StreamPackage(stream, null));
        kernel.ReadString(reader);
        var i = await SerializationHelper.ReadArray(
            reader,
            kernel,
            meta,
            async (r, k, m) =>
            {
                return k.ReadInt32(r);
            });
        i.ShouldEqualEnumerable(4, 5, 6);
    }
    
    [Theory, DefaultAutoData]
    public async Task ReadNotEnoughIntoArray(
        YamlSerializationReaderKernel kernel,
        SerializationMetaData meta)
    {
        var i = new int?[5];
        var str =
            """
            key:
            - 4
            - 5
            - 6
            """;
        var stream = GenerateStreamFromString(str);
        var reader = kernel.GetNewObject(new StreamPackage(stream, null));
        kernel.ReadString(reader);

        await Assert.ThrowsAsync<DataMisalignedException>(async () =>
        {
            await SerializationHelper.ReadIntoArray(
                reader,
                i,
                kernel,
                meta,
                async (r, k, m) =>
                {
                    return k.ReadInt32(r);
                });
        });
    }
}